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

using RobotLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;

namespace TdsCommons
{
    public class TickServerReader<T> : IDisposable where T : struct
    {
        public delegate void BarOnTickDelegate();
        public T ServerTick;

        private AbstractRobot mBot;
        private readonly string mPipeName;
        private readonly NamedPipeClientStream mPipe;
        private readonly BinaryReader mReader;
        private byte[] mPeekBuffer;
        private List<BarOnTickDelegate> mRegisteredBars = new List<BarOnTickDelegate>();
        private bool mIsInit = true;

        public TickServerReader(AbstractRobot abstractRobot, string pipeName)
        {
            mBot = abstractRobot;
            mPipeName = pipeName.Replace(">>", "").Replace(" ", "");
            mPipe = new NamedPipeClientStream(".", mPipeName, PipeDirection.In);
            try
            {
                mPipe.Connect(3000);
            }
            catch
            {
                throw (new Exception("NinjaTrader TickServer strategy is not running or Symbol Pairs are not correct. Start NinjaTrader TickServer and cTrader Algo with correct Symbol Pairs"));
            }
            mReader = new BinaryReader(mPipe);
        }

        public void RegisterBar(BarOnTickDelegate onTick)
        {
            mRegisteredBars.Add(onTick);
            if (!TickServerReaderOnTick())
                throw (new Exception("NinjaTrader start date must be before cTrader start date"));
        }

        public bool TickServerReaderOnTick()
        {
            long ntNative;
            var ctNative = mBot.Time.ToNativeSec();
            do
            {
                if (!TryPeek(out ServerTick))
                    return false;

                dynamic dynTick = ServerTick;
                var ntTime = (DateTime)dynTick.Time;
                ntNative = ntTime.ToNativeSec();

                // When NT is ahead of cTrader, do not consume the NT tick but return
                if (ntNative > ctNative)
                    return !mIsInit;    // if its the 1st time, its an error

                mIsInit = false;

                TryDequeue(out ServerTick);
                foreach (var bar in mRegisteredBars)
                    bar.Invoke();

            } while (ntNative < ctNative);  // loop til NinjaTrader time is >= cTrader time
            return true;
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
