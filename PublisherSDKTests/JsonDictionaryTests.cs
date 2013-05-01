using System;
using System.Collections;
using System.Collections.Generic;
using GoPlay;
using GoPlay.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace PublisherSDKTests
{
    [TestClass]
    public class JsonDictionaryTests
    {
        [TestMethod]
        public void TestDictionaryLoadsValueFromString()
        {
            JsonDictionary d = new JsonDictionary("{\"a\":1,\"b\":2,\"c\":[\"a\",\"b\"]}");
            Assert.AreEqual(1L, d["a"]);
            Assert.AreEqual(2L, d["b"]);
            Assert.IsInstanceOfType(d["c"], typeof(JsonArray));

            Assert.AreEqual("{\"a\":1,\"b\":2,\"c\":[\"a\",\"b\"]}", d.ToString());
        }
    }
}
