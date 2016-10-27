using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Tapako.Startup
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //AppDomain.CurrentDomain.AppendPrivatePath(@"bin\DLLs");

            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;

            new Bootstrapper().Run();   // Der Bootstrapper übernimmt das Laden des Programms

        }

        /// <summary>
        /// Saves the unhandle exception in the directory of Tapako.exe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="unhandledExceptionEventArgs"></param>
        private void LogUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            string message = string.Format("Sender: {0}", sender);

            Exception exception = unhandledExceptionEventArgs.ExceptionObject as Exception;

            while (exception != null)
            {
                message += exception;
                exception = exception.InnerException;
            }

            string executableDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            if (executableDirectory == null)
            {
                executableDirectory = string.Empty;
            }

            string folderName = "Logs";

            string directory = Path.Combine(executableDirectory, folderName);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string filename = string.Format("{0} UnhandledException.log", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
            
            string filepath = Path.Combine(directory, filename);

            File.WriteAllText(filepath, message);

            MessageBox.Show(string.Format("Programm has been terminated due to following Problem: {0}. This Message will be stored at the programs executable path.",message), "Fully Tapako termination", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }
}
