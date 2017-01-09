using System;
using Akomi.Logger;
using ExtensionMethodsCollection;
using Tapako.ObjectMerger;

namespace Tapako.DeviceInformationManagement
{
    internal class ObjectMergerLoggerAdapter : IObjectMergerLogger
    {
        public void Debug(string message, params object[] paramList)
        {
            Logger.Debug(message, paramList);
        }

        public void Warning(string message, params object[] paramList)
        {
            Logger.Warning(message, paramList);
        }

        public void Error(string message, params object[] paramList)
        {
            Logger.Error(message, paramList);
        }

        public void Info(string message, params object[] paramList)
        {
            Logger.Info(message, paramList);
        }

        public void Debug(Exception exception)
        {
            Logger.Debug(exception.ToString(true));
        }

        public void Warning(Exception exception)
        {
            Logger.Warning(exception.ToString(true));
        }

        public void Error(Exception exception)
        {
            Logger.Error(exception.ToString(true));
        }

        public void Info(Exception exception)
        {
            Logger.Info(exception.ToString(true));
        }
    }
}