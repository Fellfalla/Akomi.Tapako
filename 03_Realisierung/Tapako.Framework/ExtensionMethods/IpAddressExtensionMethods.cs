using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Tapako.Framework.ExtensionMethods
{
    public static class IpAddressExtensionMethods
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="timeout">in seconds</param>
        /// <param name="nOfRetries"></param>
        /// <returns></returns>
        public static async Task<bool> IsResponsiveAsync(this IPAddress ipAddress, int timeout, int nOfRetries)
        {
            // Create Ping Object
            var ping = new Ping();

            while (nOfRetries > 0)
            {
                var reply = ping.SendPingAsync(ipAddress, timeout);
                var result = await reply;
                if (result.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    nOfRetries--;
                }
            }
            return false;
        }

         /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="timeout">in seconds</param>
        /// <param name="nOfRetries"></param>
        /// <returns></returns>
        public static bool IsResponsive(this IPAddress ipAddress, int timeout, int nOfRetries)
        {
            // Create Ping Object
            var ping = new Ping();

            while (nOfRetries > 0)
            {
                var reply = ping.Send(ipAddress, timeout);
                if (reply != null && reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    nOfRetries--;
                }
            }
            return false;
        }
    }
}
