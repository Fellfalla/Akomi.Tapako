using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Akomi.InformationModel.Device;
using Akomi.Logger;
using ExtensionMethodsCollection;
using Tapako.DeviceInformationManagement.InformationSources;
using Tapako.Repositories.DeviceDriverRepository;

namespace Tapako.Repositories.VirtualRepresentationRepository
{
    public class VirtualRepresentationRepository : RepositoryBase
    {
        private SourcePriority _sourcePriority = SourcePriority.Medium;

        private MacListRepository _macListRepository;

        private string _backupExtension = ".backup";

        public MacListRepository MacListRepository
        {
            get { return _macListRepository ?? (_macListRepository = MacListRepository.GetInstance()); }
            set { _macListRepository = value; }
        }

        public VirtualRepresentationRepository()
        {
            RepositoryFolder = Directory.GetCurrentDirectory();
        }

        public VirtualRepresentationRepository(string repositoryFolder) : base(repositoryFolder)
        {
        }

        protected override IDevice InnerGetDeviceInformations(IDevice iDevice)
        {
            string filePath = GetFilePath(iDevice);
            if (filePath == null)
            {
                Logger.Warning("{0}: Could not find driver for {1}", Name, iDevice.ToString(true));
                return null;
            }

            //IDevice device = CreateDeviceFromFile(filePath);
            return CompleteInformation(filePath, iDevice);

            //return device;
        }

        private string _currentFile; // this field is used for logger ouput in recursive loop

        private T CompleteInformation<T>(string filePath, T obj)
        {
            var informationSource = XDocument.Load(filePath);
            _currentFile = filePath;
            return CompleteInformation(informationSource.Root, obj);
        }

        private T CompleteInformation<T>(XElement informationSource, T obj)
        {
            if (informationSource.HasElements)
            {
                Parallel.ForEach(informationSource.Elements(), element =>
                {
                    if (string.IsNullOrWhiteSpace(element.Name.LocalName))
                    {
                        Logger.Warning("Empty tag in IR-File \"{0}\".", _currentFile);
                    }

                    object child = null;
                    bool isPrimitive = false;
                    bool isConvertible = false;
                    PropertyInfo propInfo = null;
                    FieldInfo fieldInfo = null;
                    try // to get child of element
                    {
                        propInfo = obj.GetType().GetProperty(element.Name.LocalName);
                        if (propInfo != null)
                        {
                            child = propInfo.GetValue(obj);
                            isPrimitive = propInfo.PropertyType.IsPrimitive;
                            isConvertible = typeof(IConvertible).IsAssignableFrom(propInfo.PropertyType);
                        }
                        else
                        {
                            fieldInfo = obj.GetType().GetField(element.Name.LocalName);
                            if (fieldInfo != null)
                            {
                                child = fieldInfo.GetValue(obj);
                                isPrimitive = fieldInfo.FieldType.IsPrimitive;
                                isConvertible = typeof(IConvertible).IsAssignableFrom(fieldInfo.FieldType);
                            }
                        }
                    }
                    catch(AmbiguousMatchException) { }
                    catch(ArgumentNullException) { }
                    
                    if (isPrimitive || isConvertible) // if element is primitive
                    {
                        if (propInfo != null)
                        {
                            propInfo.SetValue(obj, Convert.ChangeType(element.Value, propInfo.PropertyType, CultureInfo.InvariantCulture));
                        }
                        else if (fieldInfo != null)
                        {
                            fieldInfo.SetValue(obj, Convert.ChangeType(element.Value, fieldInfo.FieldType));
                        }
                    }
                    else  // child is not primitive -> Go deeper
                    {
                        CompleteInformation(element, child);
                    }
                });
            }
            else
            {
                Logger.Warning("Empty tag \"{0}\" in \"{1}\".", informationSource.Name, _currentFile);
            }

            return obj;
        }

        /// <summary>
        /// Stores the Membervalues of the device into existing Tags in the XmlFile
        /// </summary>
        /// <param name="iDevice"></param>
        protected override void InnerStoreDeviceInformations(IDevice iDevice)
        {
            // Make Backup if none exists yet
            MakeBackup(iDevice);

            // Get VR information structure
            string vrFile = GetFilePath(iDevice);
            var xml = XDocument.Load(vrFile);
            SaveObjectToXml(iDevice, xml.Root);

            // Store those information into XML
            xml.Save(vrFile, SaveOptions.None);
        }

        private void SaveObjectToXml(object obj, XElement xElement)
        {
            if (obj == null)
            {
                xElement.Value = string.Empty;
                return;
            }

            foreach (var element in xElement.Elements())
            {
                object objValueForElement = GetReflectedValue(obj, element.Name.LocalName);

                if (element.HasElements) // go deeper
                {
                    SaveObjectToXml(objValueForElement, element);
                }
                else if (objValueForElement != null)
                {
                    element.SetValue(objValueForElement);
                }
                else
                {
                    element.SetValue(string.Empty);
                }
            }

            // todo: Store given properties or fields, that don't exist in VR File
        }

        /// <summary>
        /// Returns null if no member was found, or the value itself is null
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        private object GetReflectedValue(object obj, string memberName)
        {
            var property = obj.GetType().GetProperty(memberName);
            if (property == null)
            {
                // Try to do the same with field
                var field = obj.GetType().GetField(memberName);
                if (field != null)
                {
                    return field.GetValue(obj);
                }
                else
                {
                    Logger.Warning("Could not find member \"{0}\" in \"{1}\"", memberName, obj.GetType().FullName);
                }
            }
            else
            {
                return property.GetValue(obj);
            }
            return null; // returns null if no member was found
        }

        public override void RestoreDeviceInformations(IDevice iDevice)
        {
            RestoreBackup(iDevice);
        }

        public override void RestoreInformationSource()
        {
            RestoreAllBackups();
        }

        public override string Name
        {
            get { return "Virtual Representation Repository"; }
        }

        public override int Id
        {
            get { return 2; }
        }

        public override bool HasDeviceDriver(IDevice iDevice)
        {
            return File.Exists(GetFilePath(iDevice));
        }

        public override SourcePriority SourcePriority
        {
            get { return _sourcePriority; }
            set { _sourcePriority = value; }
        }


        protected string GetFilePath(IDevice iDevice)
        {
            if (iDevice != null && iDevice.Identification != null &&
                ((!string.IsNullOrEmpty(iDevice.Identification.SerialNumber) &&
                !string.IsNullOrEmpty(iDevice.Identification.ModelNumber)) ||
                !string.IsNullOrEmpty(iDevice.Identification.PhysicalAddress)))
            {
                string filePath;
                if (File.Exists(filePath = GenerateFilePath(iDevice)))
                {
                    return filePath;
                }
                if (File.Exists(filePath = GenerateFilePathFromMac(iDevice)))
                {
                    return filePath;
                }
            }
            return null;
        }

        private string GenerateFilePathFromMac(IDevice iDevice)
        {
            var hostName = MacListRepository.GetDllName(iDevice.Identification.PhysicalAddress);

            if (string.IsNullOrEmpty(hostName)) return null;

            return base.GetFilePath(iDevice.Identification.PhysicalAddress,
                Path.Combine(RepositoryFolder, hostName));
        }

        private string GenerateFilePath(IDevice iDevice)
        {
            return base.GetFilePath(iDevice.Identification.SerialNumber,
                Path.Combine(RepositoryFolder, ReplaceIllegalCharacters(iDevice.Identification.ModelNumber)));
        }

        /// <summary>
        /// Initialisiert ein neues Device und befüllt diese mit den VR-Informationen
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static IDevice CreateDeviceFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            Logger.Debug("IRR" +
                         " is loading {0}...", filePath);
            IDevice device = default(IDevice);
            Type deserializedType = typeof(DeviceBase);
         

            var serializer = new XmlSerializer(deserializedType);

            var xml = ReformatXmlFile(filePath, deserializedType, saveReformattedFile: true);

            try
            {
                device = (IDevice)serializer.Deserialize(xml.CreateReader());
            }
            catch (InvalidOperationException exception)
            {
                Logger.Error("VRR: InvalidOperationException while trying to load \"{0}\".\n\n{1}", filePath, exception.ToString(deep: true));
            }

            return device;
        }

        /// <summary>
        /// Adjusts the root tag with DeviceBase
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="rootType"></param>
        /// <param name="saveReformattedFile"></param>
        private static XDocument ReformatXmlFile(string filePath, Type rootType, bool saveReformattedFile = true)
        {

            var xml = XDocument.Load(filePath);
            if (xml.Root != null)
            {
                string tagName = xml.Root.Name.LocalName;
                string typeName = rootType.Name;
                xml = ReformatXmlFile(xml, rootType);

                if (saveReformattedFile && !tagName.Equals(typeName))
                {
                    Logger.Debug("VRR: Changing root tag of \"{0}\" which is currently \"{1}\" to \"{2}\"", filePath, tagName,
                        typeName);
                    try
                    {
                        xml.Save(filePath);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Could not change XML root tag:\n{0}", e);
                    }
                }
            }
            return xml;
        }

        /// <summary>
        /// Adjusts the root tag with DeviceBase
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="rootType"></param>
        private static XDocument ReformatXmlFile(XDocument xml, Type rootType)
        {
            if (xml.Root != null)
            {
                string tagName = xml.Root.Name.LocalName;
                string typeName = rootType.Name;

                if (!tagName.Equals(typeName))
                {
                    //Logger.Info("Changing root tag of \"{0}\" which is currently \"{1}\" to \"{2}\"", filePath, tagName, typeName);
                    xml.Root.Name = typeName;
                }
            }
            return xml;
        }

        private static void SetPropertyValue(object obj, PropertyInfo property, XElement xElement)
        {
            if (property == null || xElement == null || obj == null || !property.CanRead || !property.CanWrite)
            {
                return;
            }
            Logger.Debug("VRR: Setting value of property \"{0}\"", property.Name, xElement);

            // Bei untergeordnete Elementen -> Property zu jedem element raussuchen und diese Methode rekursiv ausführen
            if (xElement.HasElements)
            {
                foreach (var element in xElement.Elements())
                {
                    PropertyInfo subProperty = property.PropertyType.GetProperty(element.Name.LocalName);
                    object subObject = property.GetValue(obj);

                    if (subObject == null)
                    {
                        subObject = CreateInstance(property.PropertyType);
                    }
                    property.SetValue(obj, subObject);
                    SetPropertyValue(subObject, subProperty, element);
                }
            }
            else
            {
                string typeName = property.PropertyType.Name;
                string xmlValue = xElement.Value;

                try
                {
                    TypeCode targetType;


                    if (Enum.TryParse(typeName, out targetType))
                    {
                        var newValue = Convert.ChangeType(xmlValue, targetType);
                        Logger.Debug("VRR: Setting \"{0}\" for value of property \"{1}\"", newValue, property.Name);
                        property.SetValue(obj, newValue);
                    }
                    else
                    {
                        Logger.Debug("VRR: Cannot convert \"{0}\" to \"{1}\"", xmlValue, typeName);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Error("VRR: Tried to set \"{0}\" as \"{1}\" value of property \"{2}\". \n{3}", 
                        xmlValue, typeName, property.Name, exception.ToString(true));
                }
            }
        }

        private static object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// Makes a new backup file from current vr file.
        /// Adds <see cref="_backupExtension"/> behind file
        /// </summary>
        /// <param name="backupTarget">Target object, to which the backupfile refers</param>
        /// <param name="overrideExistingBackups">If true, existing backups will be overriden</param>
        private void MakeBackup(IDevice backupTarget, bool overrideExistingBackups = false)
        {
            string file = GetFilePath(backupTarget);

            string backupFile = file + _backupExtension;

            // Save only if not existing or override is activated
            if (!File.Exists(backupFile) || overrideExistingBackups)
            {
                if (File.Exists(file))
                {
                    File.Copy(file, backupFile);
                }
            }
        }

        private void RestoreAllBackups()
        {
            foreach (var file in Directory.GetFiles(RepositoryFolder))
            {
                RestoreBackup(file);
            }
        }

        /// <summary>
        /// Restores a backup file from current vr file.
        /// </summary>
        /// <param name="backupTarget">Target object, to which the backupfile refers</param>
        private void RestoreBackup(IDevice backupTarget)
        {
            string file = GetFilePath(backupTarget);

            RestoreBackup(file);
        }


        private void RestoreBackup(string file)
        {
            string backupFile = file + _backupExtension;

            if (File.Exists(backupFile))
            {
                File.Copy(backupFile, file, true);
            }
        }
    }
}