using System;

namespace Tapako.Utilities.WiringTool.View
{
    public class Wiring<T> : Tuple<T, T>
    {
        public Wiring(T item1, T item2) : base(item1, item2)
        {
        }

        public static Wiring<T> Create(T item1, T item2)
        {
            return new Wiring<T>(item1, item2);
        }

        /// <summary>
        /// Fills give variables with the data in this tuple
        /// </summary>
        /// <param name="first">Will be filled with first data</param>
        /// <param name="second">Will be filled with second data</param>
        public void GetItems(out T first, out T second)
        {
            first = Item1;
            second = Item2;

            //if (firstVisual == null ||
            //    secondVisual == null)
            //{
            //    first = default(T);
            //    second = default(T);
            //}
            //else
            //{
            //    first = firstVisual.DataContext;
            //    second = secondVisual.DataContext;
            //}
        }
    }
}