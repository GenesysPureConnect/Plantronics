using System;
using System.ComponentModel;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlantronicsClientAddIn.Plantronics;
using PlantronicsClientAddIn.ViewModels;

namespace UnitTests
{
    [TestClass]
    public class StatusViewModelTests
    {
        const string TestValue = "VALUE";
        const string TestValue2 = "VALUE2";

        [TestMethod]
        public void IsConnectedTest()
        {
            var statusMock = new Moq.Mock<IDeviceManager>();
            statusMock.SetupGet(s => s.IsDeviceConnected).Returns(false);

            StatusViewModel target = new StatusViewModel(statusMock.Object);

            Assert.AreEqual(Visibility.Hidden, target.ConnectionVisibility);
            
            Assert.AreEqual(Visibility.Visible, target.ErrorVisibility);
        }

        [TestMethod]
        public void InternalNameTest()
        {
            var statusMock = new Moq.Mock<IDeviceManager>();
            statusMock.SetupGet(s => s.InternalName).Returns(TestValue);

            StatusViewModel target = new StatusViewModel(statusMock.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };


            Assert.AreEqual(TestValue, target.InternalName);
            target.InternalName = TestValue2;

            Assert.AreEqual(TestValue2, target.InternalName);
            Assert.AreEqual("InternalName", actual);
        }

        [TestMethod]
        public void ManufacturerNameTest()
        {
            var statusMock = new Moq.Mock<IDeviceManager>();
            statusMock.SetupGet(s => s.ManufacturerName).Returns(TestValue);

            StatusViewModel target = new StatusViewModel(statusMock.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };


            Assert.AreEqual(TestValue, target.ManufacturerName);
            target.ManufacturerName = TestValue2;

            Assert.AreEqual(TestValue2, target.ManufacturerName);
            Assert.AreEqual("ManufacturerName", actual);
        }

        [TestMethod]
        public void ProductNameTest()
        {
            var statusMock = new Moq.Mock<IDeviceManager>();
            statusMock.SetupGet(s => s.ProductName).Returns(TestValue);

            StatusViewModel target = new StatusViewModel(statusMock.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };


            Assert.AreEqual(TestValue, target.ProductName);
            target.ProductName = TestValue2;

            Assert.AreEqual(TestValue2, target.ProductName);
            Assert.AreEqual("ProductName", actual);
        }

        [TestMethod]
        public void SerialNumberTest()
        {
            var statusMock = new Moq.Mock<IDeviceManager>();
            statusMock.SetupGet(s => s.SerialNumber).Returns(TestValue);

            StatusViewModel target = new StatusViewModel(statusMock.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };


            Assert.AreEqual(TestValue, target.SerialNumber);
            target.SerialNumber = TestValue2;

            Assert.AreEqual(TestValue2, target.SerialNumber);
            Assert.AreEqual("SerialNumber", actual);
        }

        [TestMethod]
        public void VersionNumberTest()
        {
            var statusMock = new Moq.Mock<IDeviceManager>();
            statusMock.SetupGet(s => s.VersionNumber).Returns(0);

            StatusViewModel target = new StatusViewModel(statusMock.Object);

            string actual = null;

            target.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                actual = e.PropertyName;
            };


            Assert.AreEqual(0, target.VersionNumber);
            target.VersionNumber = 1;

            Assert.AreEqual(1, target.VersionNumber);
            Assert.AreEqual("VersionNumber", actual);
        }
    }
}
