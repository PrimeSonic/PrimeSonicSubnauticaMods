namespace Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;    
    using System.Runtime.InteropServices;
    using System.Threading;

    /// <summary>Represents a double-ended queue collection of objects.</summary>
    /// <typeparam name="T">Specifies the type of elements in the dequeue.</typeparam>
    [ComVisible(false)]
    [Serializable]
    public class DeQueue<T> : IEnumerable<T>, IEnumerable, ICollection
    {
        private static readonly T[] _emptyArray = new T[0];
        private T[] _array;
        private int _head;
        private int _tail;
        private int _size;
        private int _version;
        [NonSerialized]
        private object _syncRoot;
        private const int MinimumGrow = 4;
        private const long GrowFactor = 200;
        private const int DefaultCapacity = 4;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index > this._array.Length)
                    throw new ArgumentOutOfRangeException(nameof(index), "ArgumentOutOfRange_Index");

                int i = (this._head + index) % this._array.Length;
                return _array[i];
            }
            set
            {
                if (index < 0 || index > this._array.Length)
                    throw new ArgumentOutOfRangeException(nameof(index), "ArgumentOutOfRange_Index");

                int i = (this._head + index) % this._array.Length;
                _array[i] = value;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:ScratchProjects.Queue`1" /> class that is empty and has the default initial capacity.</summary>
        public DeQueue()
        {
            this._array = _emptyArray;
        }

        /// <summary>Initializes a new instance of the <see cref="T:ScratchProjects.Queue`1" /> class that is empty and has the specified initial capacity.</summary>
        /// <param name="capacity">The initial number of elements that the <see cref="T:ScratchProjects.Queue`1" /> can contain.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.</exception>
        public DeQueue(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "ArgumentOutOfRange_NeedNonNegNumRequired");
            this._array = new T[capacity];
            this._head = 0;
            this._tail = 0;
            this._size = 0;
        }

        /// <summary>Initializes a new instance of the <see cref="T:ScratchProjects.Queue`1" /> class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.</summary>
        /// <param name="collection">The collection whose elements are copied to the new <see cref="T:ScratchProjects.Queue`1" />.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="collection" /> is null.</exception>
        public DeQueue(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            this._array = new T[DefaultCapacity];
            this._size = 0;
            this._version = 0;
            foreach (T obj in collection)
                this.PushTail(obj);
        }

        /// <summary>Gets the number of elements contained in the <see cref="T:ScratchProjects.Queue`1" />.</summary>
        /// <returns>The number of elements contained in the <see cref="T:ScratchProjects.Queue`1" />.</returns>
        public int Count => this._size;

        bool ICollection.IsSynchronized => false;
        
        object ICollection.SyncRoot
        {
            get
            {
                if (this._syncRoot == null)
                    Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), null);
                return this._syncRoot;
            }
        }

        /// <summary>Removes all objects from the <see cref="T:ScratchProjects.Queue`1" />.</summary>
        public void Clear()
        {
            if (this._head < this._tail)
            {
                Array.Clear(this._array, this._head, this._size);
            }
            else
            {
                Array.Clear(this._array, this._head, this._array.Length - this._head);
                Array.Clear(this._array, 0, this._tail);
            }
            this._head = 0;
            this._tail = 0;
            this._size = 0;
            ++this._version;
        }

        /// <summary>Copies the <see cref="T:ScratchProjects.Queue`1" /> elements to an existing one-dimensional <see cref="T:System.Array" />, starting at the specified array index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:ScratchProjects.Queue`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than zero.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:ScratchProjects.Queue`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "ArgumentOutOfRange_Index");
            int length1 = array.Length;
            if (length1 - arrayIndex < this._size)
                throw new ArgumentException("Argument_InvalidOffLen");
            int num = length1 - arrayIndex < this._size ? length1 - arrayIndex : this._size;
            if (num == 0)
                return;
            int length2 = this._array.Length - this._head < num ? this._array.Length - this._head : num;
            Array.Copy(this._array, this._head, array, arrayIndex, length2);
            int length3 = num - length2;
            if (length3 <= 0)
                return;
            Array.Copy(this._array, 0, array, arrayIndex + this._array.Length - this._head, length3);
        }
        
        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (array.Rank != 1)
                throw new ArgumentException("Arg_RankMultiDimNotSupported");
            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException("Arg_NonZeroLowerBound");
            int length1 = array.Length;
            if (index < 0 || index > length1)
                throw new ArgumentOutOfRangeException(nameof(index), "ArgumentOutOfRange_Index");
            if (length1 - index < this._size)
                throw new ArgumentException("Argument_InvalidOffLen");
            int num = length1 - index < this._size ? length1 - index : this._size;
            if (num == 0)
                return;
            try
            {
                int length2 = this._array.Length - this._head < num ? this._array.Length - this._head : num;
                Array.Copy(this._array, this._head, array, index, length2);
                int length3 = num - length2;
                if (length3 <= 0)
                    return;
                Array.Copy(this._array, 0, array, index + this._array.Length - this._head, length3);
            }
            catch (ArrayTypeMismatchException ex)
            {
                throw new ArgumentException("Argument_InvalidArrayType", ex);
            }
        }

        /// <summary>Adds an object to the end of the <see cref="T:ScratchProjects.Queue`1" />.</summary>
        /// <param name="item">The object to add to the <see cref="T:ScratchProjects.Queue`1" />. The value can be null for reference types.</param>
        public void PushHead(T item)
        {
            if (this._size == this._array.Length)
            {
                int capacity = (int)(this._array.Length * GrowFactor / 100L);
                if (capacity < this._array.Length + MinimumGrow)
                    capacity = this._array.Length + MinimumGrow;
                this.SetCapacity(capacity);
            }
            this._head = (this._head + this._array.Length - 1) % this._array.Length;
            this._array[this._head] = item;
            ++this._size;
            ++this._version;
        }

        public void PushTail(T item)
        {
            if (this._size == this._array.Length)
            {
                int capacity = (int)(this._array.Length * GrowFactor / 100L);
                if (capacity < this._array.Length + MinimumGrow)
                    capacity = this._array.Length + MinimumGrow;
                this.SetCapacity(capacity);
            }
            this._array[this._tail] = item;
            this._tail = (this._tail + 1) % this._array.Length;
            ++this._size;
            ++this._version;
        }

        /// <summary>Returns an enumerator that iterates through the <see cref="T:ScratchProjects.Queue`1" />.</summary>
        /// <returns>An <see cref="T:ScratchProjects.Queue`1.Enumerator" /> for the <see cref="T:ScratchProjects.Queue`1" />.</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }
        
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>Removes and returns the object at the beginning of the <see cref="T:ScratchProjects.Queue`1" />.</summary>
        /// <returns>The object that is removed from the beginning of the <see cref="T:ScratchProjects.Queue`1" />.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:ScratchProjects.Queue`1" /> is empty.</exception>
        public T PopHead()
        {
            if (this._size == 0)
                throw new InvalidOperationException("InvalidOperation_EmptyQueue");
            T obj = this._array[this._head];
            this._array[this._head] = default(T);
            this._head = (this._head + 1) % this._array.Length;
            --this._size;
            ++this._version;
            return obj;
        }

        /// <summary>Returns the object at the beginning of the <see cref="T:ScratchProjects.Queue`1" /> without removing it.</summary>
        /// <returns>The object at the beginning of the <see cref="T:ScratchProjects.Queue`1" />.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:ScratchProjects.Queue`1" /> is empty.</exception>
        public T PeekHead()
        {
            if (this._size == 0)
                throw new InvalidOperationException("InvalidOperation_EmptyQueue");
            return this._array[this._head];
        }

        /// <summary>Removes and returns the object at the end of the <see cref="T:ScratchProjects.Queue`1" />.</summary>
        /// <returns>The object that is removed from the end of the <see cref="T:ScratchProjects.Queue`1" />.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:ScratchProjects.Queue`1" /> is empty.</exception>
        public T PopTail()
        {
            if (this._size == 0)
                throw new InvalidOperationException("InvalidOperation_EmptyQueue");
            T obj = this._array[this._tail - 1];
            this._array[this._tail - 1] = default(T);
            this._tail = (this._tail - 1) % this._array.Length;
            --this._size;
            ++this._version;
            return obj;
        }

        /// <summary>Returns the object at the end of the <see cref="T:ScratchProjects.Queue`1" /> without removing it.</summary>
        /// <returns>The object at the end of the <see cref="T:ScratchProjects.Queue`1" />.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:ScratchProjects.Queue`1" /> is empty.</exception>
        public T PeekTail()
        {
            if (this._size == 0)
                throw new InvalidOperationException("InvalidOperation_EmptyQueue");
            return this._array[this._tail - 1];
        }

        /// <summary>Determines whether an element is in the <see cref="T:ScratchProjects.Queue`1" />.</summary>
        /// <param name="item">The object to locate in the <see cref="T:ScratchProjects.Queue`1" />. The value can be null for reference types.</param>
        /// <returns>true if <paramref name="item" /> is found in the <see cref="T:ScratchProjects.Queue`1" />; otherwise, false.</returns>
        public bool Contains(T item)
        {
            int index = this._head;
            int size = this._size;
            EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
            while (size-- > 0)
            {
                if (item == null)
                {
                    if (this._array[index] == null)
                        return true;
                }
                else if (this._array[index] != null && equalityComparer.Equals(this._array[index], item))
                    return true;
                index = (index + 1) % this._array.Length;
            }
            return false;
        }

        internal T GetElement(int i)
        {
            return this._array[(this._head + i) % this._array.Length];
        }

        /// <summary>Copies the <see cref="T:ScratchProjects.Queue`1" /> elements to a new array.</summary>
        /// <returns>A new array containing elements copied from the <see cref="T:ScratchProjects.Queue`1" />.</returns>
        public T[] ToArray()
        {
            T[] objArray = new T[this._size];
            if (this._size == 0)
                return objArray;
            if (this._head < this._tail)
            {
                Array.Copy(this._array, this._head, objArray, 0, this._size);
            }
            else
            {
                Array.Copy(this._array, this._head, objArray, 0, this._array.Length - this._head);
                Array.Copy(this._array, 0, objArray, this._array.Length - this._head, this._tail);
            }
            return objArray;
        }

        private void SetCapacity(int capacity)
        {
            T[] objArray = new T[capacity];
            if (this._size > 0)
            {
                if (this._head < this._tail)
                {
                    Array.Copy(this._array, this._head, objArray, 0, this._size);
                }
                else
                {
                    Array.Copy(this._array, this._head, objArray, 0, this._array.Length - this._head);
                    Array.Copy(this._array, 0, objArray, this._array.Length - this._head, this._tail);
                }
            }
            this._array = objArray;
            this._head = 0;
            this._tail = this._size == capacity ? 0 : this._size;
            ++this._version;
        }

        /// <summary>Sets the capacity to the actual number of elements in the <see cref="T:ScratchProjects.Queue`1" />, if that number is less than 90 percent of current capacity.</summary>

        public void TrimExcess()
        {
            if (this._size >= (int)(this._array.Length * 0.9))
                return;
            this.SetCapacity(this._size);
        }

        /// <summary>Enumerates the elements of a <see cref="T:ScratchProjects.Queue`1" />.</summary>

        [Serializable]
        public struct Enumerator : IEnumerator<T>
        {
            private DeQueue<T> _q;
            private int _index;
            private int _version;
            private T _currentElement;

            internal Enumerator(DeQueue<T> q)
            {
                this._q = q;
                this._version = this._q._version;
                this._index = -1;
                this._currentElement = default(T);
            }

            /// <inheritdoc />
            /// <summary>Releases all resources used by the <see cref="T:ScratchProjects.Queue`1.Enumerator" />.</summary>
            public void Dispose()
            {
                this._index = -2;
                this._currentElement = default(T);
            }

            /// <summary>Advances the enumerator to the next element of the <see cref="T:ScratchProjects.Queue`1" />.</summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext()
            {
                if (this._version != this._q._version)
                    throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                if (this._index == -2)
                    return false;
                ++this._index;
                if (this._index == this._q._size)
                {
                    this._index = -2;
                    this._currentElement = default(T);
                    return false;
                }
                this._currentElement = this._q.GetElement(this._index);
                return true;
            }

            /// <summary>Gets the element at the current position of the enumerator.</summary>
            /// <returns>The element in the <see cref="T:ScratchProjects.Queue`1" /> at the current position of the enumerator.</returns>
            /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception>
            public T Current
            {
                get
                {
                    if (this._index < 0)
                    {
                        if (this._index == -1)
                            throw new InvalidOperationException("InvalidOperation_EnumNotStarted");
                        throw new InvalidOperationException("InvalidOperation_EnumEnded");
                    }
                    return this._currentElement;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (this._index < 0)
                    {
                        if (this._index == -1)
                            throw new InvalidOperationException("InvalidOperation_EnumNotStarted");
                        throw new InvalidOperationException("InvalidOperation_EnumEnded");
                    }
                    return this._currentElement;
                }
            }

            void IEnumerator.Reset()
            {
                if (this._version != this._q._version)
                    throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
                this._index = -1;
                this._currentElement = default(T);
            }
        }
    }
}
