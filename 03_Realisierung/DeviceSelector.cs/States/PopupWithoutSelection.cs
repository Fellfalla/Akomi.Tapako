using System;
using Tapako.Utilities.DeviceSelector.View;

namespace Tapako.Utilities.DeviceSelector.States
{
    class PopupWithoutSelection : IDeviceSelectorState
    {
        public DeviceSelectorView Parent { get; set; }

        public void PressEnter()
        {
            throw new NotImplementedException();
        }

        public void PressTab()
        {
            throw new NotImplementedException();
        }

        public void PressUp()
        {
            throw new NotImplementedException();
        }

        public void PressDown()
        {
            throw new NotImplementedException();
        }

        public void PressEscape()
        {
            throw new NotImplementedException();
        }
    }
}
