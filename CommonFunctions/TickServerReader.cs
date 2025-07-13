/* MIT License
Copyright (c) 2025 Quantrosoft Pty. Ltd.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. 
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;

namespace TdsCommons
{
    public class TickServerReader<T> : IDisposable where T : struct
    {
        public delegate void BarOnTickDelegate(DateTime time, DateTime _);
        private readonly string mPipeName;
        private readonly NamedPipeClientStream mPipe;
        private readonly BinaryReader mReader;
        private byte[] mPeekBuffer;
        private bool mIsInit;
        private long mNtPrev;
        private List<BarOnTickDelegate> mRegisteredBars;

        public TickServerReader(string pipeName)
        {
            mPipeName = pipeName.Replace(">>", "");
            mPipe = new NamedPipeClientStream(".", mPipeName, PipeDirection.In);
            try
            {
                mPipe.Connect(3000);
            }
            catch
            {
                throw (new Exception("NinjaTrader is not running or Symbol Pairs are not correct. Start NinjaTrader and cTrader with correct Symbol Pairs"));
            }
            mReader = new BinaryReader(mPipe);
        }

        public void RegisterBar(BarOnTickDelegate onTick)
        { 
            mRegisteredBars.Add(onTick);
        }

        public void OnTick(DateTime ctTime, DateTime ctPrevTime)
        {
            long ntNative;
            var ctNative = ctTime.ToNativeSec();
            do
            {
                if (!TryPeek(out T serverTick))
                    return;

                //if (mIsInit && serverTick.Time > ctTime - TimeSpan.FromSeconds(10 * TimeFrameSeconds))
                //    throw new Exception("Set start date of NinjaTrader earlier than cTrader");

                dynamic dynTick = serverTick;
                ntNative = ((DateTime)dynTick.Time).ToNativeSec();
                //var isNewServerBar = CoFu.IsNewBar(TimeFrameSeconds, ntNative, mNtPrev);
                //mIsNewServerBar = isNewServerBar && ntNative == ctNative;

                // When NT is ahead of cTrader, do not consume the NT tick but return
                if (ntNative > ctNative)
                    break;

                TryDequeue(out serverTick);
                //UpdateNtBar(mIsInit || isNewServerBar); call bar update method here

                mNtPrev = ntNative;
            } while (ntNative < ctNative);  // loop til NinjaTrader time is >= cTrader time
        }

        public bool TryPeek(out T item)
        {
            item = default;
            if (mPeekBuffer != null)
            {
                item = BytesToStruct(mPeekBuffer);
                return true;
            }

            try
            {
                int size = mReader.ReadInt32();
                mPeekBuffer = mReader.ReadBytes(size);
                item = BytesToStruct(mPeekBuffer);
                return true;
            }
            catch
            {
                mPeekBuffer = null;
                return false;
            }
        }

        public bool TryDequeue(out T item)
        {
            item = default;
            if (mPeekBuffer != null)
            {
                item = BytesToStruct(mPeekBuffer);
                mPeekBuffer = null;
                return true;
            }

            try
            {
                int size = mReader.ReadInt32();
                byte[] buffer = mReader.ReadBytes(size);
                item = BytesToStruct(buffer);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static T BytesToStruct(byte[] data)
        {
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                return Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }
        }

        public void Dispose()
        {
            mReader.Dispose();
            mPipe.Dispose();
        }
    }
}
// end of file
