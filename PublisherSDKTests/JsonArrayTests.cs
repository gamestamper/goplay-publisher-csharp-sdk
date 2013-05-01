using System;
using GoPlay.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PublisherSDKTests
{
    [TestClass]
    public class JsonArrayTests
    {
        [TestMethod]
        public void TestArrayLoadsValueFromString()
        {
            JsonArray d = new JsonArray("[\"a\",\"b\",{\"a\":1,\"b\":2}]");
            Assert.AreEqual("a", d[0]);
            Assert.AreEqual("b", d[1]);
            Assert.IsInstanceOfType(d[2], typeof(JsonDictionary));

            Assert.AreEqual("[\"a\",\"b\",{\"a\":1,\"b\":2}]", d.ToString());
        }
    }
}
