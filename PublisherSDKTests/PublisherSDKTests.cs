using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using GoPlay;
using GoPlay.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PublisherSDKTests
{
    [TestClass]
    public class PublisherSDKTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            GraphRequest.Testing = true;
        }

        [TestMethod]
        public void TestInstantion()
        {
            var sdk = new PublisherSDK("id", "secret");
            Assert.AreEqual(sdk.Secret, "secret");
            Assert.AreEqual(sdk.Key, "id");
        }

        [TestMethod]
        public void TestSingletonReferencesSameObject()
        {
            var sdk = new PublisherSDK("pbgspub", "a8sdfjweuy3456");
            var anothersdk = new PublisherSDK("pbgspub", "a8sdfjweuy3456");
            Assert.AreEqual(null, sdk.StoredToken);
            Assert.AreEqual(null, anothersdk.StoredToken);

            Assert.IsNotNull(sdk.Token);
            Assert.AreEqual(anothersdk.StoredToken, sdk.Token);
        }


        [TestMethod]
        public void TestUnknownPropertyCalls()
        {
            dynamic sdk = new PublisherSDK("pbgspub", "a8sdfjweuy3456");
            Assert.AreEqual("/pbgspub", sdk.Pub.Endpoint);
            Assert.AreEqual("/pbgspub", sdk.pbgspub.Endpoint);
            Assert.AreEqual("/pbgspub/mystats", sdk.Pub.MyStats.Endpoint);
        }

        [TestMethod]
        public void TestToken()
        {
            dynamic sdk = new PublisherSDK("pbgspub", "a8sdfjweuy3456");
            Assert.IsNotNull(sdk.Token);
            Assert.AreEqual(sdk.Token, sdk.Pub.Token);
        }

        [TestMethod]
        public void TestPlayersClub()
        {
            dynamic sdk = new PublisherSDK("pbgspub", "a8sdfjweuy3456");

            //delete all
            //sdk.Pub.PlayersClub.Delete();

            //post a player
            JsonDictionary parameters = new JsonDictionary("{\"players\":[{\"accountId\":1,\"email\":\"a@b.com\",\"birthday\":\"6/26/1971\"}]}");
            GraphResponse r = sdk.Pub.PlayersClub.Post(parameters);
            Assert.AreEqual(typeof(JsonArray), r.Data.GetType());
            Assert.AreEqual(1, r.Data.Count);
            Assert.IsTrue(r.Data[0].StartsWith("ex"), r.Request.Response + " from " + r.Request.EffectiveUrl);

            //get him back
            string id = r.Data[0].ToString();
            r = sdk.Get(id);
            Assert.AreEqual(typeof(JsonDictionary), r.Data.GetType());
            Assert.AreEqual(1, r.Data["accountId"]);
            Assert.AreEqual("a@b.com", r.Data["email"]);
            Assert.AreEqual("1971-06-26", r.Data["birthday"]);

            //get all players
            r = sdk.Pub.PlayersClub.Get();
            Assert.AreEqual(typeof(JsonArray), r.Data.GetType(),"response from "+r.Request.EffectiveUrl+" was "+r.Request.Response);
            Assert.AreEqual(1,r.Data.Count);
            Assert.AreEqual(typeof(JsonDictionary), r.Data[0].GetType());
            Assert.AreEqual(1, r.Data[0]["accountId"]);
            Assert.AreEqual("a@b.com", r.Data[0]["email"]);
            Assert.AreEqual("1971-06-26", r.Data[0]["birthday"]);

            //now delete
            r = sdk.Pub.PlayersClub.Delete("1");

            //get, should be no more left
            r = sdk.Pub.PlayersClub.Get();
            Assert.AreEqual(0, r.Data.Count);
        }

        [TestMethod]
        public void TestPlayersClubPaging()
        {
            dynamic sdk = new PublisherSDK("pbgspub", "a8sdfjweuy3456");

            //post a player
            JsonDictionary parameters = new JsonDictionary();
            JsonArray players = new JsonArray();
            for (int j = 1; j <= 30; j++)
            {
                players.Add(new JsonDictionary("{\"accountId\":" + j.ToString() + ",\"email\":\"a" + j.ToString() + "@b.com\",\"birthday\":\"6/" + j.ToString() + "/1971\"}"));
            }
            parameters.Add("players", players);


            GraphResponse r = sdk.Pub.PlayersClub.Post(parameters);

            r = sdk.Pub.PlayersClub.Get();
            Assert.AreEqual(25, r.Data.Count);
            r = r.Next();
            Assert.AreEqual(5, r.Data.Count);
            r = r.Previous();
            Assert.AreEqual(25, r.Data.Count);

            for (int j = 1; j <= 30; j++)
            {
                sdk.Pub.PlayersClub.Delete(j.ToString());
            }
        }

        [TestMethod]
        public void TestFailTokenIncrementsAndClears()
        {
            dynamic sdk = new PublisherSDK("pbgspub", "a8sdfjweuy3456");
            string token = sdk.Token;
            Assert.AreEqual(0, sdk.TokenFailures);
            sdk.FailToken();
            Assert.AreEqual(1, sdk.TokenFailures);
            sdk.ClearTokenFailures();
            Assert.AreEqual(0, sdk.TokenFailures);
        }

        [TestMethod]
        public void TestHandleResponseThrowsExceptions()
        {
            dynamic sdk = new PublisherSDK("pbgspub", "a8sdfjweuy3456");
            string token = sdk.Token;

            GraphResponse resp = new GraphRequest("pbgspub/playersclub", null).GetResponse();
            try
            {
                sdk.HandleResponse(resp);
                Assert.IsFalse(true, "An exception should have been thrown");
            }
            catch (SDKException s)
            {
                Assert.AreEqual(104, s.ResponseCode);
            }
            catch (Exception e)
            {
                Assert.IsFalse(true, "The wrong kind of exception was thrown: "+e.Message);
            }
        }

        [TestMethod]
        public void TestRetryRequestAllowsOneFailure()
        {
            dynamic sdk = new PublisherSDK("pbgspub", "a8sdfjweuy3456");
            string token = sdk.Token;

            GraphResponse resp = new GraphRequest("pbgspub/playersclub",null).GetResponse();
            resp.Error = new SDKException("Sample bad token response", 190, "OAuthException");
            try
            {
                GraphResponse r = sdk.HandleResponse(resp);
                Assert.AreNotEqual(token, sdk.Token,"tokens should be different after retry");
            }
            catch (SDKException s)
            {
                Assert.AreNotEqual(190, s.ResponseCode,"exception with a bad token code should not have been thrown");
            }
        }

        [TestMethod]
        public void TestRetryRequestStopsMultipleFailures()
        {
            dynamic sdk = new PublisherSDK("pbgspub", "a8sdfjweuy3456");
            string token = sdk.Token;

            GraphResponse resp = new GraphRequest("pbgspub/playersclub", null).GetResponse();
            resp.Error = new SDKException("Sample bad token response", 190, "OAuthException");
            while (sdk.TokenFailures < PublisherSDK.ALLOWED_FAILURES)
            {
                sdk.FailToken();
            }
            try
            {
                GraphResponse r = sdk.HandleResponse(resp);
                Assert.IsFalse(true, "retry should have failed");
            }
            catch (SDKException s)
            {
                Assert.AreEqual(190, s.ResponseCode, "exception with a bad token code should have been thrown");
                Assert.IsTrue(sdk.TokenFailures > PublisherSDK.ALLOWED_FAILURES);
            }
        }


    }
}
