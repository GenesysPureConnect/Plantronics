using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlantronicsClientAddIn.Settings;

namespace UnitTests
{
    [TestClass]
    public class SettingsManagerTests
    {
        const String testKey = "TEST_KEY_BLAH";

        [TestMethod]
        public void TestLoadFile()
        {
            SettingsManager target = new SettingsManager();

            Assert.IsFalse(String.IsNullOrEmpty(target.FilePath));

        }

        [TestMethod]
        public void TestDisconnectStatusKey()
        {
            SettingsManager target = new SettingsManager();
            target.DeviceDisconnectStatusKey = testKey;

            target = new SettingsManager();

            Assert.AreEqual(testKey, target.DeviceDisconnectStatusKey);
        }

        [TestMethod]
        public void TestDisconnectChangeStatus()
        {
            SettingsManager target = new SettingsManager();
            target.DeviceDisconnectChangeStatus = true;

            target = new SettingsManager();
            Assert.AreEqual(true, target.DeviceDisconnectChangeStatus);
            target.DeviceDisconnectChangeStatus = false;

            target = new SettingsManager();
            Assert.AreEqual(false, target.DeviceDisconnectChangeStatus);
        }

         [TestMethod]
        public void TestDisconnectNotification()
        {
            SettingsManager target = new SettingsManager();
            target.DeviceDisconnectNotification = true;

            target = new SettingsManager();
            Assert.AreEqual(true, target.DeviceDisconnectNotification);
            target.DeviceDisconnectNotification = false;

            target = new SettingsManager();
            Assert.AreEqual(false, target.DeviceDisconnectNotification);
        }

         [TestMethod]
         public void TestConnectStatusKey()
         {
             SettingsManager target = new SettingsManager();
             target.DeviceConnectStatusKey = testKey;

             target = new SettingsManager();

             Assert.AreEqual(testKey, target.DeviceConnectStatusKey);
         }

         [TestMethod]
         public void TestConnectChangeStatus()
         {
             SettingsManager target = new SettingsManager();
             target.DeviceConnectChangeStatus = true;

             target = new SettingsManager();
             Assert.AreEqual(true, target.DeviceConnectChangeStatus);
             target.DeviceConnectChangeStatus = false;

             target = new SettingsManager();
             Assert.AreEqual(false, target.DeviceConnectChangeStatus);
         }

         [TestMethod]
         public void TestConnectNotification()
         {
             SettingsManager target = new SettingsManager();
             target.DeviceConnectNotification = true;

             target = new SettingsManager();
             Assert.AreEqual(true, target.DeviceConnectNotification);
             target.DeviceConnectNotification = false;

             target = new SettingsManager();
             Assert.AreEqual(false, target.DeviceConnectNotification);
         }

         [TestMethod]
         public void TestOutOfRangeStatusKey()
         {
             SettingsManager target = new SettingsManager();
             target.HeadsetDisconnectStatusKey = testKey;

             target = new SettingsManager();

             Assert.AreEqual(testKey, target.HeadsetDisconnectStatusKey);
         }

         [TestMethod]
         public void TestOutOfRangeChangeStatus()
         {
             SettingsManager target = new SettingsManager();
             target.HeadsetDisconnectChangeStatus = true;

             target = new SettingsManager();
             Assert.AreEqual(true, target.HeadsetDisconnectChangeStatus);
             target.HeadsetDisconnectChangeStatus = false;

             target = new SettingsManager();
             Assert.AreEqual(false, target.HeadsetDisconnectChangeStatus);
         }

         [TestMethod]
         public void TestHeadsetDisonnectNotification()
         {
             SettingsManager target = new SettingsManager();
             target.HeadsetDisconnectNotification = true;

             target = new SettingsManager();
             Assert.AreEqual(true, target.HeadsetDisconnectNotification);
             target.HeadsetDisconnectNotification = false;

             target = new SettingsManager();
             Assert.AreEqual(false, target.HeadsetDisconnectNotification);
         }

         [TestMethod]
         public void TestInRangeStatusKey()
         {
             SettingsManager target = new SettingsManager();
             target.HeadsetConnectStatusKey = testKey;

             target = new SettingsManager();

             Assert.AreEqual(testKey, target.HeadsetConnectStatusKey);
         }

         [TestMethod]
         public void TestInRangeChangeStatus()
         {
             SettingsManager target = new SettingsManager();
             target.HeadsetConnectChangeStatus = true;

             target = new SettingsManager();
             Assert.AreEqual(true, target.HeadsetConnectChangeStatus);
             target.HeadsetConnectChangeStatus = false;

             target = new SettingsManager();
             Assert.AreEqual(false, target.HeadsetConnectChangeStatus);
         }

         [TestMethod]
         public void TestInRangeNotification()
         {
             SettingsManager target = new SettingsManager();
             target.HeadsetConnectNotification = true;

             target = new SettingsManager();
             Assert.AreEqual(true, target.HeadsetConnectNotification);
             target.HeadsetConnectNotification = false;

             target = new SettingsManager();
             Assert.AreEqual(false, target.HeadsetConnectNotification);
         }
    }
}
