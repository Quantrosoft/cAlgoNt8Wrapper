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
using System.Threading.Tasks;

namespace TdsCommons
{
    public class TickServerWriter<T> : IDisposable where T : struct
    {
        private readonly string mPipeName;
        private readonly SemaphoreSlim mSlotSemaphore;
        private readonly SemaphoreSlim mItemSemaphore;
        private readonly ConcurrentQueue<T> mQueue;
        private readonly Thread mWriterThread;
        private readonly CancellationTokenSource mCts = new();
        private Task mWriterTask;
        private bool mWriterDied;

        public TickServerWriter(int capacity, string pipeName)
        {
            mPipeName = pipeName;
            mSlotSemaphore = new SemaphoreSlim(capacity, capacity);
            mItemSemaphore = new SemaphoreSlim(0, capacity);
            mQueue = new ConcurrentQueue<T>();

            mWriterThread = new Thread(() =>
            {
                mWriterTask = WriterLoopAsync(mCts.Token);
                try
                {
                    mWriterTask.Wait();
                }
                catch { }
            })
            {
                IsBackground = true
            };

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
            catch
            {
                return false;
            }
        }

        private async Task WriterLoopAsync(CancellationToken token)
        {
            try
            {
                using (var pipe = new NamedPipeServerStream(
                    mPipeName,
                    PipeDirection.Out,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous))
                {
                    await pipe.WaitForConnectionAsync(token).ConfigureAwait(false);

                    using (var writer = new BinaryWriter(pipe))
                    {
                        while (!token.IsCancellationRequested)
                        {
                            await mItemSemaphore.WaitAsync(token).ConfigureAwait(false);

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
                }
            }
            catch (OperationCanceledException)
            {
                // Normal exit
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

            mWriterThread.Join(3000);

            if (mWriterTask != null && !mWriterTask.IsCompleted)
            {
                try
                { mWriterTask.Wait(); }
                catch { }
            }

            mCts.Dispose();
            mSlotSemaphore.Dispose();
            mItemSemaphore.Dispose();
        }
    }
}
// end of file
