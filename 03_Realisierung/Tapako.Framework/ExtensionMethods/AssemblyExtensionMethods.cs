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
        public static IEnumerable<Type> GetChildClasses<TTargetClass>(this Assembly assembly)
        {
            Type[] loadableTypes = assembly.GetLoadableTypes().ToArray();
            foreach (var type in loadableTypes)
            {

                // searches for desired child classes in given assembly
                // --> A different Assembly version will break this "IsAssignableFrom"-Correlation
                var isDerivatedType = typeof(TTargetClass).IsAssignableFrom(type);
                var isNotAbstract = !type.IsAbstract;
                var isClass = type.IsClass;
                var hasDefaultConstructor = type.HasEmptyOrDefaultConstructor();

                if (isDerivatedType &&
                    isNotAbstract &&
                    isClass &&
                    hasDefaultConstructor)
                {
                    yield return type;
                }

            }
        }
    }
}
