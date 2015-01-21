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
            target.DisconnectStatusKey = testKey;

            target = new SettingsManager();

            Assert.AreEqual(testKey, target.DisconnectStatusKey);
        }

        [TestMethod]
        public void TestDisconnectChangeStatus()
        {
            SettingsManager target = new SettingsManager();
            target.DisconnectChangeStatus = true;

            target = new SettingsManager();
            Assert.AreEqual(true, target.DisconnectChangeStatus);
            target.DisconnectChangeStatus = false;

            target = new SettingsManager();
            Assert.AreEqual(false, target.DisconnectChangeStatus);
        }

         [TestMethod]
        public void TestDisconnectNotification()
        {
            SettingsManager target = new SettingsManager();
            target.DisconnectNotification = true;

            target = new SettingsManager();
            Assert.AreEqual(true, target.DisconnectNotification);
            target.DisconnectNotification = false;

            target = new SettingsManager();
            Assert.AreEqual(false, target.DisconnectNotification);
        }

         [TestMethod]
         public void TestConnectStatusKey()
         {
             SettingsManager target = new SettingsManager();
             target.ConnectStatusKey = testKey;

             target = new SettingsManager();

             Assert.AreEqual(testKey, target.ConnectStatusKey);
         }

         [TestMethod]
         public void TestConnectChangeStatus()
         {
             SettingsManager target = new SettingsManager();
             target.ConnectChangeStatus = true;

             target = new SettingsManager();
             Assert.AreEqual(true, target.ConnectChangeStatus);
             target.ConnectChangeStatus = false;

             target = new SettingsManager();
             Assert.AreEqual(false, target.ConnectChangeStatus);
         }

         [TestMethod]
         public void TestConnectNotification()
         {
             SettingsManager target = new SettingsManager();
             target.ConnectNotification = true;

             target = new SettingsManager();
             Assert.AreEqual(true, target.ConnectNotification);
             target.ConnectNotification = false;

             target = new SettingsManager();
             Assert.AreEqual(false, target.ConnectNotification);
         }

         [TestMethod]
         public void TestOutOfRangeStatusKey()
         {
             SettingsManager target = new SettingsManager();
             target.OutOfRangeStatusKey = testKey;

             target = new SettingsManager();

             Assert.AreEqual(testKey, target.OutOfRangeStatusKey);
         }

         [TestMethod]
         public void TestOutOfRangeChangeStatus()
         {
             SettingsManager target = new SettingsManager();
             target.OutOfRangeChangeStatus = true;

             target = new SettingsManager();
             Assert.AreEqual(true, target.OutOfRangeChangeStatus);
             target.OutOfRangeChangeStatus = false;

             target = new SettingsManager();
             Assert.AreEqual(false, target.OutOfRangeChangeStatus);
         }

         [TestMethod]
         public void TestOutOfRangeNotification()
         {
             SettingsManager target = new SettingsManager();
             target.OutOfRangeNotification = true;

             target = new SettingsManager();
             Assert.AreEqual(true, target.OutOfRangeNotification);
             target.OutOfRangeNotification = false;

             target = new SettingsManager();
             Assert.AreEqual(false, target.OutOfRangeNotification);
         }

         [TestMethod]
         public void TestInRangeStatusKey()
         {
             SettingsManager target = new SettingsManager();
             target.InRangeStatusKey = testKey;

             target = new SettingsManager();

             Assert.AreEqual(testKey, target.InRangeStatusKey);
         }

         [TestMethod]
         public void TestInRangeChangeStatus()
         {
             SettingsManager target = new SettingsManager();
             target.InRangeChangeStatus = true;

             target = new SettingsManager();
             Assert.AreEqual(true, target.InRangeChangeStatus);
             target.InRangeChangeStatus = false;

             target = new SettingsManager();
             Assert.AreEqual(false, target.InRangeChangeStatus);
         }

         [TestMethod]
         public void TestInRangeNotification()
         {
             SettingsManager target = new SettingsManager();
             target.InRangeNotification = true;

             target = new SettingsManager();
             Assert.AreEqual(true, target.InRangeNotification);
             target.InRangeNotification = false;

             target = new SettingsManager();
             Assert.AreEqual(false, target.InRangeNotification);
         }
    }
}
