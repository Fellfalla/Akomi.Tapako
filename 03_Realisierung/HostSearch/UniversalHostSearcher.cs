using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Akomi.InformationModel.Component.Identification;
using Akomi.InformationModel.Device;
using Akomi.Logger;
using Tapako.Framework;
using Tapako.Framework.ExtensionMethods;

namespace Tapako.Utilities.UniversalHostSearch
{
    /// <summary>
    /// todo: auch dieser Treiber soll geladen werden (für den ausführenden Pc)
    /// </summary>
    public static class UniversalHostSearcher
    {
        //Dauer: Ping ~0.5 sec, Hostname ~ 4 sec, Mac ~ 4 sec

        #region private Fields

        public static readonly CountingStopwatch MacStopwatch = new CountingStopwatch();
        public static readonly CountingStopwatch HostnameStopwatch = new CountingStopwatch();
        public static readonly CountingStopwatch PingStopwatch = new CountingStopwatch();

        private static int _startIpLastDigit = byte.MinValue;
        private static int _endIpLastDigit = byte.MaxValue;
        private static int _responsiveIpAddressCounter;
        private static int _maxPingRetries = 3;

        /// <summary>
        /// timeout in seconds
        /// </summary>
        private static int _pingTimeout = 1;
        #endregion

        #region private Functions

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseIp"></param>
        /// <param name="first">The lowest Ip-Adress number in the last block of the returned Ip-Range</param>
        /// <param name="last">The highest Ip-Adress number in the last block of the returned Ip-Range</param>
        /// <returns></returns>
        public static IEnumerable<IPAddress> GetIpListFromSubnet(string baseIp, int first = 0, int last = 255)
        {
            for (var i = first; i <= last; i++)
            {
                IPAddress ipAddress;
                if (IPAddress.TryParse(baseIp + i, out ipAddress))
                {
                    yield return ipAddress;
                }
                else
                {
                    //falls die übergeben Ip-Addresse ungültiges Format hat
                    Logger.Error("The given IP address {0} has a not valid format!", baseIp);
                    break;
                }
            }
        }

        private static string GetHostName(IPAddress ipaddress)
        {
            var host = new IPHostEntry {HostName = "Name not available"};
            HostnameStopwatch.Start();
            try
            {
                host = Dns.GetHostEntry(ipaddress);
            }
            catch (SocketException ex)
            {
                Logger.Warning("GetHostName of \"{0}\" didn't work: \n{1}", ipaddress, ex).ConfigureAwait(false);
            }
            HostnameStopwatch.Stop();
            return host.HostName;
        }

        private static async Task<string> GetHostNameAsync(IPAddress ipaddress)
        {
            var host = new IPHostEntry {HostName = "Name not available"};
            HostnameStopwatch.Start();
            Exception error = null;
            try
            {
                host = await Dns.GetHostEntryAsync(ipaddress);
            }
            catch (SocketException ex)
            {
                error = ex;
            }

            if (error != null)
            {
                await Logger.Warning("GetHostName of \"{0}\" didn't work: \n{1}", ipaddress, error).ConfigureAwait(false);
            }

            HostnameStopwatch.Stop();
            return host.HostName;
        }

        private static async Task<PhysicalAddress> GetMacAddressAsync(IPAddress ipAddress)
        {
            MacStopwatch.Start();
            PhysicalAddress macAddress;
            var pProcess = new Process
            {
                StartInfo =
                {
                    FileName = "arp",
                    Arguments = "-a " + ipAddress,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            //var processStarted = new TaskCompletionSource<bool>();
            //await pProcess.StartAsync(processStarted).ConfigureAwait(false);
            //await processStarted.Task.ConfigureAwait(false);
            pProcess.Start();
            string strOutput = await pProcess.StandardOutput.ReadToEndAsync();
            string[] substrings = strOutput.Split('-');
            if (substrings.Length >= 8)
            {
                var macString = substrings[3].Substring(Math.Max(0, substrings[3].Length - 2))
                                + "-" + substrings[4] + "-" + substrings[5] + "-" + substrings[6]
                                + "-" + substrings[7] + "-"
                                + substrings[8].Substring(0, 2);
                MacStopwatch.Stop();
                macAddress = PhysicalAddress.Parse(macString.ToUpper());
                // To Upper da der Parser keine lowercases verträgt
                return macAddress;
            }

            //Eigene Mac adresse wird durch obigen Ablauf nicht gefunden, deshalb sind folgende Zeilen notwendig
            macAddress = IsOwnIpAddress(ipAddress) ? GetOwnMacAddres(ipAddress) : PhysicalAddress.None;
            MacStopwatch.Stop();
            return macAddress;
        }

        private static PhysicalAddress GetMacAddress(IPAddress ipAddress)
        {
            MacStopwatch.Start();
            PhysicalAddress macAddress;
            var pProcess = new Process
            {
                StartInfo =
                {
                    FileName = "arp",
                    Arguments = "-a " + ipAddress,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            pProcess.Start();
            string strOutput = pProcess.StandardOutput.ReadToEnd();
            string[] substrings = strOutput.Split('-');
            if (substrings.Length >= 8)
            {
                var macString = substrings[3].Substring(Math.Max(0, substrings[3].Length - 2))
                                + "-" + substrings[4] + "-" + substrings[5] + "-" + substrings[6]
                                + "-" + substrings[7] + "-"
                                + substrings[8].Substring(0, 2);
                MacStopwatch.Stop();
                macAddress = PhysicalAddress.Parse(macString.ToUpper());
                // To Upper da der Parser keine lowercases verträgt
                return macAddress;
            }

            //Eigene Mac adresse wird durch obigen Ablauf nicht gefunden, deshalb sind folgende Zeilen notwendig
            macAddress = IsOwnIpAddress(ipAddress) ? GetOwnMacAddres(ipAddress) : PhysicalAddress.None;
            MacStopwatch.Stop();
            return macAddress;
        }

        private static bool IsOwnIpAddress(IPAddress ipAddress)
        {
            IPAddress[] localIpAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            return (localIpAddresses.Contains(ipAddress));
        }


        /// <summary>
        /// Gibt die Mac-adresse des lokalen PCs zurück
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns>Physical Addres oder null falls diese nicht in übereinstimmung mit der ipAdresse gefunden wird</returns>
        private static PhysicalAddress GetOwnMacAddres(IPAddress ipAddress)
        {
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (UnicastIPAddressInformation ip in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.Equals(ipAddress))
                        {
                            return networkInterface.GetPhysicalAddress();
                        }
                    }
                }
            }
            return null;
        }

        public static IEnumerable<PhysicalAddress> GetOwnMacAddresses()
        {
            return NetworkInterface.GetAllNetworkInterfaces().Select(i => i.GetPhysicalAddress()).ToList();
        }

        /// <summary>
        /// Liefert alle eigenen IpV4-Adresse zurück
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<IPAddress> GetOwnIpv4Addresses()
        {
            IPAddress[] localIpAddresses = Dns.GetHostAddresses(Dns.GetHostName());

            var ipAddresses = new List<IPAddress>();
            foreach (var ipAddress in localIpAddresses)
            {
                if (IPAddress.IsLoopback(ipAddress))
                {
                    continue;
                }
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddresses.Add(ipAddress);
                }
            }
            return ipAddresses;
        }

        /// <summary>
        /// Liefert eine eigene IpV4-Adresse anhand der ersten akzeptablen Netzwerkaddresse zurück
        /// </summary>
        /// <returns></returns>
        private static IPAddress GetOwnIpAddress()
        {
            return GetOwnIpv4Addresses().FirstOrDefault();
        }

        #endregion

        #region public Functions

        /// <summary>
        /// Searches for SubSystems in an specific IpAddress Intervall
        /// </summary>
        /// <param name="startIp">ex. 192.168.1.1</param>
        /// <param name="endIp">ex. 192.168.1.4</param>
        /// <returns>List of HostDevices (with Ipaddress, Mac, ...)</returns>
        public static IEnumerable<IDevice> SearchForSubSystems(string startIp, string endIp)
        {
            //Aufteilung in BaseIp (alle bis auf den letzten IP-Block) und den letzten IP-Block
            _startIpLastDigit = Int32.Parse(startIp.Split('.').Last());
            _endIpLastDigit = Int32.Parse(endIp.Split('.').Last());

            string baseIp = startIp.Remove(startIp.LastIndexOf(".", StringComparison.Ordinal)) + ".";

            return SearchForSubSystems(baseIp);
        }

        /// <summary>
        /// Searches for SubSystems in the IpAddress intervall of the Host-Computer
        /// </summary>
        /// <returns>List of HostDevices (with Ipaddress, Mac, ...)</returns>
        public static IEnumerable<IDevice> SearchForSubSystems()
        {
            return SearchForSubSystems(GetOwnIpAddress());
        }

        /// <summary>
        /// Searches for SubSystems in the IpAddress intervall of the Host-Computer
        /// </summary>
        /// <returns>List of HostDevices (with Ipaddress, Mac, ...)</returns>
        public static IEnumerable<IDevice> SearchForSubSystems(IPAddress ipAddress)
        {
            // Leitet die Suchanfrage weiter
            return SearchForSubSystems(ipAddress.ToString());
        }

        public static IEnumerable<string> GetIpAddressSuggestions()
        {
            var suggestions = new List<string> {Constants.DefaultSubnet};

            foreach (var ownIpAddress in GetOwnIpv4Addresses())
            {
                string subnet = ParseForSubnetString(ownIpAddress.ToString());
                if (!suggestions.Contains(subnet))
                {
                    suggestions.Add(subnet);
                }
            }

            return suggestions;
        }

        /// <summary>
        /// Diese Methode Extrahiert einen zur Suche verwendbaren SubnetString aus der übergeben IpAdresse
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static string ParseForSubnetString(IPAddress ipAddress)
        {
            if (ipAddress == null)
            {
                return null;
            }
            return ParseForSubnetString(ipAddress.ToString());
        }

        /// <summary>
        /// Diese Methode Extrahiert einen zur Suche verwendbaren SubnetString aus dem übergebenen String
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static string ParseForSubnetString(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
            {
                return ParseForSubnetString(GetOwnIpAddress());
            }
            string parsedSubnetString = string.Empty;
            List<string> blocks = new List<string>(ipAddress.Split('.'));
            // Den String in einzelne Blöcke unterteilen, damit diese gezählt und analysiert werden können

            const string notAcceptedFormat = " has not accepted format. Accepted formats look like XXX.XXX.XXX";

            if (blocks.Count == 3) // Falls das Format xxx xxx xxx vorliegt
            {
                foreach (var block in blocks)
                {
                    byte number;
                    if (byte.TryParse(block, out number))
                    {
                        parsedSubnetString += block + ".";
                    }
                    else
                    {
                        Logger.Debug(ipAddress + notAcceptedFormat);
                        return null;
                    }
                }
                return parsedSubnetString;
            }
            else if (blocks.Count == 4)
            {
                for (int i = 0; i < 3; i++)
                {
                    parsedSubnetString += blocks[i] + ".";
                }
                return parsedSubnetString;
            }

            Logger.Debug(ipAddress + notAcceptedFormat);
            return null;

        }

        /// <summary>
        /// IDevice with gathered network information
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns>null if the ipAddress is not responsive</returns>
        public static async Task<IDevice> ProcessNetworkAddress(IPAddress ipAddress)
        {
            PingStopwatch.Start();
            var isResponsive = await ipAddress.IsResponsiveAsync(_pingTimeout, _maxPingRetries);
            PingStopwatch.Stop();

            if (isResponsive)
            {
                _responsiveIpAddressCounter++;

                
                var mac = GetMacAddressAsync(ipAddress).ConfigureAwait(false);
                var hostName = GetHostNameAsync(ipAddress).ConfigureAwait(false);

                var newDevice = new DeviceBase()
                {
                    Identification = new Identification()
                    {
                        IpAddress = ipAddress.ToString(),
                        PhysicalAddress = (await mac).ToString(),
                    },
                };

                newDevice.SetBrowseName((await hostName));

                return newDevice;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Searches for SubSystems in an complete Subnet. 192.168.1.1-192.168.1.255
        /// </summary>
        /// <param name="baseIp">e.x. 192.168.1.</param>
        /// <returns>List of HostDevices (with Ipaddress, Mac, ...)</returns>
        public static IEnumerable<IDevice> SearchForSubSystems(string baseIp)
        {
            baseIp = ParseForSubnetString(baseIp);
            MacStopwatch.Reset();
            HostnameStopwatch.Reset();
            PingStopwatch.Reset();

            var tasks = new List<Task<IDevice>>();
            var completedTasks = new List<Task<IDevice>>();

            foreach (IPAddress ipAddress in GetIpListFromSubnet(baseIp))
            {
                var task = ProcessNetworkAddress(ipAddress);
                task.ContinueWith(ContinuationAction, TaskContinuationOptions.OnlyOnFaulted);
                tasks.Add(task);
            }



            // Gib die fertigen Tasks vor den nicht fertigen zurück
            while (!((completedTasks.Count == tasks.Count) && !tasks.Except(completedTasks).Any()))
            {
                foreach (Task<IDevice> task in GetCompletedTasks(tasks).Except(completedTasks))
                {
                    if (!task.IsFaulted)
                    {
                        yield return task.Result;
                    }
                    completedTasks.Add(task);
                    task.Dispose();
                }
                // ReSharper disable once RedundantArgumentNameForLiteralExpression
                Task.WaitAll(Task.Delay(millisecondsDelay: 100));
            }

            Logger.Info("{0} responsive Network devices have been discovered.", _responsiveIpAddressCounter);
            //return subDevices.OrderBy(o => o.Identification.BrowseName).ToList();
        }

        private static void ContinuationAction(Task task)
        {
            //task.();
        }

        private static IEnumerable<Task<IDevice>> GetCompletedTasks(IEnumerable<Task<IDevice>> tasks)
        {
            //return tasks.Where(task => task.Status == TaskStatus.RanToCompletion || task.Status == TaskStatus.Faulted);
            return tasks.Where(task => task.Status == TaskStatus.RanToCompletion ||
                                       task.Status == TaskStatus.Faulted ||
                                       task.Status == TaskStatus.Canceled);
        }

        #endregion
    }

    public class CountingStopwatch : Stopwatch
    {
        public int RunningRequests;

        public new void Start()
        {
            RunningRequests++;
            base.Start();
        }

        /// <summary>
        /// Only stops the stopwatch if running Requests reach 0
        /// </summary>
        public new void Stop()
        {
            RunningRequests--;

            if (RunningRequests < 0)
            {
                RunningRequests = 0;
            }

            if(RunningRequests == 0)
            {
                base.Stop();
            }
        }

        /// <summary>
        /// Resets the Stopwatch and also the runnin requests
        /// </summary>
        public new void Reset()
        {
            RunningRequests = 0;
            base.Reset();
        }
    }
}