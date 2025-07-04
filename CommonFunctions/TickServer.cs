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
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading;

namespace TdsCommons
{
    public class TickServerWriter<T> : IDisposable where T : struct
    {
        private readonly int mCapacity;
        private readonly string mPipeName;
        private readonly SemaphoreSlim mSlotSemaphore;
        private readonly SemaphoreSlim mItemSemaphore;
        private readonly ConcurrentQueue<T> mQueue;
        private readonly Thread mWriterThread;
        private readonly CancellationTokenSource mCts = new();
        private bool mWriterDied;

        public TickServerWriter(int capacity, string pipeName)
        {
            mPipeName = pipeName.Replace(">>", "");
            mCapacity = capacity;
            mSlotSemaphore = new SemaphoreSlim(capacity, capacity);
            mItemSemaphore = new SemaphoreSlim(0, capacity);
            mQueue = new ConcurrentQueue<T>();
            mWriterThread = new Thread(WriterLoop) { IsBackground = true };
            mWriterThread.Start();
        }

        public bool BlockingEnqueue(T item)
        {
            try
            {
                while (!mSlotSemaphore.Wait(1000))
                    if (mWriterDied)
                        return false;

                mQueue.Enqueue(item);
                mItemSemaphore.Release();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void WriterLoop()
        {
            try
            {
                using var pipe = new NamedPipeServerStream(mPipeName, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                pipe.WaitForConnection();

                using var writer = new BinaryWriter(pipe);
                while (!mCts.IsCancellationRequested)
                {
                    mItemSemaphore.Wait(mCts.Token);

                    if (mQueue.TryDequeue(out var item))
                    {
                        var bytes = StructToBytes(item);
                        writer.Write(bytes.Length);
                        writer.Write(bytes);
                        writer.Flush();
                        mSlotSemaphore.Release();
                    }
                }
            }
            catch (Exception)
            {
                mWriterDied = true;
            }
        }

        private static byte[] StructToBytes(T data)
        {
            int size = Marshal.SizeOf<T>();
            var buffer = new byte[size];
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr(data, handle.AddrOfPinnedObject(), false);
                return buffer;
            }
            finally
            {
                handle.Free();
            }
        }

        public void Dispose()
        {
            mCts.Cancel();
            mWriterThread.Join();
            mCts.Dispose();
            mSlotSemaphore.Dispose();
            mItemSemaphore.Dispose();
        }
    }

    public class TickServerReader<T> : IDisposable where T : struct
    {
        private readonly string mPipeName;
        private readonly NamedPipeClientStream mPipe;
        private readonly BinaryReader mReader;
        private byte[] mPeekBuffer;

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
