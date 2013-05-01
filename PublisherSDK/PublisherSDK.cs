using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Caching;
using GoPlay.Serialization;

namespace GoPlay
{
    public class PublisherSDK : SDKComponent
    {
        public const int ALLOWED_FAILURES = 2;

        public PublisherSDK(string key, string secret)
        {
            Key = key;
            Secret = secret;
        }

        #region request methods
        public GraphResponse Get()
        {
            return Get("");
        }
        public GraphResponse Get(JsonDictionary parameters)
        {
            return Get("", parameters);
        }
        public GraphResponse Get(string endpoint)
        {
            return Get(endpoint, null);
        }
        public GraphResponse Get(string endpoint, JsonDictionary parameters)
        {
            return MakeGraphRequest(endpoint, parameters, "get");
        }

        public GraphResponse Post()
        {
            return Post("");
        }
        public GraphResponse Post(JsonDictionary parameters)
        {
            return Post("", parameters);
        }
        public GraphResponse Post(string endpoint)
        {
            return Post(endpoint, null);
        }
        public GraphResponse Post(string endpoint, JsonDictionary parameters)
        {
            return MakeGraphRequest(endpoint, parameters, "post");
        }

        public GraphResponse Delete()
        {
            return Delete("");
        }
        public GraphResponse Delete(JsonDictionary parameters)
        {
            return Delete("", parameters);
        }
        public GraphResponse Delete(string endpoint)
        {
            return Delete(endpoint, null);
        }
        public GraphResponse Delete(string endpoint, JsonDictionary parameters)
        {
            return MakeGraphRequest(endpoint, parameters, "delete");
        }
        #endregion request methods

        #region Helper methods
        public string GetTokenFromServer()
        {
            StoredToken = new GraphRequest("/oauth/access_token", TokenParameters).GetResponse().GetDataValue("access_token");
            return StoredToken;
        }

        public dynamic MakeGraphRequest(string endpoint, JsonDictionary parameters, string method)
        {
            AddSegment(endpoint);
            parameters = parameters == null ? new JsonDictionary() : parameters;
            parameters["access_token"] = Token;
            GraphResponse response = new GraphRequest(Endpoint, parameters, method).GetResponse();
            Endpoint = "";
            return HandleResponse(response);
        }

        public GraphResponse HandleResponse(GraphResponse response)
        {
            if (response.Error != null)
            {
                if (response.Error.ResponseCode == 190)
                {
                    return RetryRequest(response);
                }
                ClearTokenFailures();
                throw response.Error;
            }
            ClearTokenFailures();
            return response;
        }

        public GraphResponse RetryRequest(GraphResponse response)
        {
            FailToken();
		    if (this.TokenFailures <= ALLOWED_FAILURES) {
                GraphRequest req = response.Request;
                req.Parameters["access_token"] = GetTokenFromServer();
                return HandleResponse(req.GetResponse());
            }
            throw response.Error;
        }

        public void AddSegment(string segment)
        {
            if (segment == "pub")
            {
                Endpoint += EnsureSlash(Key);
            }
            else if (!String.IsNullOrEmpty(segment))
            {
                Endpoint += EnsureSlash(segment);
            }
        }

        protected PublisherSDK ExtendPath(string path)
        {
            PublisherSDK newSdk = new PublisherSDK(Key, Secret);
            newSdk.Endpoint = Endpoint;
            newSdk.AddSegment(path);
            newSdk.Token = StoredToken;
            return newSdk;
        }

        public void FailToken()
        {
            TokenFailures++;
        }

        public void ClearTokenFailures()
        {
            TokenFailures = 0;
        }

        #endregion Helper methods

        #region DynamicObject overrides 
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (new string[] { "get", "post", "delete" }.Contains(binder.Name.ToLower()))
            {
                throw new MissingMethodException("You have called " + binder.Name + " with an incorrect signature. Please check your parameters.");
            }
            result = null;
            return false;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = ExtendPath(binder.Name.ToLower());
            return true;
        }
        #endregion DynamicObject overrides

        #region properties

        protected static PublisherSDK Instance
        {
            get;
            set;
        }

        private static Dictionary<string, TokenObject> _tokenCache = new Dictionary<string, TokenObject>();
        public static Dictionary<string, TokenObject> TokenCache
        {
            get 
            {
                return _tokenCache;
            }
        }


        protected JsonDictionary TokenParameters
        {
            get
            {
                JsonDictionary result = new JsonDictionary();
                result["client_id"] = Key;
                result["client_secret"] = Secret;
                result["grant_type"] = "publisher_credentials";
                return result;
            }
        }

        public string StoredToken
        {
            get 
            {
                return StoredTokenObject.Token;
            }
            set
            {
                StoredTokenObject = new TokenObject(value);
            }
        }

        public TokenObject StoredTokenObject
        {
            get
            {
                return TokenCache.ContainsKey(CacheKey) ? TokenCache[CacheKey] : new TokenObject();
            }
            set
            {
                TokenCache[CacheKey] = value;
            }
        }

        public string Token
        {
            get
            {
                if (StoredToken == null)
                {
                    GetTokenFromServer();
                }
                return StoredToken;
            }
            set
            {
                StoredToken = value;
            }
        }

        public int TokenFailures
        {
            get
            {
                return StoredTokenObject.Failures;
            }
            set
            {
                StoredTokenObject.Failures = value;
            }
        }

        public string Endpoint
        {
            get;
            set;
        }

        public string Key
        {
            get;
            set;
        }

        public string Secret
        {
            get;
            set;
        }

        private string _cacheKey;
        private string CacheKey
        {
            get
            {
                if (_cacheKey == null)
                {
                    _cacheKey = Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(new UTF8Encoding().GetBytes(Key + "-" + Secret)));
                }
                return _cacheKey;
            }
        }

        #endregion properties

    }

}
