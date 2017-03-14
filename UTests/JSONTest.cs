using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Client;
using Newtonsoft.Json;

namespace UTests
{
    [TestClass]
    public class JSONTest
    {
        [TestMethod]
        public void Serialization_Test()
        {
            var p = new Property( -1,"qwe",  "asd", (int)TypeCode.Boolean, -1, null, "false" );
            var json = JsonConvert.SerializeObject(p);
            Assert.AreEqual(json, "{\"id\":\"-1\",\"value\":\"false\",\"type\":3,\"objectId\":\"zxc\",\"scripts\":[],\"id\":\"qwe\",\"name\":\"asd\"}");
        }
        [TestMethod]
        public void Deserialization_Test()
        {
            var p = new Property(-1, "asd", "asd", (int)TypeCode.Boolean, -1, null, "false");
            var json = JsonConvert.DeserializeObject<Property>("{\"Scripts\":[],\"ObjectId\":\"zxc\",\"Id\":\"asd\",\"Name\":\"asd\",\"Value\":\"false\",\"Type\":3}");
            Assert.IsTrue(p.pathUnit == json.pathUnit && p.name == json.name && p.objectId == json.objectId && p.type == json.type && p.value == json.value);
        }

    }
}
