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
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;

/* Usage Examples for Multi-Process FIFO Operations:
 *
 * // Producer Process:
 * using var buffer = new RingMemoryMappedFile<double>(1000, "SharedBuffer", true);
 * 
 * // Add items to FIFO buffer (never blocks, overwrites oldest)
 * buffer.Enqueue(3.14159);
 * buffer.Enqueue(2.71828);
 * buffer.Enqueue(1.41421);
 * 
 * // Add items with blocking when buffer is full
 * buffer.BlockingEnqueue(42.0);  // Will wait if buffer is full
 * buffer.BlockingEnqueue(99.9, TimeSpan.FromSeconds(30));  // With custom timeout
 * 
 * // Consumer Process:
 * using var buffer = new RingMemoryMappedFile<double>("SharedBuffer");
 * 
 * // Read items in FIFO order
 * if (buffer.TryDequeue(out double value))
 *     Console.WriteLine($"Dequeued: {value}"); // Will print 3.14159 (first added)
 * 
 * if (buffer.TryDequeue(out double value2))
 *     Console.WriteLine($"Dequeued: {value2}"); // Will print 2.71828 (second added)
 * 
 * // Peek without removing
 * if (buffer.TryPeek(out double peekValue))
 *     Console.WriteLine($"Next item: {peekValue}");
 * 
 * // Thread-safe producer-consumer example with blocking:
 * try
 * {
 *     using var buffer = new RingMemoryMappedFile<int>(100, "MyIntBuffer");
 *     
 *     // Producer thread (blocking mode)
 *     Task.Run(() =>
 *     {
 *         for (int i = 0; i < 1000; i++)
 *         {
 *             buffer.BlockingEnqueue(i); // Will wait for space
 *             Thread.Sleep(10);
 *         }
 *     });
 *     
 *     // Consumer thread
 *     Task.Run(() =>
 *     {
 *         while (true)
 *         {
 *             if (buffer.TryDequeue(out int value))
 *                 Console.WriteLine($"Consumed: {value}");
 *             else
 *                 Thread.Sleep(50); // Brief pause when empty
 *         }
 *     });
 * }
 * catch (Exception ex)
 * {
 *     Console.WriteLine($"Error: {ex.Message}");
 * }
 */

namespace TdsCommons
{
    /// <summary>
    /// Represents a fixed length FIFO ring buffer using memory-mapped files to store a 
    /// specified maximal count of items. Newest values overwrite oldest in a non-blocking,
    /// thread-safe manner.
    /// </summary>
    /// <typeparam name="T">The generic type of the items stored within the ring buffer. 
    /// Must be a value type.</typeparam>
    public class RingMemoryMappedFile<T> : IDisposable where T : struct
    {
        #region Private variables
        /// <summary>
        /// The write position within the ring buffer (head pointer)
        /// </summary>
        protected int mWritePosition;

        /// <summary>
        /// The read position within the ring buffer (tail pointer)
        /// </summary>
        protected int mReadPosition;

        /// <summary>
        /// The current version of the buffer, this is required for a correct 
        /// exception handling while enumerating over the items of the buffer.
        /// </summary>
        private long mVersion;

        /// <summary>
        /// Memory-mapped file for the buffer data
        /// </summary>
        private MemoryMappedFile mMmf;

        /// <summary>
        /// Accessor for the memory-mapped file
        /// </summary>
        private MemoryMappedViewAccessor mAccessor;

        /// <summary>
        /// Size of each item in bytes
        /// </summary>
        private readonly int mItemSize;

        /// <summary>
        /// Total size of the memory-mapped file in bytes
        /// </summary>
        private readonly long mTotalSize;

        /// <summary>
        /// Name of the memory-mapped file
        /// </summary>
        private readonly string mMmfName;

        /// <summary>
        /// Whether this instance owns the memory-mapped file
        /// </summary>
        private bool mOwnsFile;

        /// <summary>
        /// Whether the object has been disposed
        /// </summary>
        private bool mDisposed;

        /// <summary>
        /// ReaderWriterLockSlim for thread-safe access
        /// </summary>
        private ReaderWriterLockSlim mLock;

        /// <summary>
        /// Event to signal when space becomes available in the buffer
        /// </summary>
        private ManualResetEventSlim mSpaceAvailableEvent;

        /// <summary>
        /// Timeout for lock operations (milliseconds)
        /// </summary>
        public int LockTimeout { get; set; } = 5000;

        /// <summary>
        /// Default timeout for blocking operations
        /// </summary>
        public int BlockingTimeout { get; set; } = Timeout.Infinite;

        /// <summary>
        /// Total number of items ever written to the buffer
        /// </summary>
        private long mTotalWritten;

        /// <summary>
        /// Total number of items ever read from the buffer
        /// </summary>
        private long mTotalRead;
        #endregion

        #region Public variables
        /// <summary>
        /// Gets the maximal count of items within the ring buffer.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Get the current count of items within the ring buffer.
        /// </summary>
        public int Count
        {
            get
            {
                mLock.EnterReadLock();
                try
                {
                    LoadMetadata();
                    return CalculateCount();
                }
                finally
                {
                    mLock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Total number of items ever written to the buffer
        /// </summary>
        public long TotalWritten
        {
            get
            {
                mLock.EnterReadLock();
                try
                {
                    LoadMetadata();
                    return mTotalWritten;
                }
                finally
                {
                    mLock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Total number of items ever read from the buffer
        /// </summary>
        public long TotalRead
        {
            get
            {
                mLock.EnterReadLock();
                try
                {
                    LoadMetadata();
                    return mTotalRead;
                }
                finally
                {
                    mLock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Gets whether the buffer is empty
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Gets whether the buffer is full (all slots occupied)
        /// </summary>
        public bool IsFull => Count == Size;

        /// <summary>
        /// Gets whether data has been lost due to overwriting
        /// </summary>
        public bool HasDataLoss => TotalWritten - TotalRead > Size;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <see cref="RingMemoryMappedFile<T>"/> with a specified 
        /// cache size using memory-mapped files.
        /// </summary>
        /// <param name="slots">The maximal count of items to be stored within the ring buffer.</param>
        /// <param name="mmfName">Optional name for the memory-mapped file. If null, a unique name will be generated.</param>
        /// <param name="createNew">If true, creates a new memory-mapped file. If false, tries to open existing one.</param>
        public RingMemoryMappedFile(int slots, string mmfName = null, bool createNew = true)
        {
            if (slots <= 0)
                throw new ArgumentException("Slots must be greater than zero", nameof(slots));

            Size = slots;
            mItemSize = Marshal.SizeOf<T>();
            mTotalSize = mItemSize * slots + sizeof(int) * 2 + sizeof(long) * 3; // metadata: writePos, readPos, version, totalWritten, totalRead
            mMmfName = mmfName ?? $"RingBuffer_{Guid.NewGuid():N}";

            mLock = new ReaderWriterLockSlim();
            mSpaceAvailableEvent = new ManualResetEventSlim(true); // Start signaled
            InitializeMemoryMappedFile(createNew);
            LoadMetadata();
        }

        /// <summary>
        /// Creates a ring buffer that connects to an existing memory-mapped file.
        /// </summary>
        /// <param name="mmfName">Name of the existing memory-mapped file.</param>
        public RingMemoryMappedFile(string mmfName)
        {
            if (string.IsNullOrEmpty(mmfName))
                throw new ArgumentException("Memory-mapped file name cannot be null or empty", nameof(mmfName));

            mMmfName = mmfName;
            mOwnsFile = false;
            mLock = new ReaderWriterLockSlim();
            mSpaceAvailableEvent = new ManualResetEventSlim(true); // Start signaled

            try
            {
                mMmf = MemoryMappedFile.OpenExisting(mmfName);
                mAccessor = mMmf.CreateViewAccessor();

                // Load metadata to determine size
                LoadMetadata();
                mItemSize = Marshal.SizeOf<T>();
                mTotalSize = mItemSize * Size + sizeof(int) * 2 + sizeof(long) * 3;
            }
            catch (FileNotFoundException)
            {
                throw new ArgumentException($"Memory-mapped file '{mmfName}' does not exist",
                    nameof(mmfName));
            }
        }
        #endregion

        #region Memory-Mapped File Management
        private void InitializeMemoryMappedFile(bool createNew)
        {
            try
            {
                if (createNew)
                {
                    mMmf = MemoryMappedFile.CreateNew(mMmfName, mTotalSize);
                    mOwnsFile = true;
                }
                else
                {
                    try
                    {
                        mMmf = MemoryMappedFile.OpenExisting(mMmfName);
                        mOwnsFile = false;
                    }
                    catch (FileNotFoundException)
                    {
                        mMmf = MemoryMappedFile.CreateNew(mMmfName, mTotalSize);
                        mOwnsFile = true;
                    }
                }

                mAccessor = mMmf.CreateViewAccessor();

                if (createNew || mOwnsFile)
                {
                    // Initialize metadata if creating new file
                    mWritePosition = 0;
                    mReadPosition = 0;
                    mVersion = 0;
                    mTotalWritten = 0;
                    mTotalRead = 0;
                    SaveMetadata();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to initialize memory-mapped file: {ex.Message}", ex);
            }
        }

        private void LoadMetadata()
        {
            if (mAccessor == null)
                return;

            long offset = mTotalSize - (sizeof(int) * 2 + sizeof(long) * 3);
            Size = mAccessor.ReadInt32(offset);
            mWritePosition = mAccessor.ReadInt32(offset + sizeof(int));
            mReadPosition = mAccessor.ReadInt32(offset + sizeof(int) * 2);
            mVersion = mAccessor.ReadInt64(offset + sizeof(int) * 2 + sizeof(long) * 0);
            mTotalWritten = mAccessor.ReadInt64(offset + sizeof(int) * 2 + sizeof(long) * 1);
            mTotalRead = mAccessor.ReadInt64(offset + sizeof(int) * 2 + sizeof(long) * 2);
        }

        private void SaveMetadata()
        {
            if (mAccessor == null)
                return;

            long offset = mTotalSize - (sizeof(int) * 2 + sizeof(long) * 3);
            mAccessor.Write(offset, Size);
            mAccessor.Write(offset + sizeof(int), mWritePosition);
            mAccessor.Write(offset + sizeof(int) * 2, mReadPosition);
            mAccessor.Write(offset + sizeof(int) * 2 + sizeof(long) * 0, mVersion);
            mAccessor.Write(offset + sizeof(int) * 2 + sizeof(long) * 1, mTotalWritten);
            mAccessor.Write(offset + sizeof(int) * 2 + sizeof(long) * 2, mTotalRead);
        }

        private int CalculateCount()
        {
            if (mTotalWritten == mTotalRead)
                return 0;

            long diff = mTotalWritten - mTotalRead;
            return (int)Math.Min(diff, Size);
        }

        private T ReadItem(int index)
        {
            if (mAccessor == null)
                throw new ObjectDisposedException(nameof(RingMemoryMappedFile<T>));

            long offset = index * mItemSize;
            return mAccessor.ReadStruct<T>(offset);
        }

        private void WriteItem(int index, T item)
        {
            if (mAccessor == null)
                throw new ObjectDisposedException(nameof(RingMemoryMappedFile<T>));

            long offset = index * mItemSize;
            mAccessor.WriteStruct(offset, item);
        }
        #endregion

        #region FIFO Operations
        /// <summary>
        /// Adds an item to the end of the FIFO buffer. Never blocks - will overwrite oldest data if buffer is full.
        /// </summary>
        /// <param name="item">The item to add to the buffer.</param>
        /// <returns>True if no data was overwritten, false if oldest data was overwritten.</returns>
        public bool Enqueue(T item)
        {
            if (!mLock.TryEnterWriteLock(LockTimeout))
                throw new TimeoutException("Failed to acquire write lock within timeout");

            try
            {
                LoadMetadata();

                bool dataOverwritten = false;

                // Check if we're about to overwrite unread data
                if (mTotalWritten - mTotalRead >= Size)
                {
                    dataOverwritten = true;
                    // Move read position forward to maintain buffer size
                    mReadPosition = (mReadPosition + 1) % Size;
                    mTotalRead++;
                }

                // Write the new item
                WriteItem(mWritePosition, item);
                mWritePosition = (mWritePosition + 1) % Size;
                mTotalWritten++;
                mVersion++;

                SaveMetadata();
                return !dataOverwritten;
            }
            finally
            {
                mLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Adds an item to the end of the FIFO buffer, blocking if the buffer is full until space becomes available.
        /// </summary>
        /// <param name="item">The item to add to the buffer.</param>
        /// <param name="timeout">Optional timeout for the blocking operation. Uses BlockingTimeout if not specified.</param>
        /// <returns>True if the item was successfully added; false if the operation timed out.</returns>
        public bool BlockingEnqueue(T item, TimeSpan? timeout = null)
        {
            var actualTimeout = timeout ?? TimeSpan.FromMilliseconds(BlockingTimeout);
            var startTime = DateTime.UtcNow;

            while (true)
            {
                // Check if we have space
                if (!mLock.TryEnterReadLock(LockTimeout))
                    throw new TimeoutException("Failed to acquire read lock within timeout");

                bool hasSpace;
                try
                {
                    LoadMetadata();
                    hasSpace = (mTotalWritten - mTotalRead) < Size;
                }
                finally
                {
                    mLock.ExitReadLock();
                }

                if (hasSpace)
                {
                    // Try to enqueue without overwriting
                    if (!mLock.TryEnterWriteLock(LockTimeout))
                        throw new TimeoutException("Failed to acquire write lock within timeout");

                    try
                    {
                        LoadMetadata();

                        // Double-check we still have space
                        if ((mTotalWritten - mTotalRead) >= Size)
                        {
                            // Space was taken by another thread, continue waiting
                            mSpaceAvailableEvent.Reset();
                        }
                        else
                        {
                            // We have space, write the item
                            WriteItem(mWritePosition, item);
                            mWritePosition = (mWritePosition + 1) % Size;
                            mTotalWritten++;
                            mVersion++;

                            SaveMetadata();

                            // If buffer is now full, reset the event
                            if ((mTotalWritten - mTotalRead) >= Size)
                            {
                                mSpaceAvailableEvent.Reset();
                            }

                            return true;
                        }
                    }
                    finally
                    {
                        mLock.ExitWriteLock();
                    }
                }

                // Check timeout
                if (actualTimeout != Timeout.InfiniteTimeSpan)
                {
                    var elapsed = DateTime.UtcNow - startTime;
                    if (elapsed >= actualTimeout)
                        return false;

                    var remaining = actualTimeout - elapsed;
                    if (remaining.TotalMilliseconds < 1)
                        return false;

                    // Wait for space to become available
                    mSpaceAvailableEvent.Wait(remaining);
                }
                else
                {
                    // Wait indefinitely
                    mSpaceAvailableEvent.Wait();
                }
            }
        }

        /// <summary>
        /// Attempts to remove and return the oldest item from the FIFO buffer.
        /// </summary>
        /// <param name="item">The dequeued item (output parameter).</param>
        /// <returns>True if an item was successfully dequeued; false if buffer is empty.</returns>
        public bool TryDequeue(out T item)
        {
            item = default(T);

            if (!mLock.TryEnterWriteLock(LockTimeout))
                throw new TimeoutException("Failed to acquire write lock within timeout");

            try
            {
                LoadMetadata();

                if (mTotalRead >= mTotalWritten)
                    return false; // Buffer is empty

                // Check if buffer was full before this dequeue
                bool wasFullBefore = (mTotalWritten - mTotalRead) >= Size;

                // Read the oldest item
                item = ReadItem(mReadPosition);
                mReadPosition = (mReadPosition + 1) % Size;
                mTotalRead++;
                mVersion++;

                SaveMetadata();

                // If buffer was full and now has space, signal waiting writers
                if (wasFullBefore)
                {
                    mSpaceAvailableEvent.Set();
                }

                return true;
            }
            finally
            {
                mLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Attempts to return the oldest item from the FIFO buffer without removing it.
        /// </summary>
        /// <param name="item">The peeked item (output parameter).</param>
        /// <returns>True if an item was successfully peeked; false if buffer is empty.</returns>
        public bool TryPeek(out T item)
        {
            item = default(T);

            if (!mLock.TryEnterReadLock(LockTimeout))
                throw new TimeoutException("Failed to acquire read lock within timeout");

            try
            {
                LoadMetadata();

                if (mTotalRead >= mTotalWritten)
                    return false; // Buffer is empty

                // Read the oldest item without removing it
                item = ReadItem(mReadPosition);
                return true;
            }
            finally
            {
                mLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Clears the entire buffer.
        /// </summary>
        public void Clear()
        {
            if (!mLock.TryEnterWriteLock(LockTimeout))
                throw new TimeoutException("Failed to acquire write lock within timeout");

            try
            {
                mWritePosition = 0;
                mReadPosition = 0;
                mTotalWritten = 0;
                mTotalRead = 0;
                mVersion++;

                SaveMetadata();

                // Signal that space is available
                mSpaceAvailableEvent.Set();
            }
            finally
            {
                mLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Gets all items currently in the buffer in FIFO order (oldest first).
        /// </summary>
        /// <returns>Array of items in FIFO order.</returns>
        public T[] ToArray()
        {
            if (!mLock.TryEnterReadLock(LockTimeout))
                throw new TimeoutException("Failed to acquire read lock within timeout");

            try
            {
                LoadMetadata();
                int count = CalculateCount();

                if (count == 0)
                    return new T[0];

                T[] result = new T[count];
                int readPos = mReadPosition;

                for (int i = 0; i < count; i++)
                {
                    result[i] = ReadItem(readPos);
                    readPos = (readPos + 1) % Size;
                }

                return result;
            }
            finally
            {
                mLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets an enumerator over the current items in the buffer in FIFO order.
        /// </summary>
        /// <returns>An enumerator over the current items in FIFO order.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            T[] snapshot = ToArray();
            long version = mVersion;

            foreach (T item in snapshot)
            {
                if (version != mVersion)
                    throw new InvalidOperationException("Collection changed during enumeration");
                yield return item;
            }
        }
        #endregion

        #region Legacy Methods
        /// <summary>
        /// Legacy indexer - gets item by relative position (0 = oldest, Count-1 = newest)
        /// </summary>
        public T this[int relPos]
        {
            get
            {
                if (!mLock.TryEnterReadLock(LockTimeout))
                    throw new TimeoutException("Failed to acquire read lock within timeout");

                try
                {
                    LoadMetadata();
                    int count = CalculateCount();

                    if (count == 0 || relPos < 0 || relPos >= count)
                        throw new IndexOutOfRangeException();

                    int absoluteIndex = (mReadPosition + relPos) % Size;
                    return ReadItem(absoluteIndex);
                }
                finally
                {
                    mLock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Gets the newest item in the buffer
        /// </summary>
        public T Last()
        {
            if (!mLock.TryEnterReadLock(LockTimeout))
                throw new TimeoutException("Failed to acquire read lock within timeout");

            try
            {
                LoadMetadata();
                int count = CalculateCount();

                if (count == 0)
                    throw new InvalidOperationException("Buffer is empty");

                int newestIndex = (mWritePosition - 1 + Size) % Size;
                return ReadItem(newestIndex);
            }
            finally
            {
                mLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets the oldest item in the buffer
        /// </summary>
        public T First()
        {
            if (TryPeek(out T item))
                return item;
            throw new InvalidOperationException("Buffer is empty");
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!mDisposed)
            {
                if (disposing)
                {
                    if (mLock.TryEnterWriteLock(1000))
                    {
                        try
                        {
                            SaveMetadata();
                        }
                        finally
                        {
                            mLock.ExitWriteLock();
                        }
                    }

                    // Dispose objects
                    mLock?.Dispose();
                    mAccessor?.Dispose();
                    mMmf?.Dispose();
                    mSpaceAvailableEvent?.Dispose();
                }
                mDisposed = true;
            }
        }

        ~RingMemoryMappedFile()
        {
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// Extension methods for MemoryMappedViewAccessor to support reading and writing structs
    /// </summary>
    public static class MemoryMappedViewAccessorExtensions
    {
        /// <summary>
        /// Reads a struct of type T from the specified position in the memory-mapped file
        /// </summary>
        /// <typeparam name="T">The struct type to read</typeparam>
        /// <param name="accessor">The MemoryMappedViewAccessor instance</param>
        /// <param name="position">The position to read from</param>
        /// <returns>The struct read from memory</returns>
        public static T ReadStruct<T>(this MemoryMappedViewAccessor accessor, long position) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            byte[] buffer = new byte[size];

            // Read bytes from memory-mapped file
            accessor.ReadArray(position, buffer, 0, size);

            // Convert bytes to struct
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                return Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }
        }

        /// <summary>
        /// Writes a struct of type T to the specified position in the memory-mapped file
        /// </summary>
        /// <typeparam name="T">The struct type to write</typeparam>
        /// <param name="accessor">The MemoryMappedViewAccessor instance</param>
        /// <param name="position">The position to write to</param>
        /// <param name="structure">The struct to write</param>
        public static void WriteStruct<T>(this MemoryMappedViewAccessor accessor, long position, T structure) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            byte[] buffer = new byte[size];

            // Convert struct to bytes
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr(structure, handle.AddrOfPinnedObject(), false);
            }
            finally
            {
                handle.Free();
            }

            // Write bytes to memory-mapped file
            accessor.WriteArray(position, buffer, 0, size);
        }
    }
}
// end of file
