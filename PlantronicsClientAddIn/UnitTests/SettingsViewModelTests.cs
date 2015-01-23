using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlantronicsClientAddIn.Settings;
using PlantronicsClientAddIn.Status;
using PlantronicsClientAddIn.ViewModels;

namespace UnitTests
{
    [TestClass]
    public class SettingsViewModelTests
    {
        const string TestKey = "TEST_KEY";

        private Moq.Mock<ISettingsManager> SettingsManager()
        {
            var settings = new Moq.Mock<ISettingsManager>();
            return settings;

        }

        private Moq.Mock<ICicStatusService> StatusManager()
        {
            var status = new Moq.Mock<ICicStatusService>();

            status.Setup(s => s.GetSettableStatuses()).Returns(new List<Status>());

            return status;
        }

        [TestMethod]
        public void TestDeviceDisconnectStatusKey()
        {
            var settings = SettingsManager();
            var status = StatusManager();

            SettingsViewModel target = new SettingsViewModel(settings.Object, status.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };

            settings.SetupGet(s => s.DeviceDisconnectStatusKey).Returns(TestKey);

            target.DeviceDisconnectStatus = new Status() { Key = TestKey }; ;

            settings.VerifySet(s => s.DeviceDisconnectStatusKey = TestKey);
            Assert.AreEqual(TestKey, target.DeviceDisconnectStatus.Key);
            Assert.AreEqual("DeviceDisconnectStatus", actual);

        }

        [TestMethod]
        public void TestDeviceDisconnectChangeStatus()
        {
            var settings = SettingsManager();
            var status = StatusManager();
            settings.SetupGet(s => s.DeviceDisconnectChangeStatus).Returns(true);

            SettingsViewModel target = new SettingsViewModel(settings.Object, status.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };

            Assert.AreEqual(true, target.DeviceDisconnectChangeStatus);
            target.DeviceDisconnectChangeStatus = false;

            settings.VerifySet(s => s.DeviceDisconnectChangeStatus = false);

            Assert.AreEqual("DeviceDisconnectChangeStatus", actual);
        }

        [TestMethod]
        public void TestDeviceDisconnectNotification()
        {
            var settings = SettingsManager();
            var status = StatusManager();
            settings.SetupGet(s => s.DeviceDisconnectNotification).Returns(true);

            SettingsViewModel target = new SettingsViewModel(settings.Object, status.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };

            Assert.AreEqual(true, target.DeviceDisconnectNotification);
            target.DeviceDisconnectNotification = false;

            settings.VerifySet(s => s.DeviceDisconnectNotification = false);

            Assert.AreEqual("DeviceDisconnectNotification", actual);
        }

        [TestMethod]
        public void TestDeviceConnectStatus()
        {
            var settings = SettingsManager();
            var status = StatusManager();

            SettingsViewModel target = new SettingsViewModel(settings.Object, status.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };

            settings.SetupGet(s => s.DeviceConnectStatusKey).Returns(TestKey);

            target.DeviceConnectStatus = new Status() { Key = TestKey }; ;

            settings.VerifySet(s => s.DeviceConnectStatusKey = TestKey);
            Assert.AreEqual(TestKey, target.DeviceConnectStatus.Key);
            Assert.AreEqual("DeviceConnectStatus", actual);
        }

        [TestMethod]
        public void TestDeviceConnectChangeStatus()
        {
            var settings = SettingsManager();
            var status = StatusManager();
            settings.SetupGet(s => s.DeviceConnectChangeStatus).Returns(true);

            SettingsViewModel target = new SettingsViewModel(settings.Object, status.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };

            Assert.AreEqual(true, target.DeviceConnectChangeStatus);

            target.DeviceConnectChangeStatus = false;

            settings.VerifySet(s => s.DeviceConnectChangeStatus = false);
            Assert.AreEqual("DeviceConnectChangeStatus", actual);
        }

        [TestMethod]
        public void TestDeviceConnectNotification()
        {
            var settings = SettingsManager();
            var status = StatusManager();

            settings.SetupGet(s => s.DeviceConnectNotification).Returns(true);

            SettingsViewModel target = new SettingsViewModel(settings.Object, status.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };

            Assert.AreEqual(true, target.DeviceConnectNotification);

            target.DeviceConnectNotification = false;

            settings.VerifySet(s => s.DeviceConnectNotification = false);

            Assert.AreEqual("DeviceConnectNotification", actual);
        }

      

        [TestMethod]
        public void TestHeadsetDisconnectChangeStatus()
        {
            var settings = SettingsManager();
            var status = StatusManager();
            settings.SetupGet(s => s.HeadsetDisconnectChangeStatus).Returns(true);

            SettingsViewModel target = new SettingsViewModel(settings.Object, status.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };

            Assert.AreEqual(true, target.HeadsetDisconnectChangeStatus);

            target.HeadsetDisconnectChangeStatus = false;

            settings.VerifySet(s => s.HeadsetDisconnectChangeStatus = false);

            Assert.AreEqual("HeadsetDisconnectChangeStatus", actual);
        }

        [TestMethod]
        public void TestHeadsetDisconnectNotification()
        {
            var settings = SettingsManager();
            var status = StatusManager();
            settings.SetupGet(s => s.HeadsetDisconnectNotification).Returns(true);

            SettingsViewModel target = new SettingsViewModel(settings.Object, status.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };

            Assert.AreEqual(true, target.HeadsetDisconnectNotification);

            target.HeadsetDisconnectNotification = false;

            settings.VerifySet(s => s.HeadsetDisconnectNotification = false);
            Assert.AreEqual("HeadsetDisconnectNotification", actual);
        }
    }
}
