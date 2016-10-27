using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Akomi.Logger;
using ExtensionMethodsCollection;

namespace Tapako.Framework
{
    /// <summary>
    /// Eine Ansammlung von nützlichen Funktionen
    /// </summary>
    public static class FunctionCollection
    {
        /// <summary>
        /// Diese Methode gibt Property Infos zu einer beliebigen Property zurück
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyLambda"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(
            TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof (TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

        /// <summary>
        /// Handelt die vom Thread ausgelösten Exceptions und teilt diese mit
        /// 
        /// How To use:
        ///    ServerThread = new Task(...);
        ///    ServerThread.ContinueWith(ExceptionHandler, TaskContinuationOptions.OnlyOnFaulted);
        ///    ServerThread.Start();
        /// </summary>
        /// <param name="task"></param>
        public static void TaskExceptionThrower(Task task)
        {
            Logger.Error(task.Exception.ToString(true));
            if (task.Exception != null) throw task.Exception;
        }        
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">The input path</param>
        /// <param name="replaceWith">illegal characters will be replaced with this string</param>
        /// <returns></returns>
        public static string ReplaceIllegalPathCharacters(string path, string replaceWith)
        {
            if (!string.IsNullOrEmpty(path))
            {
                return Constants.ValidDriverCharactersRegex.Replace(path, replaceWith);
            }
            return string.Empty;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }
    }
}