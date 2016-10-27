using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;


namespace UserInterfaceTests
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für CodedUITest1
    /// </summary>
    [CodedUITest]
    public class UITest
    {
        private UIMap _map;
        public UITest()
        {
        }

        [TestInitialize]
        public void Init()
        {
            //WpfWindow uITapakoWindow = this.UIMap.UITapakoWindow;
            //if (!uITapakoWindow.Exists)
            //{
            UiMap.StartApplication();
            //}
        }

        [TestCleanup]
        public void Cleanup()
        {

        }

        [TestMethod]
        [Priority(1)]
        [Timeout(5000)]
        public void SmokeTest()
        {
            // Wählen Sie zum Generieren von Code für den Test im Kontextmenü "Code für Coded UI-Test generieren" aus, und wählen Sie eine der Menüelemente aus.
            Playback.Wait(500);
            UiMap.WindowShallBeActive();
        }

        [TestMethod]
        [Timeout(10000)]
        public void ScanForHostDevicesShallWork()
        {

            UiMap.AnalyseDefaultPeriphery();
            Playback.Wait(10000);
            UiMap.WindowShallBeActive();
        }

        [TestMethod]
        [Timeout(10000)]
        public void QuitApplicationWithAltF4()
        {
            if (UiMap.UITapakoWindow.Exists)
            {
                Keyboard.SendKeys(UiMap.UITapakoWindow, "{F4}", ModifierKeys.Alt);
                Playback.Wait(500);
            }
            UiMap.WindowShallBeInactive();

        }

        public UIMap UiMap
        {
            get
            {
                if (_map == null)
                {
                    _map = new UIMap();
                }

                return _map;
            }
        }

    }
}
