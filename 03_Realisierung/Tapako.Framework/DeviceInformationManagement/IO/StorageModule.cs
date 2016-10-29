using System;
using System.IO;
using System.Runtime.Serialization;
using Akomi.Logger;
using Microsoft.Win32;
using Tapako.Framework;

namespace Tapako.DeviceInformationManagement.IO
{
    /// <summary>
    /// Zuständig für das serialisierte abspeichern von objekten
    /// </summary>
    public static class StorageModule
    {
        /// <summary>
        /// This serializer will be used to perform save and load tasks
        /// </summary>
        public static TapakoSerializer Serializer = new TapakoSerializer();

        /// <summary>
        /// Serializes and saves <paramref name="obj"/> into <paramref name="saveFile"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">object to save</param>
        /// <param name="saveFile"></param>
        public static void SaveToFile<T>(T obj, string saveFile = null)
        {
            if (string.IsNullOrWhiteSpace(saveFile))
            {
                saveFile = ChooseFile(obj, Mode.Save);
            }

            if (!string.IsNullOrEmpty(saveFile))
            {
                try
                {
                    string dir = Path.GetDirectoryName(saveFile);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    File.WriteAllText(saveFile, SerializeToString(obj));
                    Logger.Info("\"{0}\" was written to file \"{1}\".", obj.ToString(), saveFile);
                }
                catch (Exception ex)
                {
                    if (ex is SerializationException)
                    {
                        Logger.Error(ex);
                    }

                    else if (ex is TypeInitializationException || ex is DllNotFoundException)
                    {
                        //bug: diese beiden Fehler werden nicht abgefangen; tritt auf wenn man fertiges Device auf PC lädt und versucht zu speichern, wenn auf PC kein TwinCAT installiert ist
                        Logger.Error(ex);
                    }
                    else
                    {
                        Logger.Error(ex);
                        //throw;
                    }
                }
            }
        }

        /// <summary>
        /// This method opens a dialog for choosing a file.
        /// </summary>
        /// <param name="fileNameSuggestion"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string ChooseFile(string fileNameSuggestion, Mode mode)
        {
            Logger.Debug("Starting FileDialog...");
            FileDialog fileDialog;
            switch (mode)
            {
                case Mode.Save:
                    fileDialog = new SaveFileDialog();
                    break;
                case Mode.Load:
                    fileDialog = new OpenFileDialog();
                    break;
                default:
                    mode = Mode.Load;
                    fileDialog = new OpenFileDialog();
                    break;
            }
            
            fileDialog.Title = mode.ToString() + " Device";
            fileDialog.Filter = "Device Files (*.device)|*.device|All Files (*.*)|*.*";
            fileDialog.FileName = FunctionCollection.ReplaceIllegalPathCharacters(fileNameSuggestion, "_") + ".device";

            bool? fileChoosen = fileDialog.ShowDialog();

            if (fileChoosen == null || !fileChoosen.Value)
            {
                Logger.Debug("No file choosen");
                return null;
            }

            return fileDialog.FileName;
        }

        /// <summary>
        /// Specifies the mode/operation, which will be performed
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Means that a save task may be performed
            /// </summary>
            Save,

            /// <summary>
            /// Means that a load task may be performed
            /// </summary>
            Load
        }

        /// <summary>
        /// <see cref="ChooseFile(string,Tapako.DeviceInformationManagement.IO.StorageModule.Mode)"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string ChooseFile(object obj, Mode mode)
        {
            return ChooseFile(obj.ToString(), mode);
        }

        /// <summary>
        /// Tries to resolve a object of type <typeparamref name="T"/> from <paramref name="filePath"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T LoadFromFile<T>(string filePath = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                filePath = ChooseFile("", Mode.Load);
            }


            if (string.IsNullOrWhiteSpace(filePath))
            {
                Logger.Info("No load-file selected!");
                return default(T);
            }

            if (!File.Exists(filePath))
            {
                Logger.Warning("There is no file like \"{0}\"!", filePath);
                return default(T);
            }

            try
            {
                Logger.Debug("Starting to deserialize object from file \"{0}\"...", filePath);

                T device = DeserializeDevice<T>(filePath);
                if (device != null)
                {
                    Logger.Info("Device \"{0}\" was successful loaded from \"{1}\".", device.ToString(), filePath);
                }
                return device;
            }
            catch (SerializationException exception)
            {
                Logger.Error(exception);
                return default(T);
            }
        }

        private static string SerializeToString<T>(T value)
        {
            Logger.Debug("Starting to serialize \"{0}\"...", value.ToString());

            //var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            //var serializer = GetSerializer(value);

            //var settings = new XmlWriterSettings
            //{
            //    Indent = true,
            //    OmitXmlDeclaration = true
            //};

            //using (var stream = new StringWriter())
            //using (var writer = XmlWriter.Create(stream, settings))
            //{
            //    serializer.Serialize(writer, value, emptyNamepsaces);
            //    return stream.ToString();
            //}
            return Serializer.Serialize(value);
        }


        private static T DeserializeDevice<T>(string filePath)
        {
            //T result = default(T);

            //var serializer = GetSerializer(result);

            //using (XmlReader reader = XmlReader.Create(filePath))
            //{
            //    result = (T)serializer.Deserialize(reader);
            //}

            //return result;
            return Serializer.Deserialize<T>(File.ReadAllText(filePath));
        }

        //private static XmlSerializer GetSerializer<T>(T value)
        //{
        //    var knownTypes = new List<Type> { typeof(DeviceBase) };

        //    Type serializeType;
        //    if (value == null)
        //    {
        //        serializeType = typeof(T);
        //    }
        //    else
        //    {
        //        serializeType = value.GetType();
        //    }


        //    foreach (var type in typeof(IDevice).Assembly.GetTypes())
        //    {
        //        //if (typeof(ISkill).IsAssignableFrom(type))
        //        //{

        //        //}
        //        if (type.IsGenericType || type.IsInterface || type.IsAbstract || !type.IsPublic)
        //        {
        //            continue;
        //        }
        //        knownTypes.Add(type);

        //    }

        //    var serializer = new XmlSerializer(serializeType, knownTypes.ToArray());
        //    return serializer;
        //}
    }
}