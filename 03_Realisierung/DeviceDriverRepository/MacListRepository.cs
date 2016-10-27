using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using Akomi.InformationModel.Device;
using Akomi.Logger;
using Tapako.Framework;

namespace Tapako.Repositories.DeviceDriverRepository
{
    /// <summary>
    /// Dieses Repository enthält verknüpfungen zwischen Mac-Addressen und dem zugehörigen Treiber
    /// Mehrere Mac-Adressen können auf den gleichen Treiber verweisen
    /// </summary>
    public sealed class MacListRepository
    {
        private string _repositoryName;

        public string RepositoryName
        {
            get { return _repositoryName; }
            set
            {
                _repositoryName = value;
                Logger.Info("New RepositoryName = " + _repositoryName);
                _dic = ParseMacListFileToDictionary(_repositoryName);

                ////todo: Own System can always load the WindowsDeviceDriver
                //foreach (var macAddress in UniversalHostSearcher.GetOwnMacAddresses())
                //{
                //    _dic.Add(macAddress.ToString(), "WindowsDeviceDriver");
                //}

            }
        }

        private static volatile MacListRepository _instance;
        private static readonly object SyncRoot = new Object();
        private Dictionary<string, string> _dic;

        private MacListRepository()
        {
            RepositoryName = Constants.DefaultMacRepository;
        }

        private MacListRepository(string repositoryName)
        {
            RepositoryName = repositoryName;
        }

        public static MacListRepository GetInstance(string repositoryName = "")
        {
            if (_instance == null)
            {
                lock (SyncRoot)
                {
                    if (_instance == null) //todo: warum 2 mal If-Abfrage ?
                    {
                        if (string.IsNullOrEmpty(repositoryName))
                        {
                            _instance = new MacListRepository(Constants.DefaultMacRepository);
                        }
                        else
                        {
                            _instance = new MacListRepository(repositoryName);
                        }
                    }
                    Logger.Info("MacListRepopsitory Instance created.");
                }
            }

            return _instance;
        }

        public static MacListRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new MacListRepository();
                        Logger.Info("MacListRepopsitory Instance created.");
                    }
                }

                return _instance;
            }
        }

        public string GetDllName(PhysicalAddress mac)
        {
            //Console.WriteLine("GetDllName");
            if (mac != null && _dic.ContainsKey(mac.ToString()))
            {
                return _dic[mac.ToString()];
            }

            return null;
        }

        public string GetDllName(string mac)
        {
            return string.IsNullOrEmpty(mac) ? null : GetDllName(PhysicalAddress.Parse(mac));
            //Console.WriteLine("GetDllName");
        }

        /// <summary>
        /// Liest eine Datei ein, in der MAC-Adressen zu den entsprechenden Treibernamen aufgelistet sind
        /// und konvertiert diese in ein Dictionary
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> ParseMacListFileToDictionary(string fileName)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            if (File.Exists(fileName))
            {
                var groups = File.ReadLines(RepositoryName)
                    .Select((v, i) => new {Index = i, Value = v})
                    .GroupBy(p => p.Index/2);

                foreach (var group in groups)
                {
                    string key = group.First().Value;
                    var value = group.Last().Value;

                    if (res.ContainsKey(key))
                    {
                        Logger.Warning("Key: \"{0}\"\n Value: \"{1}\" will be replaced by \"{2}\" ", key, res[key], value);
                    }
                    res[key] = value;
                }
                //res = groups.ToDictionary(g => g.First().Value, g => g.Last().Value);
            }
            else
            {
                Exception exception = new Exception(Path.Combine(Directory.GetCurrentDirectory(), fileName)
                                                    + " was not found! Empty Mac Dictionary will be laoded!\n"
                                                    + new System.Diagnostics.StackTrace());
                Logger.Error(exception);

                res = new Dictionary<string, string>();
            }
            return res;
        }

        /// <summary>
        /// Wandelt das MAC-Adress Repository in ein Dictionary mit Mac und Treibernamen um
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> LoadMacRepository()
        {
            //todo: schnieke xml-repository und ned behinderten tobiKack von .txt datei verwenden
            //todo: wenn der Markus des besser findet, dann soll er es machen und nicht nur an meinen Zeug rummeckern, des wenigstens funktioniert :D
            return null;
        }

        public IDevice GetDeviceInformation(IDevice iDevice)
        {
            //if (typeof (ITapakoDevice) == iDevice.GetType())
            //{
            //ITapakoDevice newDevice = new TapakoDevice();
            //newDevice.DriverName = GetDllName(((ITapakoDevice)iDevice).MacAddress);
            Logger.Info("Get DeviceInformation from MacListRepository for " + iDevice);
            if (iDevice != null && iDevice.Identification != null)
            {
                var driverName = GetDllName(iDevice.Identification.PhysicalAddress);
                return DllLoader.Load<IDevice>(driverName);
            }

            return null;
        }

    }
}