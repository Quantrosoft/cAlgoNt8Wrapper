#region Usings
#if !CTRADER
using NinjaTrader.Data;
using NinjaTrader.Cbi;
#endif
using System;
using System.Runtime.InteropServices;
using TdsCommons;
#endregion

namespace TdsCommons
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TickserverMarketDataArgs
    {
        public double Ask;
        public double Bid;
        public double Last;
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
