using System.Collections.Generic;

namespace Tapako.View.ExtensionMethods
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Source: http://stackoverflow.com/questions/4166493/drop-the-last-item-with-linq/4166561#4166561
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> WithoutLast<T>(this IEnumerable<T> source)
        {
            using (var e = source.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    for (var value = e.Current; e.MoveNext(); value = e.Current)
                    {
                        yield return value;
                    }
                }
            }
        }
    }
}
