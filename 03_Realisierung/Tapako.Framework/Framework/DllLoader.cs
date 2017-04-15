using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Akomi.InformationModel.Attributes;
using Akomi.Logger;
using ExtensionMethodsCollection;
using Tapako.Framework.ExtensionMethods;

namespace Tapako.Framework
{
    /// <summary>
    /// Loads Assemblys from dll files and returns the class
    /// </summary>
    public static class DllLoader
    {
        //todo: implementiere das Laden einer Klasse die zu einer übergebenen schnittstelle pass

        /// <summary>
        /// Loads a dll assembly and returns a Class
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Load<T>(string path)
        {
            if (string.IsNullOrEmpty(path)) return default(T);
            Logger.Debug(string.Format("DllLoader: Trying to load the assembly \"{0}\"", path));
            var assembly = LoadAssembly(path);
            Logger.Debug("DllLoader: Loaded {0} from {1}", assembly, path);
            return LoadClass<T>(assembly);
        }

        /// <summary>
        /// Methode zum Instanzieren einer Klasse eines Assemblys anhand des Klassennamens
        /// </summary>
        /// <param name="driverAssembly">Instanz der Assebmly die die gewünschte Klasse beinhaltet</param>
        /// <param name="className">Klassenname der gewünschten Klasse</param>
        /// <returns></returns>
        public static object LoadClass(Assembly driverAssembly, string className)
        {
            Type[] types = driverAssembly.GetTypes();
            object obj = null;
            foreach (Type assClass in types)
            {
                if (assClass.Name == className)
                {
                    obj = Activator.CreateInstance(assClass);
                    if (obj != null)
                        break;
                }
            }
            return obj;
        }

        /// <summary>
        /// Methode zum Instanzieren einer Klasse eines Assemblys ohne Klassenname.
        /// Hier wird die erste Klasse genommen, die gefunden wird
        /// </summary>
        /// <param name="driverAssembly">Instanz der Assebmly die die gewünschte Klasse beinhaltet</param>
        /// <returns></returns>
        public static TTargetClass LoadClass<TTargetClass>(Assembly driverAssembly)
        {
            if (driverAssembly != null)
            {
                
                List<Type> types = driverAssembly.GetChildClasses<TTargetClass>().ToList();
                List<Type> invalidTypes = new List<Type>();

                // At first try to load a marked class
                foreach (Type assemblyType in types.Where(type => type.HasCustomAttributes(new[] { typeof(PreferredPublicClass) })))
                {
                    var result = TryCreateInstance<TTargetClass>(assemblyType, sourceAssembly: driverAssembly);
                    if (result != null)
                    {
                        return result;
                    }
                    invalidTypes.Add(assemblyType);
                }

                // Try to instanziate not preferred classes
                foreach (Type assemblyType in types.Except(invalidTypes))
                {
                    var result = TryCreateInstance<TTargetClass>(assemblyType, sourceAssembly: driverAssembly);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return default(TTargetClass);
        }

        /// <summary>
        /// Tries to create an instance from given <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="TTargetClass"></typeparam>
        /// <param name="type"></param>
        /// <param name="sourceAssembly"></param>
        /// <returns></returns>
        private static TTargetClass TryCreateInstance<TTargetClass>(Type type, Assembly sourceAssembly)
        {
            try
            {
                var obj = (TTargetClass) Activator.CreateInstance(type);
                if (obj != null)
                    return obj;
            }
            catch (Exception exception)
            {
                Logger.Error("DllLoader: Failed to create {0} in {1}.\n {2}", type, sourceAssembly,
                    exception);
            }
            return default(TTargetClass);
        }

        /// <summary>
        /// Methode zum Aufrufen einer Methode, die zu einem beliebigen Objekt gehört
        /// </summary>
        /// <param name="obj">Das Objekt, welches die Auszuführende Methode enthält</param>
        /// <param name="methodName">Der Name der Methode, die ausgeführt werden soll</param>
        /// <param name="parameterObjects">Methodenparameter die der aufgerufenen Methode übergeben werden</param>
        public static void ExecuteMethod(object obj, string methodName, object[] parameterObjects = null)
        {
            MethodInfo mi = obj.GetType().GetMethod(methodName);
            mi.Invoke(obj, parameterObjects);
        }

        /// <summary>
        /// Lädt einen Treiber als Assembly
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        private static Assembly LoadAssembly(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath))
            {
                Logger.Warning(new ArgumentException("DllLoader: The passed assembly path is empty"));
                return null;
            }
            else if (!File.Exists(assemblyPath))
            {
                Logger.Warning("DllLoader: The assembly file \"{0}\" does not exist. Working directory is: \"{1}\"",
                    assemblyPath, Directory.GetCurrentDirectory());
                return null;
            }
            else
            {
                Logger.Debug("DllLoader: Loaded Assembly: \"{0}\"", assemblyPath);
                return Assembly.LoadFrom(assemblyPath);
            }
        }
    }
}