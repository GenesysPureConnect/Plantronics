The Interactive Intelligence Plantronics provides integration features to the Plantronics Spokes SDK and Plantronics headsets that support the Spokes SDK.  The AddIns were tested with the DA80 and DA45 headsets using the SDK version 3.5.  The agent addin can report headset status to a web service so that supervisors can see what devices are in use by their agents.  The web service is in a different repo at https://github.com/InteractiveIntelligence/PlantronicsHeadsetStatusWebService


AddIns
======
 
Interaction Desktop Agent AddIn
--------------------------------
 - Client view with Plantronics Headset device information
	 - Product Name
	 - Manufacturer Name
	 - Serial Number
	 - Version Number
	 - Device Mute State
 - Toast Notifications
	 - When headset is connected/disconnected
	 - When device is connected/disconnected
 - Automatically change status
	 - When headset is connected/disconnected
	 - When device is connected/disconnected
 - Automatically log out of CIC
  	 - When headset is disconnected
	 - When device is disconnected
 - Full configuration for status changes and notifications.
 - Device talk button operations
	 - Pick up ringing or held calls

Interaction Desktop Supervisor AddIn
--------------------------------
The Interaction Desktop Supervisor AddIn provides a view for supervisors to see their agents and the headsets that the agents are using.


Installing
==========
The install can be found on the Interactive Intelligence Marketplace marketplace.inin.com or can be build from the source in this repo. The Plantronics Hub v3.5 http://www.plantronics.com/us/product/plantronics-hub-desktop and the Interaction Desktop Client must be installed on the machine before running this install. 

