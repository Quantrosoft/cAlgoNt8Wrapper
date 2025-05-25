using System;
using System.Collections.Generic;

namespace TdsCommons
{
    /// <summary>
    /// Represents a fixted length ring buffer to store a specified maximal count of items within.
    /// </summary>
    /// <typeparam name="T">The generic type of the items stored within the ring buffer.</typeparam>
    //template<typename T> //MQ4_enable//
    public class Ringbuffer<T>
    {
        #region Private variables
        /// <summary>
        /// The all-over position within the ring buffer. The position 
        /// increases continously by adding new items to the buffer. This 
        /// value is needed to calculate the current relative position within the 
        /// buffer.
        /// </summary>
        protected int mPosition;

        /// <summary>
        /// The current version of the buffer, this is required for a correct 
        /// exception handling while enumerating over the items of the buffer.
        /// </summary>
        private long mVersion;

        /// <summary>
        /// The public buffer
        /// </summary>
        protected T[] mBuffer;
        #endregion

        #region Public variables
        /// <summary>
        /// Gets the maximal count of items within the ring buffer.
        /// </summary>
        public int Size;

        /// <summary>
        /// Get the current count of items within the ring buffer.
        /// </summary>
        public int Count;

        /// <summary>
        /// Counts the number of added period
        /// </summary>
        public int AddCount;

        /// <summary>
        /// Count == Size
        /// </summary>
        public bool IsBufferValid => Count == Size;

        /// <summary>
        /// AddCount > Size happend at least once
        /// </summary>
        public bool IsFalloutValid;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of a <see cref="Ringbuffer<T>"/> with a specified cache size.
        /// </summary>
        /// <param name="Size">The maximal count of items to be stored within the ring buffer.</param>
        public Ringbuffer(int _slots)
        {
            // set Size and init the cache
            Size = _slots;
            Array.Resize(ref mBuffer, _slots);
        }
        #endregion

        #region Castings for assignments
        // Unfortunately only the get direction ist possible to omit the [0]
        // double alice = RingbufferVar; does work and is the same as var alice = RingbufferVar[0];
        // RingbufferVar = 1234.0; does not work. Instead RingbufferVar[0] = 1234.0 must be used
        public static implicit operator double(Ringbuffer<T> d)
        {
            return (double)(d.mBuffer[(d.mPosition + d.Size - 1) % d.Size] as double?);
        }

        public static implicit operator int(Ringbuffer<T> d)
        {
            return (int)(d.mBuffer[(d.mPosition + d.Size - 1) % d.Size] as int?);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets or sets an item for a specified position within the ring buffer.
        /// </summary>
        /// <param name="index">The position to get or set an item.</param>
        /// <returns>The fond item at the specified position within the ring buffer.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public T this[int relPos]
        {
            get
            {
                if (Count < 1)
                    throw new IndexOutOfRangeException("Must add a ring buffer slot after initialization");
                // validate the index
                if (relPos < 0 || relPos >= Size)
                    throw new IndexOutOfRangeException();
                // calculate the relative position within the rolling base array
                return mBuffer[(mPosition + Size - relPos - 1) % Size];
            }

            //set { Insert(relPos, value); }
            set
            {
                if (Count < 1)
                    throw new IndexOutOfRangeException("Must add a ring buffer slot after initialization");
                mBuffer[(mPosition + Size - relPos - 1) % Size] = value;
            }
        }

        /// <summary>
        /// Gets the abs pos from the rel pos.
        /// </summary>
        /// <param name="relPos">The rel pos.</param>
        /// <returns></returns>
        public int GetAbsPos(int relPos)
        {
            return (mPosition + Size - Count + relPos) % Size;
        }

        /// <summary>
        /// Adds a new item to the buffer.
        /// </summary>
        /// <param name="item">The item to be added to the buffer.</param>
        public void Add(T item)
        {
            Add(item, out _, out _);
        }

        /// <summary>
        /// Adds a new item to the buffer.
        /// </summary>
        /// <param name="item">The item to be added to the buffer.</param>
        public void Add(T item, out int ndx)
        {
            Add(item, out ndx, out _);
        }

        /// <summary>
        /// Adds a new item to the buffer.
        /// </summary>
        /// <param name="item">The item to be added to the buffer.</param>
        public virtual void Add(T item, out int ndx, out T fallOut)
        {
            // return the absolute falling out position
            ndx = mPosition;

            // return the falling out value
            fallOut = mBuffer[mPosition];

            // add a new item to the current relative position within the buffer and increase the position
            mBuffer[mPosition] = item;

            // increase position
            mPosition = ++mPosition % Size;

            // increase the count if Size is not yet reached
            if (Count < Size)
                Count++;

            // buffer changed; next version
            mVersion++;

            // count this slot
            AddCount++;

            // Validate the fallout
            if (!IsFalloutValid)
                if (AddCount > Size)
                    IsFalloutValid = true;
        }

        public void Bump()
        {
            Bump(out _, out _);
        }

        public void Bump(out int ndx)
        {
            Bump(out ndx, out _);
        }

        public void Bump(out int ndx, out T fallOut)
        {
            // return the absolute falling out position
            ndx = mPosition;

            // return the falling out value
            fallOut = mBuffer[mPosition];

            // increase position
            mPosition = ++mPosition % Size;

            // increase the count if Size is not yet reached
            if (Count < Size)
                Count++;

            // buffer changed; next version
            mVersion++;

            // count this slot
            AddCount++;
        }

        /// <summary>
        /// Exchange current value
        /// </summary>
        public T Swap(T item)
        {
            var retVal = this[0];
            this[0] = item;
            return retVal;
        }

        /// <summary>
        /// Clears the whole buffer and releases all referenced objects currently stored within the buffer.
        /// </summary>
        public void Clear()
        {
            //MQL_replaceString//"default(T)|NULL"
            for (int i = 0; i < Count; i++)
                mBuffer[i] = default(T);
            mPosition = 0;
            Count = 0;
            mVersion++;
        }

        /// <summary>
        /// Determines if a specified item is currently present within the buffer.
        /// </summary>
        /// <param name="item">The item to search for within the current buffer.</param>
        /// <returns>True if the specified item is currently present within the buffer; otherwise false.</returns>
        public bool Contains(T item)
        {
            int index = IndexOf(item);
            return index != -1;
        }

        /// <summary>
        /// Copies the current items within the buffer to a specified array.
        /// </summary>
        /// <param name="array">The target array to copy the items of the buffer to.</param>
        /// <param name="arrayIndex">The start position witihn the target
        /// array to start copying.</param>
        public void CopyTo(T[] array, int arrayIndex, int cnt)
        {
            for (int i = 0; i < cnt; i++)
            {
                array[i + arrayIndex] = mBuffer[(mPosition - Count + i) % Size];
            }
        }

        /// <summary>
        /// Gets an enumerator over the current items within the buffer.
        /// </summary>
        /// <returns>An enumerator over the current items within the buffer.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            long version = mVersion;
            for (int i = 0; i < Count; i++)
            {
                if (version != mVersion)
                    throw new InvalidOperationException("Collection changed");
                yield return this[i];
            }
        }

        /// <summary>
        /// Gets the position of a specied item within the ring buffer.
        /// </summary>
        /// <param name="item">The item to get the current position for.</param>
        /// <returns>The zero based index of the found item within the 
        /// buffer. If the item was not present within the buffer, this
        /// method returns -1.</returns>
        public int IndexOf(T item)
        {
            // loop over the current count of items
            for (int i = 0; i < Count; i++)
            {
                // get the item at the relative position within the public array
                T item2 = mBuffer[(mPosition - Count + i) % Size];
                // if both items are null, return true
                if (null == item && null == item2)
                    return i;
                // if equal return the position
                if (item != null && item.Equals(item2))
                    return i;
            }
            // nothing found
            return -1;
        }

        /// <summary>
        /// Inserts an item at a specified position into the buffer.
        /// </summary>
        /// <param name="index">The position within the buffer to add 
        /// the new item.</param>
        /// <param name="item">The new item to be added to the buffer.</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <remarks>
        /// If the specified index is equal to the current count of items
        /// within the buffer, the specified item will be added.
        /// 
        /// <b>Warning</b>
        /// Frequent usage of this method might become a bad idea if you are 
        /// working with a large buffer Size. The insertion of an item
        /// at a specified position within the buffer causes all present 
        /// items below the specified position to be moved one position.
        /// </remarks>
        public void Insert(int index, T item)
        {
            // validate index
            if (index < 0 || index > Count)
                throw new IndexOutOfRangeException();
            // add if index equals to count
            if (index == Count)
            {
                Add(item);
                return;
            }

            // get the maximal count of items to be moved
            int count = Math.Min(Count, Size - 1) - index;
            // get the relative position of the new item within the buffer
            int index2 = (mPosition - Count + index) % Size;

            // move all items below the specified position
            for (int i = index2 + count; i > index2; i--)
            {
                int to = i % Size;
                int from = (i - 1) % Size;
                mBuffer[to] = mBuffer[from];
            }

            // set the new item
            mBuffer[index2] = item;

            // adjust storage information
            if (Count < Size)
            {
                Count++;
                mPosition++;
            }
            // buffer changed; next version
            mVersion++;
        }

        /// <summary>
        /// Removes a specified item from the current buffer.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        /// <returns>True if the specified item was successfully removed
        /// from the buffer; otherwise false.</returns>
        /// <remarks>
        /// <b>Warning</b>
        /// Frequent usage of this method might become a bad idea if you are 
        /// working with a large buffer Size. The removing of an item 
        /// requires a scan of the buffer to get the position of the specified
        /// item. If the item was found, the deletion requires a move of all 
        /// items stored abouve the found position.
        /// </remarks>
        public bool Remove(T item)
        {
            // find the position of the specified item
            int index = IndexOf(item);
            // item was not found; return false
            if (index == -1)
                return false;
            // remove the item at the specified position
            RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Removes an item at a specified position within the buffer.
        /// </summary>
        /// <param name="index">The position of the item to be removed.</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <remarks>
        /// <b>Warning</b>
        /// Frequent usage of this method might become a bad idea if you are 
        /// working with a large buffer Size. The deletion requires a move 
        /// of all items stored abouve the found position.
        /// </remarks>
        public void RemoveAt(int index)
        {
            // validate the index
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();
            // move all items above the specified position one step
            // closer to zeri
            for (int i = index; i < Count - 1; i++)
            {
                // get the next relative target position of the item
                int to = (mPosition - Count + i) % Size;
                // get the next relative source position of the item
                int from = (mPosition - Count + i + 1) % Size;
                // move the item
                mBuffer[to] = mBuffer[from];
            }
            // get the relative position of the last item, which becomes empty
            // after deletion and set the item as empty
            int last = (mPosition - 1) % Size;
            mBuffer[last] = default(T);
            // adjust storage information
            mPosition--;
            Count--;
            // buffer changed; next version
            mVersion++;
        }

        public T Lowest(int cnt, out int lowestNdx, int startNdx = 0)
        {
            lowestNdx = Size - 1;
            int pos = mPosition + Size - startNdx;
            T retVal = mBuffer[(pos - cnt--) % Size];
            for (; cnt > 0; cnt--)
            {
                T cmpVal = mBuffer[(pos - cnt) % Size];
                if (((IComparable)(cmpVal)).CompareTo(retVal) < 0)
                {
                    lowestNdx = cnt;
                    retVal = cmpVal;
                }
            }
            return retVal;
        }

        public T Lowest(out int lowestNdx)
        {
            lowestNdx = 0;
            return Lowest(Count, out lowestNdx);
        }

        public T Lowest()
        {
            return Lowest(Count, out _);
        }

        public T Highest(int cnt, out int highestNdx, int startNdx = 0)
        {
            highestNdx = Size - 1;
            int pos = mPosition + Size - startNdx;
            T retVal = mBuffer[(pos - cnt--) % Size];
            for (; cnt > 0; cnt--)
            {
                T cmpVal = mBuffer[(pos - cnt) % Size];
                if (((IComparable)(cmpVal)).CompareTo(retVal) > 0)
                {
                    highestNdx = cnt;
                    retVal = cmpVal;
                }
            }
            return retVal;
        }

        public T Highest(out int highestNdx)
        {
            highestNdx = 0;
            return Highest(Count, out highestNdx);
        }

        public T Highest()
        {
            return Highest(Count, out _);
        }

        public T First()
        {
            return this[Count - 1];
        }

        public T Last(int ndx = 0)
        {
            return this[ndx];
        }
        #endregion
    }
}
// end of file
