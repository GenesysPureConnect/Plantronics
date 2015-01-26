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
        private DateTime _lastSwitchChange = new DateTime();

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
            //Getting an issue where when we press the button, we get two events right
            //in a row, trying to prevent that by requiring a min time between presses
            if (DateTime.Now - _lastSwitchChange > TimeSpan.FromSeconds(1))
            {
                _interactionManager.PickupOrDisconnectCall();
            }

            _lastSwitchChange = DateTime.Now;
        }

    }
}
