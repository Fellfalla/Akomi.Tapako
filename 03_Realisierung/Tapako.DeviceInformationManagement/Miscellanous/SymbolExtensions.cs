﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Tapako.DeviceInformationManagement.Miscellanous
{
    /// <summary>
    /// Source: http://blog.functionalfun.net/2009/10/getting-methodinfo-of-generic-method.html
    /// <para/> <para/>
    /// Example 1:<para/>
    ///   var methodInfo = SymbolExtensions.GetMethodInfo&lt;TestClass&gt;(c => c.AMethod());
    /// <para/> <para/>
    /// Example 2:<para/>
    ///   var methodInfo = SymbolExtensions.GetMethodInfo&lt;TestClass&gt;(c => c.AGenericMethod(default(int)));
    /// <para/> <para/>
    /// Example 3:<para/>
    ///   var methodInfo = SymbolExtensions.GetMethodInfo(() => StaticTestClass.StaticTestMethod());
    /// 
    /// </summary>
    public static class SymbolExtensions
    {
        /// <summary>
        /// Given a lambda expression that calls a method, returns the method info.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo(Expression<Action> expression)
        {
            return GetMethodInfo((LambdaExpression)expression);
        }

        /// <summary>
            /// Given a lambda expression that calls a method, returns the method info.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="expression">The expression.</param>
            /// <returns></returns>
        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
        {
            return GetMethodInfo((LambdaExpression)expression);
        }

        /// <summary>
        /// Given a lambda expression that calls a method, returns the method info.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult">The type of the function result</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            return GetMethodInfo((LambdaExpression)expression);
        }

        /// <summary>
            /// Given a lambda expression that calls a method, returns the method info.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns></returns>
        public static MethodInfo GetMethodInfo(LambdaExpression expression)
        {
            MethodCallExpression outermostExpression = expression.Body as MethodCallExpression;

            if (outermostExpression == null)
            {
                throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
            }

            return outermostExpression.Method;
        }
    }
}