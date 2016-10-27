using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using IAkomiDevice.fIDevice;
using Tapako.DeviceInformationManagement;
using TapakoInterfaces;
using TapakoPublicClasses;
using TapakoServices;

namespace MacListRepository
{
    public sealed class MacListRepository : IInformationSource
    {
        private string _repositoryName;
        private Priority _priority = Priority.High;

        public string RepositoryName
        {
            get { return _repositoryName; }
            set
            {
                _repositoryName = value;
                _dic = ParseMacListFileToDictionary();
            }
        }

        private static volatile MacListRepository _instance;
        private static readonly object SyncRoot = new Object();
        private static Dictionary<string, string> _dic = new Dictionary<string, string>();  

        public MacListRepository()
        {
            RepositoryName = Constants.DefaultMacRepository;
            _dic = ParseMacListFileToDictionary();
        }

        public MacListRepository(string repositoryName)
        {
            RepositoryName = repositoryName;
            _dic = ParseMacListFileToDictionary();
        }

        public static MacListRepository GetInstance(string repositoryName = "")
        {
            if (_instance == null)
            {
                lock (SyncRoot)
                {
                    if (_instance == null) //todo: warum 2 mal If-Abfrage ?
                    {
                        if (String.IsNullOrEmpty(repositoryName))
                        {
                            _instance = new MacListRepository(Constants.DefaultMacRepository);
                        }
                        else
                        {
                            _instance = new MacListRepository(repositoryName);
                        }
                    }
                    Logger.Info("MacListRepopsitory Instance created. RepositoryName = " + _instance.RepositoryName);
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
                        Logger.Info("MacListRepopsitory Instance created. RepositoryName = " + _instance.RepositoryName);
                    }
                }

                return _instance;
            }
        }

        public string GetDllName(PhysicalAddress mac)
        {
            //Console.WriteLine("GetDllName");
            if (_dic.ContainsKey(mac.ToString()))
            {
                return _dic[mac.ToString()];
            }

            return null;
        }

        public string GetDllName(string mac)
        {
            //Console.WriteLine("GetDllName");
            if (mac != null)
            {
                return GetDllName(PhysicalAddress.Parse(mac));
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Liest eine Datei ein, in der MAC-Adressen zu den entsprechenden Treibernamen aufgelistet sind
        /// und konvertiert diese in ein Dictionary
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> ParseMacListFileToDictionary()
        {
            Dictionary<string, string> res;
            if (File.Exists(RepositoryName))
            {
                res = File
                    .ReadLines(RepositoryName)
                    .Select((v, i) => new { Index = i, Value = v })
                    .GroupBy(p => p.Index / 2)
                    .ToDictionary(g => g.First().Value, g => g.Last().Value);
            }
            else
            {
                Exception exception = new Exception(Path.Combine(Directory.GetCurrentDirectory(), RepositoryName)
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
            //newDevice.DriverName = GetDllName(((ITapakoDevice)iDevice).MacAddress); // todo: den namen irgendwie im IDevice unterbringen

            var driverName = GetDllName(((ITapakoDevice)iDevice).Identification.PhysicalAddress); // todo: den namen irgendwie im IDevice unterbringen
            return DllLoader.Load<IDevice>(driverName); ;

            //}
            //return null;
        }

        public Priority Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }
    }
}
