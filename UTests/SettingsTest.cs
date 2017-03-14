using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Client;

namespace UTests
{
    [TestClass]
    public class SettingsTest
    {
        [TestMethod]
        public void ValidUpdateIntervalTest()
        {
            Settings.Get().AutoUpdateInterval = 4;
            Assert.AreEqual(Settings.Get().AutoUpdateInterval, (uint)4);
        }
        [TestMethod]
        public void InvalidUpdateIntervalTest()
        {
            Settings.Get().AutoUpdateInterval = 0; 
            Assert.AreNotEqual(Settings.Get().AutoUpdateInterval, 0);
        }
    }
}
