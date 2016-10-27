using System;
using System.ComponentModel.Composition;
using System.Linq.Expressions;

namespace Tapako.View.Adapter
{
    /// <summary>
    /// Ein Adapter um geschlossene Klassen Exportieren zu können und so per 
    /// DependencyInjection zuzuweisen.
    /// Quelle: http://blog.ploeh.dk/2011/03/14/ResolvingclosedtypeswithMEF/
    /// </summary>
    public class MefAdapter<T> where T : new()
    {
        private readonly T export;

        public MefAdapter()
        {
            export = new T();
        }

        [Export]
        public virtual T Export
        {
            get { return export; }
        }
    }

    public class MefAdapter<T1, T2, TResult>
    {
        private readonly static Func<T1, T2, TResult> createExport =
            FuncFactory.Create<T1, T2, TResult>();
        private readonly TResult export;

        [ImportingConstructor]
        public MefAdapter(T1 arg1, T2 arg2)
        {
            export = createExport(arg1, arg2);
        }

        [Export]
        public virtual TResult Export
        {
            get { return export; }
        }
    }

    public class FuncFactory
    {
        internal static Func<T1, T2, TResult> Create<T1, T2, TResult>()
        {
            var arg1Exp =
                Expression.Parameter(typeof (T1), "arg1");
            var arg2Exp =
                Expression.Parameter(typeof (T2), "arg2");

            var ctorInfo =
                typeof (TResult).GetConstructor(new[]
                {
                    typeof (T1),
                    typeof (T2)
                });
            var ctorExp =
                Expression.New(ctorInfo, arg1Exp, arg2Exp);

            return Expression.Lambda<Func<T1, T2, TResult>>(
                ctorExp, arg1Exp, arg2Exp).Compile();
        }
    }
}
