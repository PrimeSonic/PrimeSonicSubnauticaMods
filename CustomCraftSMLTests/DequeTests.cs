namespace CustomCraftSMLTests
{
    using System;
    using System.Collections;
    using EasyMarkup;
    using NUnit.Framework;

    [TestFixture]
    public class DequeTests
    {
        private const int MinValue = 1;
        private const int DataSetSize = 48;
        private const int MaxValue = MinValue + DataSetSize;

        [Test]
        public void Supports_Stack_Behavior_FromHead()
        {
            var dq = new Deque<byte>();

            dq.PushHead(1);
            dq.PushHead(2);
            dq.PushHead(3);

            Assert.AreEqual(3, dq.PeekHead());
            Assert.AreEqual(3, dq.PopHead());

            Assert.AreEqual(2, dq.PeekHead());
            Assert.AreEqual(2, dq.PopHead());

            Assert.AreEqual(1, dq.PeekHead());
            Assert.AreEqual(1, dq.PopHead());
        }

        [Test]
        public void Supports_Stack_Behavior_FromTail()
        {
            var dq = new Deque<byte>();

            dq.PushTail(1);
            dq.PushTail(2);
            dq.PushTail(3);

            Assert.AreEqual(3, dq.PeekTail());
            Assert.AreEqual(3, dq.PopTail());

            Assert.AreEqual(2, dq.PeekTail());
            Assert.AreEqual(2, dq.PopTail());

            Assert.AreEqual(1, dq.PeekTail());
            Assert.AreEqual(1, dq.PopTail());
        }

        [Test]
        public void Supports_Queue_Behavior_Forwards()
        {
            var dq = new Deque<byte>();

            dq.PushTail(1);
            dq.PushTail(2);
            dq.PushTail(3);

            Assert.AreEqual(1, dq.PeekHead());
            Assert.AreEqual(1, dq.PopHead());

            Assert.AreEqual(2, dq.PeekHead());
            Assert.AreEqual(2, dq.PopHead());

            Assert.AreEqual(3, dq.PeekHead());
            Assert.AreEqual(3, dq.PopHead());
        }

        [Test]
        public void Supports_Queue_Behavior_Backwards()
        {
            var dq = new Deque<byte>();

            dq.PushHead(1);
            dq.PushHead(2);
            dq.PushHead(3);

            Assert.AreEqual(1, dq.PeekTail());
            Assert.AreEqual(1, dq.PopTail());

            Assert.AreEqual(2, dq.PeekTail());
            Assert.AreEqual(2, dq.PopTail());

            Assert.AreEqual(3, dq.PeekTail());
            Assert.AreEqual(3, dq.PopTail());
        }

        [Test]
        public void PushInOrder_ReadInOrder_NoResizing()
        {
            var dq = new Deque<int>(DataSetSize);

            int expectedCount = 0;

            for (int i = MinValue; i < MaxValue; i++)
            {
                Assert.AreEqual(expectedCount++, dq.Count);

                dq.PushTail(i);
                Assert.AreEqual(i, dq.PeekTail());

                Assert.AreEqual(expectedCount, dq.Count);
            }

            for (int i = MinValue; i < MaxValue; i++)
            {
                Assert.AreEqual(expectedCount--, dq.Count);

                Assert.IsTrue(dq.Contains(i));
                Assert.AreEqual(i, dq.PeekHead());
                Assert.IsTrue(dq.Contains(i));
                Assert.AreEqual(i, dq.PopHead());
                Assert.IsFalse(dq.Contains(i));

                Assert.AreEqual(expectedCount, dq.Count);

            }
        }

        [Test]
        public void PushInOrder_ReadInOrder_WithResizing()
        {
            var dq = new Deque<int>();

            int expectedCount = 0;

            for (int i = MinValue; i < MaxValue; i++)
            {
                Assert.AreEqual(expectedCount++, dq.Count);

                dq.PushTail(i);
                Assert.AreEqual(i, dq.PeekTail());

                Assert.AreEqual(expectedCount, dq.Count);
            }

            for (int i = MinValue; i < MaxValue; i++)
            {
                Assert.AreEqual(expectedCount--, dq.Count);

                Assert.IsTrue(dq.Contains(i));
                Assert.AreEqual(i, dq.PeekHead());
                Assert.IsTrue(dq.Contains(i));
                Assert.AreEqual(i, dq.PopHead());
                Assert.IsFalse(dq.Contains(i));

                Assert.AreEqual(expectedCount, dq.Count);
            }
        }

        [Test]
        public void PushInOrder_ReadInReverseOrder_NoResizing()
        {
            var dq = new Deque<int>(DataSetSize);

            int expectedCount = 0;

            for (int i = MinValue; i < MaxValue; i++)
            {
                Assert.AreEqual(expectedCount++, dq.Count);

                dq.PushTail(i);
                Assert.AreEqual(i, dq.PeekTail());

                Assert.AreEqual(expectedCount, dq.Count);
            }

            for (int i = MaxValue - 1; i >= MinValue; i--)
            {
                Assert.AreEqual(expectedCount--, dq.Count);

                Assert.AreEqual(i, dq.PeekTail());
                Assert.AreEqual(i, dq.PopTail());

                Assert.AreEqual(expectedCount, dq.Count);
            }
        }

        [Test]
        public void PushInOrder_ReadInReverseOrder_WithResizing()
        {
            var dq = new Deque<int>();

            int expectedCount = 0;

            for (int i = MinValue; i < MaxValue; i++)
            {
                Assert.AreEqual(expectedCount++, dq.Count);

                dq.PushTail(i);
                Assert.AreEqual(i, dq.PeekTail());

                Assert.AreEqual(expectedCount, dq.Count);
            }

            for (int i = MaxValue - 1; i >= MinValue; i--)
            {
                Assert.AreEqual(expectedCount--, dq.Count);

                Assert.AreEqual(i, dq.PeekTail());
                Assert.AreEqual(i, dq.PopTail());

                Assert.AreEqual(expectedCount, dq.Count);
            }
        }

        [Test]
        public void PushInReverseOrder_ReadInOrder_NoResizing()
        {
            var dq = new Deque<int>(DataSetSize);

            int expectedCount = 0;

            for (int i = MinValue; i < MaxValue; i++)
            {
                Assert.AreEqual(expectedCount++, dq.Count);

                dq.PushHead(i);
                Assert.AreEqual(i, dq.PeekHead());

                Assert.AreEqual(expectedCount, dq.Count);
            }

            for (int i = MaxValue - 1; i >= MinValue; i--)
            {
                Assert.AreEqual(expectedCount--, dq.Count);

                Assert.AreEqual(i, dq.PeekHead());
                Assert.AreEqual(i, dq.PopHead());

                Assert.AreEqual(expectedCount, dq.Count);
            }
        }

        [Test]
        public void PushInReverseOrder_ReadInOrder_WithResizing()
        {
            var dq = new Deque<int>();

            int expectedCount = 0;

            for (int i = MinValue; i < MaxValue; i++)
            {
                Assert.AreEqual(expectedCount++, dq.Count);

                dq.PushHead(i);
                Assert.AreEqual(i, dq.PeekHead());

                Assert.AreEqual(expectedCount, dq.Count);
            }

            for (int i = MaxValue - 1; i >= MinValue; i--)
            {
                Assert.AreEqual(expectedCount--, dq.Count);

                Assert.AreEqual(i, dq.PeekHead());
                Assert.AreEqual(i, dq.PopHead());

                Assert.AreEqual(expectedCount, dq.Count);
            }
        }

        [Test]
        public void WithSequence_IsEnumerable_NoResizing()
        {
            var dq = new Deque<int>(DataSetSize);
            Assert.AreEqual(DataSetSize, dq.Capacity);

            int expectedCount = 0;

            for (int i = MinValue; i < MaxValue; i++)
            {
                Assert.AreEqual(expectedCount++, dq.Count);

                dq.PushTail(i);
                Assert.AreEqual(i, dq.PeekTail());

                Assert.AreEqual(expectedCount, dq.Count);
            }

            Assert.AreEqual(DataSetSize, dq.Capacity);

            int expectedValue = MinValue;
            foreach (int i in dq)
            {
                Assert.AreEqual(expectedValue++, i);
            }

            expectedValue = MinValue;
            for (int i = 0; i < DataSetSize; i++)
            {
                Assert.AreEqual(expectedValue, dq[i]);

                expectedValue++;
            }
        }

        [Test]
        public void WithSequence_IsEnumerable_WithResizing()
        {
            var dq = new Deque<int>();

            int expectedCount = 0;

            for (int i = MinValue; i < MaxValue; i++)
            {
                Assert.AreEqual(expectedCount++, dq.Count);

                dq.PushTail(i);
                Assert.AreEqual(i, dq.PeekTail());

                Assert.AreEqual(expectedCount, dq.Count);
            }

            int expectedValue = MinValue;
            foreach (int i in dq)
            {
                Assert.AreEqual(expectedValue++, i);
            }

            expectedValue = MinValue;
            for (int i = 0; i < DataSetSize; i++)
            {
                Assert.AreEqual(expectedValue, dq[i]);

                expectedValue++;
            }
        }

        [Test]
        public void WithSequence2_IsEnumerable_WithResizing()
        {
            var dq = new Deque<int>();
            Assert.AreEqual(4, dq.Capacity);

            int expectedCount = 0;

            for (int i = MaxValue - 1; i >= MinValue; i--)
            {
                Assert.AreEqual(expectedCount++, dq.Count);

                dq.PushHead(i);
                Assert.AreEqual(i, dq.PeekHead());

                Assert.AreEqual(expectedCount, dq.Count);

                Assert.IsTrue(dq.Capacity >= dq.Count);
            }

            int expectedValue = MinValue;
            foreach (int i in dq)
            {
                Assert.AreEqual(expectedValue++, i);
            }

            expectedValue = MinValue;
            for (int i = 0; i < DataSetSize; i++)
            {
                Assert.AreEqual(expectedValue, dq[i]);

                expectedValue++;
            }
        }

        [TestCase(DataSetSize)]
        [TestCase(DataSetSize / 2)]
        [TestCase(DataSetSize * 2)]
        [TestCase(3)]
        public void WithSequence_ArrayCopying_WhenCorrectlySized_AreEquivalent(int dataSetSize)
        {
            var dq = new Deque<int>();
            Assert.AreEqual(4, dq.Capacity);

            for (int i = 1; i <= dataSetSize; i++)
            {
                dq.PushTail(i);
            }

            Assert.IsTrue(dq.Capacity >= dq.Count);

            int[] ar1 = new int[dataSetSize];
            Array ar2 = new int[dataSetSize];
            int[] ar3 = dq.ToArray();

            dq.CopyTo(ar1, 0);
            dq.CopyTo(ar2, 0);


            for (int i = 0; i < dq.Count; i++)
            {
                Assert.AreEqual(dq[i], ar1[i]);
                Assert.AreEqual(dq[i], ar2.GetValue(i));
                Assert.AreEqual(dq[i], ar3[i]);
            }
        }

        [Test]
        public void TrimExcess_CapacityMuchLargerThanCount_Trims()
        {
            var dq = new Deque<int>(new[] { 1, 2, 3, 4, 5 });
            Assert.AreNotEqual(dq.Count, dq.Capacity);

            dq.TrimExcess();
            Assert.AreEqual(dq.Count, dq.Capacity);

            dq.TrimExcess();
            Assert.AreEqual(dq.Count, dq.Capacity);
        }

        [Test]
        public void TrimExcess_CapacityNotThatMuchLarger_Trims()
        {
            var dq = new Deque<int>(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 });
            Assert.AreNotEqual(dq.Count, dq.Capacity);

            dq.TrimExcess();
            Assert.AreNotEqual(dq.Count, dq.Capacity);
        }

        [Test]
        public void OverwriteByIndex()
        {
            var dq = new Deque<int>(new[] { 1, 2, 3, 4, 5 });

            for (int i = 0; i < dq.Count; i++)
            {
                dq[i] = 7;
            }

            foreach (int i in dq)
            {
                Assert.AreEqual(7, i);
            }
        }

        [Test]
        public void Clear_BecomesEmpty()
        {
            var dq = new Deque<int>(new[] { 1, 2, 3, 4, 5 });

            Assert.AreNotEqual(0, dq.Count);

            int originalCapacity = dq.Capacity;

            dq.Clear();

            Assert.AreEqual(0, dq.Count);
            Assert.AreEqual(originalCapacity, dq.Capacity);
        }

        [Test]
        public void Clear_WrappedArray_BecomesEmpty()
        {
            var dq = new Deque<int>(4);
            dq.PushHead(4);
            dq.PushTail(4);
            dq.PushHead(4);
            dq.PushTail(4);

            Assert.AreNotEqual(0, dq.Count);

            int originalCapacity = dq.Capacity;

            dq.Clear();

            Assert.AreEqual(0, dq.Count);
            Assert.AreEqual(originalCapacity, dq.Capacity);
        }

        [Test]
        public void Resizing_MinimumGrowHandled()
        {
            var dq = new Deque<int>(2);
            Assert.AreEqual(2, dq.Capacity);

            dq.PushHead(1);
            dq.PushHead(1);
            dq.PushHead(1);

            Assert.AreEqual(6, dq.Capacity);
        }

        [Test]
        public void Contains_WhenRefType_NullAcceptable()
        {
            var dq = new Deque<object>(4);
            dq.PushTail(1);
            dq.PushTail('C');
            dq.PushTail(null);
            dq.PushTail("text");

            Assert.IsTrue(dq.Contains(1));
            Assert.IsTrue(dq.Contains('C'));
            Assert.IsTrue(dq.Contains(null));
            Assert.IsTrue(dq.Contains("text"));
        }

        [Test]
        public void ToArray_SequencedCopiedInCorrectOrder()
        {
            var dq = new Deque<int>(7);

            dq.PushTail(3);
            dq.PushTail(4);
            dq.PushTail(5);

            dq.PushHead(2);
            dq.PushHead(1);
            dq.PushHead(0);

            int[] array = dq.ToArray();

            Assert.AreEqual(dq.Count, array.Length);

            for (int i = 0; i < 6; i++)
            {
                Assert.AreEqual(i, array[i]);
                Assert.AreEqual(i, dq[i]);
            }
        }

        [Test]
        public void ToArray_WithNoElements_ReturnSizeZeroArray()
        {
            var dq = new Deque<int>();
            Assert.AreEqual(0, dq.Count);

            int[] array = dq.ToArray();

            Assert.AreEqual(0, array.Length);
        }

        [Test]
        public void CopyTo_CompatibleDifferentTypes_DataConvertible()
        {
            var deque = new Deque<char>(6);
            deque.PushTail('A');
            deque.PushTail('B');
            deque.PushTail('C');
            deque.PushTail('D');
            deque.PushTail('E');
            deque.PushTail('F');
            Array array = new int[deque.Count];

            deque.CopyTo(array, 0);

            System.Collections.Generic.IEnumerator<char> dqMover = deque.GetEnumerator();
            IEnumerator arMover = array.GetEnumerator();

            dqMover.MoveNext();
            arMover.MoveNext();

            do
            {
                Assert.AreEqual(dqMover.Current, Convert.ToChar(arMover.Current));

            } while (dqMover.MoveNext() && arMover.MoveNext());

            dqMover.Dispose();
        }

        [Test]
        public void DequeEnumerator_Reset_BackToStart()
        {
            var deque = new Deque<char>();
            deque.PushTail('A');
            deque.PushTail('B');
            deque.PushTail('C');

            System.Collections.Generic.IEnumerator<char> dqMover = deque.GetEnumerator();
            dqMover.MoveNext();
            Assert.AreEqual('A', dqMover.Current);
            dqMover.MoveNext();
            Assert.AreEqual('B', dqMover.Current);

            dqMover.Reset();
            dqMover.MoveNext();
            Assert.AreEqual('A', dqMover.Current);

            dqMover.Dispose();
        }

        [Test]
        public void ThreadingProtection()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var deque = new Deque<char>();

            Assert.IsFalse(deque.IsSynchronized);
            Assert.IsNotNull(deque.SyncRoot);
        }

        #region Exceptions

        [Test]
        public void Deque_BadCapacity_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                // ReSharper disable once UnusedVariable
                var deque = new Deque<char>(capacity: -1);
            });
        }

        [Test]
        public void Deque_NullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                // ReSharper disable once UnusedVariable
                var deque = new Deque<char>(collection: null);
            });
        }

        [TestCase(2)]
        [TestCase(-1)]
        public void Indexer_Get_OutOfRange_Throws(int badIndex)
        {
            var deque = new Deque<char>(new[] { 'A', 'B' });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                // ReSharper disable once UnusedVariable
                char c = deque[badIndex];

            });
        }

        [TestCase(2)]
        [TestCase(-1)]
        public void Indexer_Set_OutOfRange_Throws(int badIndex)
        {
            // ReSharper disable once CollectionNeverQueried.Local
            var deque = new Deque<char>(new[] { 'A', 'B' });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                // ReSharper disable once UnusedVariable
                deque[badIndex] = 'C';

            });
        }

        [Test]
        public void CopyTo_NullTargetArray_Throws()
        {
            var deque = new Deque<char>(new[] { 'A', 'B' });

            Assert.Throws<ArgumentNullException>(() =>
            {
                deque.CopyTo(null, 0);
            });
        }

        [TestCase(0)]
        [TestCase(1)]
        public void CopyTo_TargetArrayTooSmall_Throws(int startingIndex)
        {
            var deque = new Deque<char>(new[] { 'A', 'B', 'C' });
            char[] smallArray = new char[1];

            Assert.Throws<ArgumentException>(() =>
            {
                deque.CopyTo(smallArray, startingIndex);
            });
        }

        [Test]
        public void CopyTo_IndexBeyondTargetArray_Throws()
        {
            var deque = new Deque<char>(new[] { 'A', 'B' });
            char[] smallArray = new char[2];

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                deque.CopyTo(smallArray, 3);
            });
        }

        [Test]
        public void CopyTo_ArrayTypesIncompatible_Throws()
        {
            var deque = new Deque<char>(new[] { 'A', 'B' });
            Array wrongArray = new string[2];

            Assert.Throws<ArgumentException>(() =>
            {
                deque.CopyTo(wrongArray, 0);
            });
        }

        [Test]
        public void CopyTo_TargetArrayIsMultiDimentional_Throws()
        {
            var deque = new Deque<char>(new[] { 'A', 'B' });
            Array wrongArray = new char[2, 2];

            Assert.Throws<ArgumentException>(() =>
            {
                deque.CopyTo(wrongArray, 0);
            });
        }

        [Test]
        public void PopHead_WhenEmpty_Throws()
        {
            var deque = new Deque<char>();

            Assert.Throws<InvalidOperationException>(() =>
            {
                deque.PopHead();
            });
        }

        [Test]
        public void PopTail_WhenEmpty_Throws()
        {
            var deque = new Deque<char>();

            Assert.Throws<InvalidOperationException>(() =>
            {
                deque.PopTail();
            });
        }

        [Test]
        public void PeekHead_WhenEmpty_Throws()
        {
            var deque = new Deque<char>();

            Assert.Throws<InvalidOperationException>(() =>
            {
                deque.PeekHead();
            });
        }

        [Test]
        public void PeekTail_WhenEmpty_Throws()
        {
            var deque = new Deque<char>();

            Assert.Throws<InvalidOperationException>(() =>
            {
                deque.PeekTail();
            });
        }

        [Test]
        public void DequeEnumerator_Get_WhenNotInitialized_Throws()
        {
            var deque = new Deque<char>(6);
            deque.PushTail('A');
            deque.PushTail('B');
            deque.PushTail('C');

            System.Collections.Generic.IEnumerator<char> dqMover = deque.GetEnumerator();

            Assert.Throws<InvalidOperationException>(() =>
            {
                // ReSharper disable once UnusedVariable
                char c = dqMover.Current;
            });

            dqMover.Dispose();
        }

        [Test]
        public void DequeEnumerator_MoveNext_BetweenChanges_Throws()
        {
            var deque = new Deque<char>(6);
            deque.PushTail('A');
            deque.PushTail('B');
            deque.PushTail('C');

            System.Collections.Generic.IEnumerator<char> dqMover = deque.GetEnumerator();
            dqMover.MoveNext();

            Assert.AreEqual('A', dqMover.Current);

            deque.PopTail();

            Assert.Throws<InvalidOperationException>(() =>
            {
                dqMover.MoveNext();
            });

            dqMover.Dispose();
        }

        [Test]
        public void DequeEnumerator_MovedOutOfRange_Throws()
        {
            var deque = new Deque<char>(6);
            deque.PushTail('A');
            deque.PushTail('B');

            System.Collections.Generic.IEnumerator<char> dqMover = deque.GetEnumerator();
            Assert.IsTrue(dqMover.MoveNext());
            Assert.IsTrue(dqMover.MoveNext());
            Assert.IsFalse(dqMover.MoveNext());
            Assert.IsFalse(dqMover.MoveNext());

            Assert.Throws<InvalidOperationException>(() =>
            {
                // ReSharper disable once UnusedVariable
                char c = dqMover.Current;
            });

            dqMover.Dispose();
        }

        [Test]
        public void DequeEnumerator_Reset_BetweenChanges_Throws()
        {
            var deque = new Deque<char>(6);
            deque.PushTail('A');
            deque.PushTail('B');
            deque.PushTail('C');

            System.Collections.Generic.IEnumerator<char> dqMover = deque.GetEnumerator();
            dqMover.MoveNext();

            Assert.AreEqual('A', dqMover.Current);

            deque[1] = 'X';

            Assert.Throws<InvalidOperationException>(() =>
            {
                dqMover.Reset();
            });

            dqMover.Dispose();
        }

        #endregion
    }
}