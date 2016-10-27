using Tapako.Utilities.DeviceSelector.View;

namespace Tapako.Utilities.DeviceSelector.States
{
    interface IDeviceSelectorState
    {
        DeviceSelectorView Parent { get; set; }

        void PressEnter();
        void PressTab();
        void PressUp();
        void PressDown();
        void PressEscape();
    }
}
