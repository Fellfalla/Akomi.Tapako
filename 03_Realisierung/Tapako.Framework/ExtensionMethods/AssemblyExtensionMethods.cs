using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtensionMethodsCollection;

namespace Tapako.Framework.ExtensionMethods
{
    /// <summary>
    /// Extension Methods for the Assembly-Class
    /// </summary>
    public static class AssemblyExtensionMethods
    {
        /// <summary>
        /// Source: http://stackoverflow.com/questions/7889228/how-to-prevent-reflectiontypeloadexception-when-calling-assembly-gettypes
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        /// <summary>
        /// Gets all <see cref="Type"/>s which are potentially instanziable as <typeparamref name="TTargetClass"/>
        /// </summary>
        /// <typeparam name="TTargetClass"></typeparam>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetPotentialTargetTypes<TTargetClass>(this Assembly assembly)
        {
            Type[] types = assembly.GetLoadableTypes().ToArray();
            return types.Where(
                assemblyType =>
                    typeof(TTargetClass).IsAssignableFrom(assemblyType) && !assemblyType.IsInterface &&
                    assemblyType.GetParameterlessConstructor() != null);
        }
    }
}
