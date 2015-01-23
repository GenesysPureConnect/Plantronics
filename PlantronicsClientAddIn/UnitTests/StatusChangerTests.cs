using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlantronicsClientAddIn.Status;
using ININ.InteractionClient.AddIn;
using PlantronicsClientAddIn.Plantronics;
using PlantronicsClientAddIn.Settings;

namespace UnitTests
{
    [TestClass]
    public class StatusChangerTests
    {

        const string STATUS_KEY = "STATUS_KEY";

        [TestMethod]
        public void TestHeadsetDisconnected()
        {
            var statusMock = new Moq.Mock<ICicStatusService>();
            var deviceManagerMock = new Moq.Mock<IDeviceManager>();
            var settingsManagerMock = new Moq.Mock<ISettingsManager>();

            settingsManagerMock.SetupGet(s => s.HeadsetDisconnectStatusKey).Returns(STATUS_KEY);
            settingsManagerMock.SetupGet(s => s.HeadsetDisconnectChangeStatus).Returns(true);

            var target = new StatusChanger(statusMock.Object, deviceManagerMock.Object, settingsManagerMock.Object);

            deviceManagerMock.Raise(d => d.HeadsetDisconnected += null, new Plantronics.UC.SpokesWrapper.ConnectedStateArgs(true, true));

            statusMock.Verify(cic => cic.SetStatus(STATUS_KEY));
        }

        [TestMethod]
        public void TestHeadsetConnected()
        {
            var statusMock = new Moq.Mock<ICicStatusService>();
            var deviceManagerMock = new Moq.Mock<IDeviceManager>();
            var settingsManagerMock = new Moq.Mock<ISettingsManager>();

            settingsManagerMock.SetupGet(s => s.HeadsetConnectStatusKey).Returns(STATUS_KEY);
            settingsManagerMock.SetupGet(s => s.HeadsetConnectChangeStatus).Returns(true);

            var target = new StatusChanger(statusMock.Object, deviceManagerMock.Object, settingsManagerMock.Object);

            deviceManagerMock.Raise(d => d.HeadsetConnected += null, new Plantronics.UC.SpokesWrapper.ConnectedStateArgs(true, true));

            statusMock.Verify(cic => cic.SetStatus(STATUS_KEY));
        }

        [TestMethod]
        public void TestDeviceDisconnected()
        {
            var statusMock = new Moq.Mock<ICicStatusService>();
            var deviceManagerMock = new Moq.Mock<IDeviceManager>();
            var settingsManagerMock = new Moq.Mock<ISettingsManager>();

            settingsManagerMock.SetupGet(s => s.DeviceDisconnectStatusKey).Returns(STATUS_KEY);
            settingsManagerMock.SetupGet(s => s.DeviceDisconnectChangeStatus).Returns(true);

            var target = new StatusChanger(statusMock.Object, deviceManagerMock.Object, settingsManagerMock.Object);

            deviceManagerMock.Raise(d => d.PlantronicsDeviceDetached += null, new Plantronics.UC.SpokesWrapper.AttachedArgs(null));

            statusMock.Verify(cic => cic.SetStatus(STATUS_KEY));
        }

        [TestMethod]
        public void TestDeviceConnected()
        {
            var statusMock = new Moq.Mock<ICicStatusService>();
            var deviceManagerMock = new Moq.Mock<IDeviceManager>();
            var settingsManagerMock = new Moq.Mock<ISettingsManager>();

            settingsManagerMock.SetupGet(s => s.DeviceConnectStatusKey).Returns(STATUS_KEY);
            settingsManagerMock.SetupGet(s => s.DeviceConnectChangeStatus).Returns(true);

            var target = new StatusChanger(statusMock.Object, deviceManagerMock.Object, settingsManagerMock.Object);

            deviceManagerMock.Raise(d => d.PlantronicsDeviceAttached += null, new Plantronics.UC.SpokesWrapper.AttachedArgs(null));

            statusMock.Verify(cic => cic.SetStatus(STATUS_KEY));
        }


    }
}
