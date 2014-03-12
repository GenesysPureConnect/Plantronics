Plantronics Client Add-In
========================

This Add-In is a scaffold example around the [Plantronics Spokes SDK][1].  The Spokes SDK provides events for plantronics headsets on windows machines.  These events are raised for when devices are connected, disconnected or when a hardware button is pressed.  The SDK will also allow you to get device information such as product id, product name, serial number, version, and other details.  

This example was tested with the [Plantronics DA45][2].  Minimal testing was done with the [Plantronics Legend UC][3] but some of the device events such as connected/disconnected were inconsistent from the DA45.

A demo video of this project in action can be seen at http://www.youtube.com/watch?v=pFQC-6TcQSo

Running The Add-In
===================
It is required that the SDK is installed in order for this Add-In to run.  Once it is installed, create a directory in Program Files (x86)\Interactive Intelligence\ICUserApps called AddIns, copy the 2 dlls to that new folder and restart the Interaction Client. 


AddIn Features
--------------
The AddIn will perform the following actions based on the Plantronics headset event

**USB Plugged In**

**Plantronics Event:** Device State Changed

**CIC Action:** Log device information to trace log and pop toast

----------
**USB Unplugged** 

**Plantronics Event:** Device State Changed

**CIC Action:** Pop Toast

----------

**Headset Connected To DA45**

**Plantronics Event:** Headset state change - In Range

**CIC Action:** Set status to available.

----------

**Headset Unplugged from DA45**

**Plantronics Event:** Headset state change - out of range

**CIC Action:** Set status to Away From Desk


----------

**Mute/Volume Buttons Pressed**

**Plantronics Event:** Button Pressed

**CIC Action:** None

----------

**Headset talk Button Pressed**

**Plantronics Event:** Button Pressed - HeadsetButton_Talk

**CIC Action:** Answer alerting calls or disconnect the currently connected call. 




  [1]: http://developer.plantronics.com/docs/DOC-1073
  [2]: http://www.plantronics.com/us/product/da45
  [3]: http://www.plantronics.com/us/product/voyager-legend-uc
