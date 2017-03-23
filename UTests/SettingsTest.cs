using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Client;
using Client.Classes;

namespace UTests
{
    [TestClass]
    public class SettingsTest
    {
        [TestMethod]
        public void ValidUpdateIntervalTest()
        {
            Settings.Current.AutoUpdateInterval = 4;
            Assert.AreEqual(Settings.Current.AutoUpdateInterval, (uint)4);
        }
        [TestMethod]
        public void InvalidUpdateIntervalTest()
        {
            Settings.Current.AutoUpdateInterval = 0; 
            Assert.AreNotEqual(Settings.Current.AutoUpdateInterval, 0);
        }
    }
}
