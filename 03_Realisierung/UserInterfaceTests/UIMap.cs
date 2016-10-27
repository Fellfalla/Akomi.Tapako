namespace UserInterfaceTests
{
    using Microsoft.VisualStudio.TestTools.UITesting.WinControls;
    using System.CodeDom.Compiler;
    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
    using Mouse = Microsoft.VisualStudio.TestTools.UITesting.Mouse;
    using System.Drawing;
    using System.Windows.Input;


    public partial class UIMap
    {

        /// <summary>
        /// CloseUnifiedAutomationPopup - Verwenden Sie "CloseUnifiedAutomationPopupParams", um Parameter an diese Methode zu übergeben.
        /// </summary>
        public void CloseUnifiedAutomationPopup()
        {
            #region Variable Declarations
            WinTitleBar uIAboutTitleBar = UIAboutWindow.UIAboutTitleBar;
            WinButton uICloseButton = UIAboutWindow.UICloseWindow.UICloseButton;
            #endregion

            // Klicken "About" Titelleiste
            Mouse.Click(uIAboutTitleBar, new Point(442, 17));

            // "{Enter}" in "Close" Schaltfläche eingeben
            Keyboard.SendKeys(uICloseButton, CloseUnifiedAutomationPopupParams.UICloseButtonSendKeys, ModifierKeys.None);
        }

        public virtual CloseUnifiedAutomationPopupParams CloseUnifiedAutomationPopupParams
        {
            get
            {
                if ((mCloseUnifiedAutomationPopupParams == null))
                {
                    mCloseUnifiedAutomationPopupParams = new CloseUnifiedAutomationPopupParams();
                }
                return mCloseUnifiedAutomationPopupParams;
            }
        }

        private CloseUnifiedAutomationPopupParams mCloseUnifiedAutomationPopupParams;
    }
    /// <summary>
    /// An "CloseUnifiedAutomationPopup" zu übergebende Parameter
    /// </summary>
    [GeneratedCode("Coded UI-Test-Generator", "12.0.31101.0")]
    public class CloseUnifiedAutomationPopupParams
    {

        #region Fields
        /// <summary>
        /// "{Enter}" in "Close" Schaltfläche eingeben
        /// </summary>
        public string UICloseButtonSendKeys = "{Enter}";
        #endregion
}
}
