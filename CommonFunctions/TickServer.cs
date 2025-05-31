#region Usings
#if !CTRADER
using NinjaTrader.Data;
using NinjaTrader.Cbi;
#endif
using cAlgo.API;
using System;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using TdsCommons;
using System.Threading.Tasks;
#endregion

namespace cAlgo.Robots
{
    public class TickServer
    {
#if CTRADER
        public Ringbuffer<QcBar> QcBars = new Ringbuffer<QcBar>(2);
        private NamedPipeClientStream mPipe;
        private long mPrevNative;
#else
        private NamedPipeServerStream mPipe;
        private TickserverMarketDataArgs mPrevBarArgs;
        private ulong mMessageId;
#endif
        private readonly string mPipeName;
        private readonly int mStructSize;
        private readonly Robot mBot;

        public TickServer(Robot robot, string pipeName)
        {
            mBot = robot;
            mPipeName = pipeName;
            mStructSize = Marshal.SizeOf(typeof(TickserverMarketDataArgs));
#if CTRADER
            mPipe = new NamedPipeClientStream(".", mPipeName, PipeDirection.In);
            mPipe.Connect();
#else
            mPipe = new NamedPipeServerStream(mPipeName,
                PipeDirection.Out,
                1,
                PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous);
#endif
        }

        public void Dispose()
        {
            mPipe?.Dispose();
        }

        public bool IsConnected => mPipe.IsConnected;

#if CTRADER

        public bool Read(out TickserverMarketDataArgs data)
        {
            byte[] buffer = new byte[mStructSize];
            int totalRead = 0;

            while (totalRead < mStructSize)
            {
                int bytesRead = mPipe.Read(buffer, totalRead, mStructSize - totalRead);
                if (bytesRead == 0)
                {
                    data = default;
                    return false; // Pipe closed or no data
                }
                totalRead += bytesRead;
            }

            data = ByteArrayToStructure<TickserverMarketDataArgs>(buffer);
            return true;
        }

        private static T2 ByteArrayToStructure<T2>(byte[] bytes) where T2 : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return Marshal.PtrToStructure<T2>(handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }
        }

        public string TickServerOnTick(DateTime from, DateTime to, int barSeconds, bool xy, bool doPrint)
        {
            TickserverMarketDataArgs args = default;
            var fromNative = from.ToNativeSec();
            var toNative = to.ToNativeSec();
            long argsNative = 0;

            // Step 1: Skip stale ticks before 'from'
            while (true)
            {
                if (!Read(out args))
                    return "Server closed the connection";

                if (fromNative < argsNative)
                    return "NinjaTrader start time must be BEFORE cTrader start time";

                if (doPrint)
                    mBot.Print(args.MessageId);

                argsNative = args.Time.ToNativeSec();

                if (argsNative >= fromNative)
                    break;
            }

            var qcBar = new QcBar();
            if (null == qcBar || CoFu.IsNewBar(barSeconds, argsNative, mPrevNative))
            {
                QcBars.Add(qcBar);

                qcBar = new QcBar();
                qcBar.TimeOpen = (fromNative - (fromNative % barSeconds)).FromNativeSec();
                qcBar.BidOpen = qcBar.BidHigh = qcBar.BidLow = args.Bid;
                qcBar.AskOpen = qcBar.AskHigh = qcBar.AskLow = args.Ask;
                // qcBar.AskVolume and qcBar.BidVolume are alway 0 on a new qcBar();
            }

            // Step 3: Define tick accumulation logic as local function
            void AccumulateTick(TickserverMarketDataArgs tick)
            {
                qcBar.BidHigh = Math.Max(qcBar.BidHigh, tick.Bid);
                qcBar.BidLow = Math.Min(qcBar.BidLow, tick.Bid);
                qcBar.AskHigh = Math.Max(qcBar.AskHigh, tick.Ask);
                qcBar.AskLow = Math.Min(qcBar.AskLow, tick.Ask);
                qcBar.BidClose = tick.Bid;
                qcBar.AskClose = tick.Ask;

                if (tick.Ask != tick.Bid)
                {
                    if (tick.Price >= tick.Ask)
                        qcBar.AskVolume += tick.Volume;
                    else if (tick.Price <= tick.Bid)
                        qcBar.BidVolume += tick.Volume;
                }
            }

            // Step 4: Accumulate initial tick
            AccumulateTick(args);

            mPrevNative = argsNative;

            return "";
        }
#else
        public bool Write(TickserverMarketDataArgs data)
        {
            data.MessageId = ++mMessageId;
            mBot.Print(data.MessageId);
            byte[] buffer = StructureToByteArray(data);
            try
            {
                mPipe.Write(buffer, 0, buffer.Length);
                mPipe.Flush();
                return true;
            }
            catch
            {
                return false; // Pipe might be broken or closed
            }
        }

        private static byte[] StructureToByteArray(TickserverMarketDataArgs obj)
        {
            byte[] buffer = new byte[Marshal.SizeOf<TickserverMarketDataArgs>()];
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr(obj, handle.AddrOfPinnedObject(), false);
                return buffer;
            }
            finally
            {
                handle.Free();
            }
        }

        public void WaitForConnection()
        {
            mPipe.WaitForConnection();
        }

        public Task WaitForConnectionAsync()
        {
            return mPipe.WaitForConnectionAsync();
        }

#endif
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TickserverMarketDataArgs
    {
        public double Ask;
        public double Bid;
        public double Last;
#if !CTRADER
        public Instrument Instrument;
#endif
        public MarketDataType MarketDataType;
        public double Price;
        public long TimeTicks;
        public long Volume;
        public ulong MessageId;

        public DateTime Time
        {
            get => new DateTime(TimeTicks);
            set => TimeTicks = value.Ticks;
        }
    }
}
// End of file
