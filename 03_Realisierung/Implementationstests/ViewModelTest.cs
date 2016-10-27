using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Akomi.Logger;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tapako.ViewModel;

namespace Implementationstests
{
    [TestClass]
    public class ViewModelTest
    {
        TapakoViewModel _sut = new TapakoViewModel(null);
        const string TestString = "Test";


        [TestInitialize]
        public void Init()
        {
            _sut = new TapakoViewModel(null);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _sut = null;
        }

        [TestMethod]
        [Timeout(1000)]
        public void ViewModelGetsLoggerMessageNotifications()
        {
            var messages = new List<Message>();
            Logger.NewLogMessage += (sender, msg) => messages.Add(msg);

            Logger.Info(TestString);
            

        }

        [TestMethod]
        [Timeout(1000)]
        public void ViewModelGetsLotsOfLoggerNotifications()
        {
            var messages = new List<Message>();
            Logger.NewLogMessage += (sender, msg) => messages.Add(msg);

            for (int i = 0; i < 20; i++)
            {
                Logger.Warning(TestString);
                Logger.Info(TestString);
                Logger.Error(TestString);
            }


        }


        [TestMethod]
        [Timeout(1000)]
        public void ViewModelGetsLoggerNotificationFromTask(){
            var messages = new List<Message>();
            Logger.NewLogMessage += (sender, msg) => messages.Add(msg);

            Task task = new Task(
                () =>
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Logger.Error(TestString);
                        Logger.Warning(TestString);
                        Logger.Info(TestString);
                    }
                }
                    );

            task.Start();
            task.Wait();

 
        }

        [TestMethod]
        [Timeout(1000)]
        public void ViewModelGetsLoggerNotificationFromDifferentThread(){
            var messages = new List<Message>();
            Logger.NewLogMessage += (sender, msg) => messages.Add(msg);

            ThreadStart threadStart = new ThreadStart(
                () =>
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Logger.Error(TestString);
                        Logger.Warning(TestString);
                        Logger.Info(TestString);
                    }
                    return;
                }
                    );
            Thread thread = new Thread(threadStart);
            thread.Start();
            thread.Join();

        }

    }
}
