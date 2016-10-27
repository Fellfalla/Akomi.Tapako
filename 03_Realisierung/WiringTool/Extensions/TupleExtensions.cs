using System;

namespace Tapako.Utilities.WiringTool.Extensions
{
    public static class TupleExtensions
    {
        public static Tuple<TOut1, TOut2> Cast<TOut1,TOut2>(this Tuple<object, object> tuple)
        {
            return Tuple.Create((TOut1) tuple.Item1, (TOut2) tuple.Item2);
        }

        public static bool Contains<T1, T2, T3>(this Tuple<T1, T2> tuple, T3 item) 
        {
            return tuple.Item1.Equals(item) || tuple.Item2.Equals(item);
        }

        public static bool Contains<T1, T2, T3, T4>(this Tuple<T1, T2> tuple, T3 item1, T4 item2) 
        {
            return tuple.Contains(item1) && tuple.Contains(item2);
        }
    }
}
