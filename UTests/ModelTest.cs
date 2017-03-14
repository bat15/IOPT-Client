using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Client;

namespace UTests
{
    [TestClass]
    public class IdTests
    {
        [TestMethod]
        public void ScriptId()
        {
            var script = new Script(-1, "Pirozhok","Пирожок","asdwq",0);
            Assert.AreEqual(script.pathUnit,"Pirozhok");
        }
        [TestMethod]
        public void PropertyId()
        {
            var Property = new Property(-1, "Pirozhok", "Пирожок", (int)TypeCode.Decimal,0,null, "asdwq");
            Assert.AreEqual(Property.pathUnit, "Pirozhok");
        }
        [TestMethod]
        public void ObjectId()
        {
            var Object = new Client.Object(-1, "Pirozhok","Пирожок",-1,null);
            Assert.AreEqual<string>(Object.pathUnit, "Pirozhok");
        }
        [TestMethod]
        public void ModelId()
        {
            var Model = new Model(-1, "Pirozhok","Пирожок",null);
            Assert.AreEqual(Model.pathUnit, "Pirozhok");
        }
    }

    [TestClass]
    public class PropertyValues
    {
        [TestMethod]
        public void ValidDouble()
        {
            var Property = new Property(-1, "Pirozhok", "Пирожок", (int)TypeCode.Double, 0, null, "11.0");
            Assert.AreEqual(Property.value, "11,0");
        }
        [TestMethod]
        public void InvalidDouble()
        {
            var Property = new Property(-1, "Pirozhok", "Пирожок", (int)TypeCode.Double, 0, null, "asd11.0");
            Assert.AreNotEqual(Property.value, "asd11,0");
        }
        [TestMethod]
        public void ValidInt()
        {
            var Property = new Property(-1, "Pirozhok", "Пирожок", (int)TypeCode.Int32, 0, null, "11");
            Assert.AreEqual(Property.value, "11");
        }
        [TestMethod]
        public void InvalidInt()
        {
            var Property = new Property(-1, "Pirozhok", "Пирожок", (int)TypeCode.Int32, 0, null, "11.0");
            Assert.AreNotEqual(Property.value, "11,0");
        }
        [TestMethod]
        public void ValidBool()
        {
            var Property = new Property(-1, "Pirozhok", "Пирожок", (int)TypeCode.Boolean, 0, null, "False");
            Assert.AreEqual(Property.value, "false");
        }
        [TestMethod]
        public void InvalidBool()
        {
            var Property = new Property(-1, "Pirozhok", "Пирожок", (int)TypeCode.Boolean, 0, null, "11.0");
            Assert.AreNotEqual(Property.value, "11.0");
        }
    }


}
