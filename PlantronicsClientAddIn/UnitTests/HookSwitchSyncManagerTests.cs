using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlantronicsClientAddIn.Interactions;
using PlantronicsClientAddIn.Plantronics;

namespace UnitTests
{
    [TestClass]
    public class HookSwitchSyncManagerTests
    {
        [TestMethod]
        public void TestButtonPressed()
        {
            Moq.Mock<IInteractionManager> interactionManagerMock = new Moq.Mock<IInteractionManager>();
            Moq.Mock<IDeviceManager> deviceManagerMock = new Moq.Mock<IDeviceManager>();

            var target = new HookSwitchSyncManager(interactionManagerMock.Object, deviceManagerMock.Object);

            deviceManagerMock.Raise(d => d.TalkButtonPressed += null, EventArgs.Empty);

            interactionManagerMock.Verify(i => i.PickupOrDisconnectCall());

        }
    }
}
