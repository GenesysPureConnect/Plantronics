using ININ.InteractionClient.AddIn;
using PlantronicsClientAddIn.Plantronics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlantronicsClientAddIn.Interactions
{
    public class HookSwitchSyncManager
    {
        private IInteractionManager _interactionManager = null;
        private IDeviceManager _deviceManager = null;

        public HookSwitchSyncManager(IInteractionManager interactionManager, IDeviceManager deviceManager)
        {
            _interactionManager = interactionManager;
            _deviceManager = deviceManager;
            _deviceManager.TalkButtonPressed += OnTalkButtonPressed;
        }

        private void OnTalkButtonPressed(object sender, EventArgs e)
        {
            _interactionManager.PickupOrDisconnectCall();
        }

    }
}
