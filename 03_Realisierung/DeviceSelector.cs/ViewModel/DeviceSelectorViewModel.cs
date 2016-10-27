using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Akomi.InformationModel.Device;
using ExtensionMethodsCollection;
using Tapako.DeviceInformationManagement;
using Tapako.Repositories.DeviceDriverRepository;

namespace Tapako.Utilities.DeviceSelector.ViewModel
{
    class DeviceSelectorViewModel : IDeviceSelectorViewModel
    {
        public DeviceSelectorViewModel()
        {
            SerialNumberSuggestions = GetSerialNumbersFromFiles();
        }
        /// <summary>
        /// Das aktuell eingegebene Modell
        /// </summary>
        public string ModelNumber { get; set; }

        public string SerialNumber { get; set; }

        /// <summary>
        /// Liefert eine Liste der Namen, welche bei eingabe eines neuen Modells vorgeschlagen werden
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable GetDeviceModelNames(Type type)
        {
            if (ModelSuggestions != null && ModelSuggestions.Any())
            {
                return OverrideModelSuggestions ? ModelSuggestions : GetDefaultModelNames().Concat(ModelSuggestions);
            }
            else
            {
                return GetDefaultModelNames();
            }
        }

        private IEnumerable<string> _additionalModelSuggestions;

        /// <summary>
        /// Eine Liste von Vorschlägen, welche im Code unabhängig von der Resource-Datei gesetzt werden kann
        /// </summary>
        public IEnumerable<string> ModelSuggestions
        {
            get
            {
                if (OverrideModelSuggestions) return _additionalModelSuggestions;

                return _additionalModelSuggestions == null
                    ? GetDefaultModelNames()
                    : GetDefaultModelNames().Concat(_additionalModelSuggestions);
            }
            set { _additionalModelSuggestions = value; }
        }

        public Dictionary<string, List<string>> SerialNumberSuggestions { get; set; }

        /// <summary>
        /// Bestimmt, ob die Vorschläge, die im Code übergeben wurden, die Vorschläge aus der Resource-Datei erweitern oder überschreiben
        /// </summary>
        public bool OverrideModelSuggestions { get; set; }

        public IDevice ParentDevice { get; set; }

        /// <summary>
        /// Setzt die Benutzereingaben, die in diesem ViewModel gespeichert wurden, zurück
        /// </summary>
        public void Reset()
        {
            ModelNumber = null;
            SerialNumber = null;
        }

        /// <summary>
        /// Returns
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<string> GetDefaultModelNames()
        {
            return GetDefaultModelNamesFromFiles();
            //todo: use GetDefaultModelNamesFromFiles()
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<string> GetDefaultModelNamesFromFiles()
        {
            var deviceDriverRepositories = DeviceInformationManager.InformationSources.OfType<DeviceDriverRepository>();

            List<string> files = new List<string> {""};

            foreach (var deviceDriverRepository in deviceDriverRepositories)
            {
                if (Directory.Exists(deviceDriverRepository.RepositoryFolder))
                {
                    foreach (var file in Directory.GetFiles(deviceDriverRepository.RepositoryFolder))
                    {
                        if (Path.GetExtension(file) == ".dll")
                        {
                            files.Add(Path.GetFileNameWithoutExtension(file));
                        }
                    }
                }
            }
            return files;
        }

        private static Dictionary<string, List<string>> GetSerialNumbersFromFiles()
        {
            var deviceDriverRepositories = DeviceInformationManager.InformationSources.OfType<DeviceDriverRepository>();

            //dictionary with an empty entry
            var dictionary = new Dictionary<string, List<string>> {{"", new List<string> {""}}};

            foreach (var deviceDriverRepository in deviceDriverRepositories)
            {
                if (Directory.Exists(deviceDriverRepository.RepositoryFolder))
                {
                    foreach (var directory in Directory.GetDirectories(deviceDriverRepository.RepositoryFolder))
                    {
                        var files = new List<string>();
                        foreach (var file in Directory.GetFiles(directory))
                        {
                            if (Path.GetExtension(file) == ".xml")
                            {
                                files.Add(Path.GetFileNameWithoutExtension(file));
                            }
                        }
                        if (!files.IsNullOrEmpty())
                        {
                            dictionary.Add(new DirectoryInfo(directory).Name, files);
                        }

                    }
                }
            }
            return dictionary;
        }

        /// <summary>
        /// Liest die Resource-Datei ein.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<string> GetDefaultModelNamesFromXml()
        {
            //todo: from filesystem und vorschläge für seriennummer
            var list = new List<string>();
            string filePath = @"Resources\ModelNames.xml";

            string modelNamesXml;
            if (
                Assembly.GetAssembly(typeof (DeviceSelectorViewModel))
                    .TryReadEmbeddedString(filePath, out modelNamesXml) && !string.IsNullOrEmpty(modelNamesXml))
            {
                // Loading from a file, you can also load from a stream
                //var xml = XDocument.Load(filePath);
                var xml = XDocument.Parse(modelNamesXml);

                // Query the data and write out a subset of contacts
                if (xml.Root != null)
                {
                    var query = xml.Root.Descendants("ModelName");
                    //where (int)c.Attribute("id") < 4
                    //select c.Element("firstName").Value + " " +
                    //       c.Element("lastName").Value;


                    foreach (string name in query)
                    {
                        list.Add(name);
                        Debug.WriteLine("New Suggestion \"" + name + "\" added.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Resource File {0} not found at {1}", filePath, Directory.GetCurrentDirectory());
            }

            return list;
        }
    }
}