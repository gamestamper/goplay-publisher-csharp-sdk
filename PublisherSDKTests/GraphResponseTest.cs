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
    public class GraphResponseTest
    {
        [TestMethod]
        public void TestInstantiationWithArray()
        {
            JArray data = JArray.Parse("[\"a\",\"b\"]");
            GraphResponse response = new GraphResponse(data, new GraphRequest("endpoint", null));

            Assert.AreEqual(typeof(JsonArray), response.Data.GetType());
            Assert.AreEqual("a", response.Data[0]);
            Assert.AreEqual("b", response.Data[1]);

            Assert.AreEqual(typeof(JsonArray), response.Response.GetType());
            Assert.AreEqual("a", response.Response[0]);
            Assert.AreEqual("b", response.Response[1]);
        }

        [TestMethod]
        public void TestInstantiationWithDataObject()
        {
            JObject data = JObject.Parse("{\"data\":[\"a\",\"b\",1]}");
            GraphResponse response = new GraphResponse(data, new GraphRequest("endpoint", null));

            Assert.AreEqual(typeof(JsonArray), response.Data.GetType());
            Assert.AreEqual("a", response.Data[0]);
            Assert.AreEqual("b", response.Data[1]);
            Assert.AreEqual(1, response.Data[2]);

            Assert.AreEqual(typeof(JsonDictionary), response.Response.GetType());
            Assert.AreEqual(1, ((JsonDictionary)response.Response).Keys.Count);
            Assert.AreEqual(typeof(JsonArray), response.Response["data"].GetType());

        }

        [TestMethod]
        public void TestInstantiationWithObject()
        {
            JObject data = JObject.Parse("{\"a\":1,\"b\":\"2\"}");
            GraphResponse response = new GraphResponse(data, new GraphRequest("endpoint", null));

            Assert.AreEqual(typeof(JsonDictionary), response.Data.GetType());
            Assert.AreEqual(response.Response, response.Data);

            JsonDictionary responseData = (JsonDictionary)response.Data;
            Assert.IsTrue(responseData.ContainsKey("a"));
            Assert.IsTrue(responseData.ContainsKey("b"));

            Assert.AreEqual((long)1, responseData["a"]);
            Assert.AreEqual("2", responseData["b"]);

        }

    }
}
