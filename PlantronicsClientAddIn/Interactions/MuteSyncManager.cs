using ININ.InteractionClient.AddIn;
using PlantronicsClientAddIn.Plantronics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlantronicsClientAddIn.Interactions
{
    
    public class MuteSyncManager 
    {
        private IInteractionSelector _interactionSelector = null;
        private IDeviceManager _deviceManager = null;

        public MuteSyncManager(IInteractionSelector interactionSelector, IDeviceManager deviceManager)
        {
            _interactionSelector = interactionSelector;
            _interactionSelector.SelectedInteractionChanged += OnSelectedInteractionChanged;
            _deviceManager = deviceManager;
        }

        private void OnSelectedInteractionChanged(object sender, EventArgs e)
        {
            if (_interactionSelector.SelectedInteraction == null)
            {
                return;
            }

            var callIsMuted = _interactionSelector.SelectedInteraction.GetAttribute("Eic_Muted") == "1";
            var deviceIsMuted = _deviceManager.IsHeadsetMuted;

            if (callIsMuted != deviceIsMuted)
            {
                _deviceManager.ToggleMute();
            }
        }
    }
    
}
