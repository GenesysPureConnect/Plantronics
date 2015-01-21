using System;
using ININ.InteractionClient.AddIn;
using Interop.Plantronics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlantronicsClientAddIn.Plantronics;

namespace UnitTests
{
    [TestClass]
    public class DeviceSettingsTests
    {
        private ICOMDevice CreateDevice()
        {
            Moq.Mock<ICOMDevice> deviceMock = new Moq.Mock<ICOMDevice>();
            deviceMock.SetupGet(s => s.InternalName).Returns("InternalName");
            deviceMock.SetupGet(s => s.ManufacturerName).Returns("ManufacturerName");
            deviceMock.SetupGet(s => s.ProductName).Returns("ProductName");
            deviceMock.SetupGet(s => s.SerialNumber).Returns("SerialNumber");
            deviceMock.SetupGet(s => s.VersionNumber).Returns(123);

            return deviceMock.Object;
        }


        [TestMethod]
        public void TestDeviceConnected()
        {
            var device = CreateDevice();
            var traceMock = new Moq.Mock<ITraceContext>();
            var target = new DeviceStatus(traceMock.Object);

            bool settingsChanged = false;
            target.SettingsChanged += delegate(object sender, EventArgs e)
            {
                settingsChanged = true;
            };


            Assert.IsFalse(target.IsConnected);

            target.DeviceConnected(device);

            Assert.IsTrue(target.IsConnected);
            Assert.IsTrue(settingsChanged);

            Assert.AreEqual(device.InternalName, target.InternalName);
            Assert.AreEqual(device.ManufacturerName, target.ManufacturerName);
            Assert.AreEqual(device.SerialNumber, target.SerialNumber);
            Assert.AreEqual(device.VersionNumber, target.VersionNumber);
            Assert.AreEqual(device.ProductName, target.ProductName);

        }

        [TestMethod]
        public void TestDeviceDisconnected()
        {
            var device = CreateDevice();
            var traceMock = new Moq.Mock<ITraceContext>();
            var target = new PlantronicsClientAddIn.Plantronics.DeviceStatus(traceMock.Object);

            target.DeviceConnected(device);

            bool settingsChanged = false;
            target.SettingsChanged += delegate(object sender, EventArgs e)
            {
                settingsChanged = true;
            };

            target.DeviceDisconnected();
            Assert.IsFalse(target.IsConnected);
            Assert.IsTrue(settingsChanged);

            Assert.AreEqual(String.Empty, target.InternalName);
            Assert.AreEqual(String.Empty, target.ManufacturerName);
            Assert.AreEqual(String.Empty, target.SerialNumber);
            Assert.AreEqual(0, target.VersionNumber);
            Assert.AreEqual(String.Empty, target.ProductName);

        }
    }
}
