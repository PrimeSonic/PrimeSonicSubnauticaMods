using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomCraftSML.Serialization.EasyMarkup
{
    public class DoubleQueue<T> : LinkedList<T>
    {
        public T PeekStart() => base.First.Value;
        public T PeekEnd() => base.Last.Value;

        public T RemoveFromStart()
        {
            T value = base.First.Value;

            base.RemoveFirst();

            return value;
        }

        public T RemoveFromEnd()
        {
            T value = base.Last.Value;

            base.RemoveLast();

            return value;
        }

        public bool RemoveFromStart(T value)
        {
            if (base.First.Value.Equals(value))
            {
                base.RemoveFirst();
                return true;
            }

            return false;
        }

        public bool RemoveFromEnd(T value)
        {
            if (base.Last.Value.Equals(value))
            {
                base.RemoveLast();
                return true;
            }

            return false;
        }

        public void AddFromStart(T value) => base.AddFirst(value);
        public void AddFromEnd(T value) => base.AddLast(value);
    }
}
