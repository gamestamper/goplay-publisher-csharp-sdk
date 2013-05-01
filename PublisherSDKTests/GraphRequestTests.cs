using System;
using System.Collections;
using System.Collections.Generic;
using GoPlay;
using GoPlay.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace PublisherSDKTests
{
    [TestClass]
    public class GraphRequestTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            GraphRequest.Testing = true;
        }

        [TestMethod]
        public void TestInstantion()
        {
            var req = new GraphRequest("/pub/mystats", null);
            Assert.AreEqual(req.Endpoint, "/pub/mystats");

            Assert.AreEqual(typeof(JsonDictionary), req.Parameters.GetType());
            Assert.IsTrue(req.Parameters.ContainsKey("method"));
            Assert.AreEqual(req.Parameters["method"], "get");

            req = new GraphRequest("/pub/mystats", new JsonDictionary("{\"a\":\"b\"}"));
            Assert.AreEqual(req.Endpoint, "/pub/mystats");
            Assert.IsTrue(req.Parameters.ContainsKey("method"));
            Assert.AreEqual(req.Parameters["method"], "get");
            Assert.IsTrue(req.Parameters.ContainsKey("a"));
            Assert.AreEqual(req.Parameters["a"], "b");
        }


        [TestMethod]
        public void TestGetResponse()
        {
            JsonDictionary parameters = new JsonDictionary();
            parameters.Add("client_id", "pbgspub");
            parameters.Add("client_secret", "a8sdfjweuy3456");
            parameters.Add("grant_type", "publisher_credentials");

            GraphRequest request = new GraphRequest("/oauth/access_token", parameters);

            Assert.AreEqual("https://test-graph.goplay.com/oauth/access_token?client_id=pbgspub&client_secret=a8sdfjweuy3456&grant_type=publisher_credentials&method=get",
                request.EffectiveUrl);
            GraphResponse response = request.GetResponse();
            Assert.AreEqual(typeof(JsonDictionary),response.Data.GetType());
            Assert.IsTrue(response.Data.ContainsKey("access_token"),JsonConvert.SerializeObject(response.Response));
        }

    }
}
