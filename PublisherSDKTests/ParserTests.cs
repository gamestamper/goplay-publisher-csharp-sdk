using System;
using System.Collections.Generic;
using GoPlay.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace PublisherSDKTests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void TestJsonObjectDeserialization()
        {
            dynamic decoded = Parser.Parse("{\"a\":1}");
            Assert.AreEqual(decoded.GetType(), typeof(JObject));
            Assert.AreEqual((int)decoded["a"], 1);
        }

        [TestMethod]
        public void TestJsonArrayDeserialization()
        {
            dynamic decoded = Parser.Parse("[\"a\",1]");
            Assert.AreEqual(decoded.GetType(), typeof(JArray));
            Assert.AreEqual(decoded.Count, 2);
            Assert.AreEqual(decoded[0].ToString(), "a");
            Assert.AreEqual((int)decoded[1], 1);
        }

        [TestMethod]
        public void TestQueryStringDeserialization()
        {
            dynamic decoded = Parser.Parse("a=1&b=2");
            Assert.AreEqual(decoded.GetType(), typeof(JsonDictionary));
            Assert.AreEqual(decoded.Count, 2);
            Assert.AreEqual(decoded["a"], "1");
            Assert.AreEqual(decoded["b"], "2");
        }
    }
}
