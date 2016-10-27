using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using Akomi.InformationModel;
using Akomi.InformationModel.Component.Connection;
using Akomi.InformationModel.Skills;
using Akomi.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace Tapako.DeviceInformationManagement.IO
{
    /// <summary>
    /// This class takes care about serializing Tapako scans
    /// </summary>
    public class TapakoSerializer
    {
        private JsonSerializerSettings Settings { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TapakoSerializer()
        {
            Settings = new JsonSerializerSettings
            {
                TraceWriter = new TraceLogger(),
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full,
                TypeNameHandling = TypeNameHandling.Objects,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Binder = new RepositoryTypesBinder(),
                Error = JsonErrorHandler,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                NullValueHandling = NullValueHandling.Ignore
            };
            Settings.Converters.Add(new IpAddressConverter());
            Settings.Converters.Add(new ConcreteConverter<ISkillList, SkillList>());
            Settings.Converters.Add(new ConcreteConverter<IConnectionList, ConnectionList>());
        }

        private static void JsonErrorHandler(object sender, ErrorEventArgs e)
        {
            Logger.Error((Exception) e.ErrorContext.Error);
        }

        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            errorContext.Handled = true;
        }

        /// <summary>
        /// Turns <paramref name="obj"/> into a serialized string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Serialize(object obj)
        {
            // Serialize
            return JsonConvert.SerializeObject(obj, Formatting.Indented, Settings);
        }

        /// <summary>
        /// Deserializes a instance with type <typeparamref name="T"/> from <paramref name="serializedData"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedData">A string with serialized information about the instance</param>
        /// <returns></returns>
        public T Deserialize<T>(string serializedData)
        {
            if (!IsValidJson(serializedData))
            {
                Logger.Warning("Could not deserialize the given file");
                return default(T);
            }
            
            // Deserialize
            return JsonConvert.DeserializeObject<T>(serializedData, Settings);
        }

        /// <summary>
        /// Checks if the input Data is correct Json Data
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        private static bool IsValidJson(string inputData)
        {
            var strInput = inputData.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    Logger.Warning(jex);
                    return false;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return false;
                }
            }
            return false;

        }

        /// <summary>
        /// This class is used pass Messages from serializer to <see cref="Logger"/>
        /// </summary>
        private class TraceLogger : ITraceWriter
        {
            public TraceLogger()
            {
                LevelFilter = TraceLevel.Warning;
            }

            public void Trace(TraceLevel level, string message, Exception ex)
            {
                switch (level)
                {
                    case TraceLevel.Error:
                        Logger.Error(message);
                        Logger.Error(ex);
                        break;
                    case TraceLevel.Warning:
                        Logger.Warning(message);
                        Logger.Warning(ex);
                        break;
                    case TraceLevel.Info:
                        Logger.Info(message);
                        Logger.Info(ex);
                        break;
                    case TraceLevel.Verbose:
                        Logger.Debug(message);
                        Logger.Debug(ex);
                        break;
                    default:
                        break;
                }
            }


            public TraceLevel LevelFilter { get; private set; }
        }

        /// <summary>
        /// This Converter takes the converion of <see cref="IPAddress"/>
        /// </summary>
        private class IpAddressConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof (IPAddress));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                IPAddress ip = (IPAddress) value;
                writer.WriteValue(ip.ToString());
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                JToken token = JToken.Load(reader);
                return IPAddress.Parse(token.Value<string>());
            }
        }

        /// <summary>
        /// A Concrete converter implementation for specifying a type <typeparamref name="TConcrete"/> 
        /// that is used to handle a interface type <typeparamref name="TInterface"/>.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TConcrete"></typeparam>
        public class ConcreteConverter<TInterface, TConcrete> : JsonConverter where TConcrete : new ()
            {
            /// <summary>Writes the JSON representation of the object.</summary>
            /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
            /// <param name="value">The value.</param>
            /// <param name="serializer">The calling serializer.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }

            /// <summary>Reads the JSON representation of the object.</summary>
            /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
            /// <param name="objectType">Type of the object.</param>
            /// <param name="existingValue">The existing value of object being read.</param>
            /// <param name="serializer">The calling serializer.</param>
            /// <returns>The object value.</returns>
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return serializer.Deserialize<TConcrete>(reader);
            }

            /// <summary>
            /// Determines whether this instance can convert the specified object type.
            /// </summary>
            /// <param name="objectType">Type of the object.</param>
            /// <returns>
            /// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
            /// </returns>
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(TInterface);
            }
        }

        /// <summary>
        /// This binder looks for not found <see cref="Type"/>s in the unloaded assemblies of the repositories.
        /// </summary>
        public class RepositoryTypesBinder : DefaultSerializationBinder
        {
            /// <summary>
            /// Resolves a type for <paramref name="assemblyName"/> and <paramref name="typeName"/>
            /// </summary>
            /// <param name="assemblyName"></param>
            /// <param name="typeName"></param>
            /// <returns></returns>
            public override Type BindToType(string assemblyName, string typeName)
            {
                Type type = null;
                try
                {
                    type = base.BindToType(assemblyName, typeName);

                    if (type != null)
                    {
                        return type;
                    }
                }
                catch (JsonSerializationException)
                {
                    // Type has not been found, so try to find type in some repositories
                    foreach (var repository in DeviceInformationManager.InformationSources)
                    {
                        foreach (var driverUri in repository.GetDriverUris(null))
                        {
                            // look if driver is dll file
                            if (!Path.GetExtension(driverUri.LocalPath).Equals(".dll"))
                            {
                                continue; // do not load, because file seems to be no driver
                            }

                            var targetAssemblyName = AssemblyName.GetAssemblyName(driverUri.LocalPath);
                            if (targetAssemblyName != null && targetAssemblyName.Name.Equals(assemblyName))
                            {
                                // Load assembly into AppDomain
                                Logger.Debug("Loading assembly \"{0}\"", targetAssemblyName);
                                Assembly assembly = Assembly.Load(targetAssemblyName);
                                type = assembly.GetType(typeName, false);
                                if (type != null)
                                {
                                    return type; // If type was found: finish
                                }
                                // else continue
                            }
                        }
                    }

                    // If not sufficient type has found until here: throw Error;
                    throw;
                }

                return null;
            }

            /// <summary>
            /// This method is used to get the current assembly and type name of <paramref name="serializedType"/>
            /// </summary>
            /// <param name="serializedType"></param>
            /// <param name="assemblyName"></param>
            /// <param name="typeName"></param>
            public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                assemblyName = serializedType.Assembly.GetName().Name;
                typeName = serializedType.FullName;
            }
        }
    }
}