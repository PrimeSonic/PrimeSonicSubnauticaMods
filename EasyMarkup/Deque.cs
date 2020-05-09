namespace EasyMarkup
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;

    /// <inheritdoc cref="ICollection{T}" />    
    /// <summary>Represents a double-ended queue collection of objects.</summary>
    /// <typeparam name="T">Specifies the type of elements in the deque.</typeparam>
    /// <remarks>https://en.wikipedia.org/wiki/Double-ended_queue</remarks>
    [Serializable]
    internal class Deque<T> : ICollection, IEnumerable<T> // Replace IEnumerable with IReadOnlyCollection on .NET 4.x
    {
        private const int MinimumGrow = 4;
        private const long GrowFactor = 200;
        private const int DefaultCapacity = 4;
        private readonly EqualityComparer<T> _equalityComparer = EqualityComparer<T>.Default;

        [NonSerialized] private object _syncRoot;

        /// <summary>
        /// This array is the underlying data structure that holds all the elements in the <see cref="Deque{T}" />.
        /// </summary>
        private T[] _array;

        /// <summary>
        /// The index to the first element in the <see cref="Deque{T}" />.
        /// Because the <see cref="Deque{T}" /> is implemented as a circular array, the head index may be at any position,
        /// even after the tail index.
        /// </summary>
        private int _head;

        /// <summary>
        /// The index to the last element in the <see cref="Deque{T}" />.
        /// Because the <see cref="Deque{T}" /> is implemented as a circular array, the tail index may be at any position,
        /// even before the head index.
        /// </summary>
        private int _tail;

        /// <summary>
        /// Increments whenever the internal array of the <see cref="Deque{T}" /> is changed in any way.
        /// This is necessary for proper error checking during enumeration.
        /// </summary>
        private int _version;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="Deque{T}" /> class that is empty and has the default initial
        /// capacity.
        /// </summary>
        public Deque()
            : this(DefaultCapacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Deque{T}" /> class that is empty and has the specified initial
        /// capacity.
        /// </summary>
        /// <param name="capacity">The specified initial capacity.</param>
        public Deque(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity is less than zero.");
            }

            _array = new T[capacity];
            _head = 0;
            _tail = 0;
            this.Count = 0;
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="Deque{T}" /> class and populates it with the
        /// specified collection.
        /// </summary>
        /// <param name="collection">The collection to be added to this <see cref="Deque{T}" />.</param>
        public Deque(IEnumerable<T> collection)
            : this(DefaultCapacity)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection), "Provided collection is null");
            }

            foreach (T obj in collection)
            {
                PushTail(obj);
            }
        }

        /// <summary>
        /// Gets or sets the value of an element in the collection by relative index.
        /// Index 0 always refers to the first element in the collection.
        /// Incrementing the index always moves through the collection from head to tail.
        /// </summary>
        /// <param name="index">The relative index of the element.</param>
        /// <exception cref="ArgumentOutOfRangeException">Requested index falls outside the array.</exception>
        /// <returns>The element at the specified index.</returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Requested index falls outside the array");
                }

                int i = RelativeZeroBasedIndex(index);
                return _array[i];
            }
            set
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Requested index falls outside the array");
                }

                int i = RelativeZeroBasedIndex(index);
                _array[i] = value;
                ++_version;
            }
        }

        /// <summary>
        /// Gets the current capacity of the <see cref="Deque{T}" />.
        /// This can be optionally set at the constructor and will automatically resize as more elements are added to the
        /// collection.
        /// </summary>
        public int Capacity => _array.Length;

        /// <summary>
        /// Gets the total number of elements from the start of the head index.
        /// </summary>
        private int CountBeforeWrap => this.IsWrapped ? _array.Length - _head : this.Count;

        /// <summary>
        /// Gets the total number of elements from the start of the internal array.
        /// </summary>
        private int CountAfterWrap => this.IsWrapped ? _tail + 1 : this.Count;

        /// <summary>
        /// If the head index is greater than the tail index, it means we have wrapped around the end of the internal array.
        /// </summary>
        private bool IsWrapped => _head > _tail;

        /// <summary>
        /// Returns the next index, moving forwards, of the circular array.
        /// Used whenever the elements are added from the end or removed from the start.
        /// </summary>
        /// <param name="currentIndex">The starting index.</param>
        /// <returns>The next index in the array that is 1 increment ahead.</returns>
        private int NextIndexForwards(int currentIndex)
        {
            return (currentIndex + 1) % _array.Length;
        }

        /// <summary>
        /// Returns the next index, moving backwards, of the circular array.
        /// Used whenever the elements are added from the start or removed from the end.
        /// </summary>
        /// <param name="currentIndex">The starting index.</param>
        /// <returns>The next index in the array that is 1 decrement behind.</returns>
        private int NextIndexBackwards(int currentIndex)
        {
            return (currentIndex + _array.Length - 1) % _array.Length;
        }

        /// <summary>
        /// Returns an index that maps the start of the collection to zero.
        /// </summary>
        /// <param name="zeroBasedIndex">A zero based index, where zero refers to the first element of the collection.</param>
        /// <returns>The actual index of the internal array that maps to the zero based index.</returns>
        private int RelativeZeroBasedIndex(int zeroBasedIndex)
        {
            return (_head + zeroBasedIndex) % _array.Length;
        }

        /// <inheritdoc cref="IEnumerable{T}" />
        /// <summary>Gets the number of elements contained in the <see cref="Deque{T}" />.</summary>
        /// <returns>The number of elements contained in the <see cref="Deque{T}" />.</returns>
        public int Count { get; private set; }

        /// <inheritdoc />
        /// <summary>This collection type is not intended for multi-threaded use.</summary>
        public bool IsSynchronized => false;

        /// <inheritdoc />
        /// <summary>This collection type is not intended for multi-threaded use.</summary>
        public object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
                }

                return _syncRoot;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Copies the <see cref="Deque{T}" /> elements to an existing one-dimensional
        /// <see cref="Array" />, starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the elements copied from <see cref="Deque{T}" />.
        /// The <see cref="Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero or greater than the target array size.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source <see cref="Deque{T}" /> is greater than the
        /// available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.
        /// </exception>
        public void CopyTo(Array array, int index)
        {
            ArrayCopy(array, index);
        }

        /// <inheritdoc />
        /// <summary>
        /// Enumeration is always done in order from head to tail.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Copies the <see cref="Deque{T}" /> elements to an existing one-dimensional <see cref="Array" />, starting at
        /// the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the elements copied from
        /// <see cref="Deque{T}" />.
        /// The <see cref="Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array" /> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source <see cref="Deque{T}" /> is greater than the
        /// available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.
        /// </exception>
        public void CopyTo(T[] array, int index)
        {
            ArrayCopy(array, index);
        }

        /// <summary>
        /// Removes all objects from the <see cref="Deque{T}" />.
        /// </summary>
        public void Clear()
        {
            if (this.IsWrapped)
            {
                Array.Clear(_array, _head, this.CountBeforeWrap);
                Array.Clear(_array, 0, _tail);
            }
            else
            {
                Array.Clear(_array, _head, this.Count);
            }

            _head = 0;
            _tail = 0;
            this.Count = 0;
            ++_version;
        }

        /// <summary>
        /// Adds an object to the beginning of the <see cref="Deque{T}" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="Deque{T}" />. The value can be null for reference types.</param>
        public void PushHead(T item)
        {
            HandleArrayCapacity();

            _head = NextIndexBackwards(_head);

            if (this.Count == 0)
            {
                _tail = _head; // Tail and Head have the same index when there is only 1 element.
            }

            _array[_head] = item;

            ++this.Count;
            ++_version;
        }

        /// <summary>Adds an object to the end of the <see cref="Deque{T}" />.</summary>
        /// <param name="item">The object to add to the <see cref="Deque{T}" />. The value can be null for reference types.</param>
        public void PushTail(T item)
        {
            HandleArrayCapacity();

            _tail = NextIndexForwards(_tail);

            if (this.Count == 0)
            {
                _head = _tail; // Head and Tail have the same index when there is only 1 element.
            }

            _array[_tail] = item;

            ++this.Count;
            ++_version;
        }

        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Deque{T}" />.
        /// </summary>
        /// <returns>
        /// The object that is removed from the beginning of the <see cref="Deque{T}" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">The <see cref="Deque{T}" /> is empty.</exception>
        public T PopHead()
        {
            if (this.Count == 0)
            {
                throw new InvalidOperationException("The DeQueue is empty");
            }

            T obj = _array[_head];
            _array[_head] = default;

            _head = NextIndexForwards(_head);

            --this.Count;
            ++_version;
            return obj;
        }

        /// <summary>
        /// Removes and returns the object at the end of the <see cref="Deque{T}" />.
        /// </summary>
        /// <returns>
        /// The object that is removed from the end of the <see cref="Deque{T}" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">The <see cref="Deque{T}" /> is empty.</exception>
        public T PopTail()
        {
            if (this.Count == 0)
            {
                throw new InvalidOperationException("The DeQueue is empty");
            }

            T obj = _array[_tail];
            _array[_tail] = default;

            _tail = NextIndexBackwards(_tail);

            --this.Count;
            ++_version;
            return obj;
        }

        /// <summary>
        /// Returns the object at the beginning of the <see cref="Deque{T}" /> without removing it.
        /// </summary>
        /// <returns>
        /// The object at the beginning of the <see cref="Deque{T}" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">The <see cref="Deque{T}" /> is empty.</exception>
        public T PeekHead()
        {
            if (this.Count == 0)
            {
                throw new InvalidOperationException("The DeQueue is empty");
            }

            return _array[_head];
        }

        /// <summary>
        /// Returns the object at the end of the <see cref="Deque{T}" /> without removing it.
        /// </summary>
        /// <returns>
        /// The object at the end of the <see cref="Deque{T}" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">The <see cref="Deque{T}" /> is empty.</exception>
        public T PeekTail()
        {
            if (this.Count == 0)
            {
                throw new InvalidOperationException("The DeQueue is empty");
            }

            return _array[_tail];
        }

        /// <summary>Determines whether an element is in the <see cref="Deque{T}" />.</summary>
        /// <param name="item">The object to locate in the <see cref="Deque{T}" />. The value can be null for reference types.</param>
        /// <returns><c>True</c> if <paramref name="item" /> is found in the <see cref="Deque{T}" />; otherwise, <c>false</c>.</returns>
        public bool Contains(T item)
        {
            int index = _head;
            int size = this.Count;
            while (size-- > 0)
            {
                if (item == null)
                {
                    if (_array[index] == null)
                    {
                        return true;
                    }
                }
                else if (_array[index] != null && _equalityComparer.Equals(_array[index], item))
                {
                    return true;
                }

                index = NextIndexForwards(index);
            }

            return false;
        }

        /// <summary>Copies the <see cref="Deque{T}" /> elements to a new array.</summary>
        /// <returns>A new array containing elements copied from the <see cref="Deque{T}" />.</returns>
        public T[] ToArray()
        {
            var objArray = new T[this.Count];
            if (this.Count == 0)
            {
                return objArray;
            }

            if (this.IsWrapped)
            {
                Array.Copy(_array, _head, objArray, 0, this.CountBeforeWrap);
                Array.Copy(_array, 0, objArray, this.CountBeforeWrap, this.CountAfterWrap);
            }
            else
            {
                Array.Copy(_array, _head, objArray, 0, this.Count);
            }

            return objArray;
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="Deque{T}" />.
        /// but only if that number is less than 90 percent of current capacity.
        /// </summary>
        public void TrimExcess()
        {
            if (this.Count >= (int)(_array.Length * 0.9))
            {
                return;
            }

            SetCapacity(this.Count);
        }

        /// <summary>
        /// Copies the elements of the <see cref="Deque{T}" /> into a compatible array.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="targetArrayStartingIndex">The index at which to start copying.</param>
        private void ArrayCopy(Array array, int targetArrayStartingIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array), "Target array is null");
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException("Multi-Dimentional arrays not supported");
            }

            int targetArrayLength = array.Length;
            if (targetArrayStartingIndex < 0 || targetArrayStartingIndex > targetArrayLength)
            {
                throw new ArgumentOutOfRangeException(
                                                      nameof(targetArrayStartingIndex),
                                                      targetArrayStartingIndex,
                                                      "Starting index is less than zero or greater than the target array size");
            }

            int availableSpaceInTarget = targetArrayLength - targetArrayStartingIndex;

            if (availableSpaceInTarget < this.Count)
            {
                throw new ArgumentException(
                                            "The number of elements in the source deque is greater than the available space " +
                                            "from starting index to the end of the destination array.");
            }

            try
            {
                Array.Copy(_array, _head, array, targetArrayStartingIndex, this.CountBeforeWrap);

                if (this.IsWrapped)
                {
                    Array.Copy(_array, 0, array, targetArrayStartingIndex + this.CountBeforeWrap, this.CountAfterWrap);
                }
            }
            catch (ArrayTypeMismatchException ex)
            {
                throw new ArgumentException("Mismatched array types", ex);
            }
        }

        /// <summary>
        /// Checks if the internal array needs to be resized and defines the next capacity to be used.
        /// </summary>
        private void HandleArrayCapacity()
        {
            if (this.Count < this.Capacity)
            {
                return;
            }

            int capacity = (int)(_array.Length * GrowFactor / 100L);
            if (capacity < _array.Length + MinimumGrow)
            {
                capacity = _array.Length + MinimumGrow;
            }

            SetCapacity(capacity);
        }

        /// <summary> 
        /// Creates a new internal array with the specified capacity, copies the elements of the <see cref="Deque{T}" /> to it,
        /// and then replaces the previous internal array with this new one.
        /// </summary>
        /// <param name="capacity"></param>
        private void SetCapacity(int capacity)
        {
            var objArray = new T[capacity];

            if (this.Count > 0)
            {
                if (this.IsWrapped)
                {
                    Array.Copy(_array, _head, objArray, 0, this.CountBeforeWrap);
                    Array.Copy(_array, 0, objArray, this.CountBeforeWrap, this.CountAfterWrap);
                }
                else
                {
                    Array.Copy(_array, _head, objArray, 0, this.Count);
                }
            }

            _array = objArray;
            _head = 0;
            _tail = this.Count - 1;
            ++_version;
        }

        /// <inheritdoc />
        /// <summary>
        /// Enumeration is always done in order from head to tail.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <inheritdoc cref="IEnumerator{T}" />
        /// <summary>Enumerates the elements of a <see cref="Deque{T}" /> in order from head to tail.</summary>
        [Serializable]
        public struct Enumerator : IEnumerator<T>
        {
            private readonly Deque<T> _q;
            private int _index;
            private readonly int _version;
            private T _currentElement;

            internal Enumerator(Deque<T> q)
            {
                _q = q;
                _version = _q._version;
                _index = -1;
                _currentElement = default;
            }

            /// <inheritdoc />
            /// <summary>Releases all resources used by the <see cref="Deque{T}.Enumerator" />.</summary>
            public void Dispose()
            {
                _index = -2;
                _currentElement = default;
            }

            /// <inheritdoc />
            /// <summary>Advances the enumerator to the next element of the <see cref="Deque{T}" />.</summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the
            /// end of the collection.
            /// </returns>
            /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
            public bool MoveNext()
            {
                if (_version != _q._version)
                {
                    throw new InvalidOperationException("The collection was modified after the enumerator was created.");
                }

                if (_index == -2)
                {
                    return false;
                }

                ++_index;
                if (_index == _q.Count)
                {
                    _index = -2;
                    _currentElement = default;
                    return false;
                }

                _currentElement = _q[_index];
                return true;
            }

            /// <inheritdoc />
            /// <summary>Gets the element at the current position of the enumerator.</summary>
            /// <returns>The element in the <see cref="Deque{T}" /> at the current position of the enumerator.</returns>
            /// <exception cref="InvalidOperationException">
            /// The enumerator is positioned before the first element of the collection or after the last element.
            /// </exception>
            public T Current
            {
                get
                {
                    if (_index < 0)
                    {
                        if (_index == -1)
                        {
                            throw new InvalidOperationException("Enumerator is not initialized");
                        }

                        throw new InvalidOperationException(
                                                            "The enumerator is positioned before the first element of the collection " +
                                                            "or after the last element.");
                    }

                    return _currentElement;
                }
            }

            object IEnumerator.Current => this.Current;

            public void Reset()
            {
                if (_version != _q._version)
                {
                    throw new InvalidOperationException("Enumerator versioning failed");
                }

                _index = -1;
                _currentElement = default;
            }
        }
    }
}
