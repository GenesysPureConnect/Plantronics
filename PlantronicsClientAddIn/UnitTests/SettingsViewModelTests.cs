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

        private Moq.Mock<IStatusManager> StatusManager()
        {
            var status = new Moq.Mock<IStatusManager>();

            status.Setup(s => s.GetSettableStatuses()).Returns(new List<Status>());

            return status;
        }

        [TestMethod]
        public void TestDisconnectStatusKey()
        {
            var settings = SettingsManager();
            var status = StatusManager();

            SettingsViewModel target = new SettingsViewModel(settings.Object, status.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };

            settings.SetupGet(s => s.DisconnectStatusKey).Returns(TestKey);

            target.DisconnectStatus = new Status() { Key = TestKey }; ;

            settings.VerifySet(s => s.DisconnectStatusKey = TestKey);
            Assert.AreEqual(TestKey, target.DisconnectStatus.Key);
            Assert.AreEqual("DisconnectStatus", actual);

        }

        [TestMethod]
        public void TestDisconnectChangeStatus()
        {
            var settings = SettingsManager();
            var status = StatusManager();
            settings.SetupGet(s => s.DisconnectChangeStatus).Returns(true);

            SettingsViewModel target = new SettingsViewModel(settings.Object, status.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };

            Assert.AreEqual(true, target.DisconnectChangeStatus);
            target.DisconnectChangeStatus = false;

            settings.VerifySet(s => s.DisconnectChangeStatus = false);
          
            Assert.AreEqual("DisconnectChangeStatus", actual);
        }

        [TestMethod]
        public void TestDisconnectNotification()
        {
            var settings = SettingsManager();
            var status = StatusManager();
            settings.SetupGet(s => s.DisconnectNotification).Returns(true);

            SettingsViewModel target = new SettingsViewModel(settings.Object, status.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };

            Assert.AreEqual(true, target.DisconnectNotification);
            target.DisconnectNotification = false;

            settings.VerifySet(s => s.DisconnectNotification = false);
          
            Assert.AreEqual("DisconnectNotification", actual);
        }

        [TestMethod]
        public void TestConnectStatusKey()
        {
            var settings = SettingsManager();
            var status = StatusManager();

            SettingsViewModel target = new SettingsViewModel(settings.Object, status.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };

            settings.SetupGet(s => s.ConnectStatusKey).Returns(TestKey);

            target.ConnectStatus = new Status() { Key = TestKey }; ;

            settings.VerifySet(s => s.ConnectStatusKey = TestKey);
            Assert.AreEqual(TestKey, target.ConnectStatus.Key);
            Assert.AreEqual("ConnectStatus", actual);
        }

        [TestMethod]
        public void TestConnectChangeStatus()
        {
            var settings = SettingsManager();
            var status = StatusManager();
            settings.SetupGet(s => s.ConnectChangeStatus).Returns(true);

            SettingsViewModel target = new SettingsViewModel(settings.Object, status.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };

            Assert.AreEqual(true, target.ConnectChangeStatus);
            
            target.ConnectChangeStatus = false;

            settings.VerifySet(s => s.ConnectChangeStatus = false);
            Assert.AreEqual("ConnectChangeStatus", actual);
        }

        [TestMethod]
        public void TestConnectNotification()
        {
            var settings = SettingsManager();
            var status = StatusManager();

            settings.SetupGet(s => s.ConnectNotification).Returns(true);

            SettingsViewModel target = new SettingsViewModel(settings.Object, status.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };

            Assert.AreEqual(true, target.ConnectNotification);

            target.ConnectNotification = false;

            settings.VerifySet(s => s.ConnectNotification = false);
           
            Assert.AreEqual("ConnectNotification", actual);
        }

        [TestMethod]
        public void TestOutOfRangeStatusKey()
        {
            var settings = SettingsManager();
            var status = StatusManager();

            SettingsViewModel target = new SettingsViewModel(settings.Object, status.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };

            settings.SetupGet(s => s.OutOfRangeStatusKey).Returns(TestKey);

            target.OutOfRangeStatus =  new Status() { Key = TestKey }; ;

            settings.VerifySet(s => s.OutOfRangeStatusKey = TestKey);
            Assert.AreEqual(TestKey, target.OutOfRangeStatus.Key);
            Assert.AreEqual("OutOfRangeStatus", actual);
        }

        [TestMethod]
        public void TestOutOfRangeChangeStatus()
        {
            var settings = SettingsManager();
            var status = StatusManager();
            settings.SetupGet(s => s.OutOfRangeChangeStatus).Returns(true);

            SettingsViewModel target = new SettingsViewModel(settings.Object, status.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };

            Assert.AreEqual(true, target.OutOfRangeChangeStatus);

            target.OutOfRangeChangeStatus = false;

            settings.VerifySet(s => s.OutOfRangeChangeStatus = false);
           
            Assert.AreEqual("OutOfRangeChangeStatus", actual);
        }

        [TestMethod]
        public void TestOutOfRangeNotification()
        {
            var settings = SettingsManager();
            var status = StatusManager();
            settings.SetupGet(s => s.OutOfRangeNotification).Returns(true);

            SettingsViewModel target = new SettingsViewModel(settings.Object, status.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };

            Assert.AreEqual(true, target.OutOfRangeNotification);

            target.OutOfRangeNotification = false;

            settings.VerifySet(s => s.OutOfRangeNotification = false);
            Assert.AreEqual("OutOfRangeNotification", actual);
        }
    }
}
