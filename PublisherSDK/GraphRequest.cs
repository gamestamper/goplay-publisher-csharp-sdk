using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using GoPlay.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GoPlay
{
    public class GraphRequest : SDKComponent
    {
        const string VERSION = "0.90";
        private static string host = "https://graph.goplay.com";
        private static string testhost = "https://test-graph.goplay.com";

        public GraphRequest(string endpoint, JsonDictionary parameters)
        {
            Endpoint = endpoint;
            Parameters = parameters;
            EffectiveUrl = MakeUri(Host, Endpoint, Parameters);
        }

        public GraphRequest(string endpoint, JsonDictionary parameters, string method) 
        {
            Endpoint = endpoint;
            Parameters = parameters;
            Method = method;
            EffectiveUrl = MakeUri(Host, Endpoint, Parameters);
        }

        public GraphResponse GetResponse()
        {
            Run();
            return new GraphResponse(Parser.Parse(Response), this);
        }


        protected void Run()
        {
            string url = MakeUri(Host, Endpoint);
            EffectiveUrl = MakeUri(Host, Endpoint, Parameters);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            req.UserAgent = "goplay-csharp-" + VERSION;
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            using (var writer = new StreamWriter(req.GetRequestStream()))
            {
                writer.Write(GetQueryString(Parameters));
            }

            StreamReader reader;
            try
            {
                reader = new StreamReader(req.GetResponse().GetResponseStream());        
            }
            catch (WebException e)
            {
                reader = new StreamReader(e.Response.GetResponseStream());
            }
            Response = reader.ReadToEnd();       
        }

        #region Helper methods
        public string MakeUri(string host, string endpoint)
        {
            return MakeUri(host, endpoint, null);
        }
        public string MakeUri(string host, string endpoint, JsonDictionary parameters)
        {
            string result = host + EnsureSlash(endpoint);
            if (parameters != null)
            {
                result += "?" + GetQueryString(parameters);
            }
            return result;
        }

        private string GetQueryString(JsonDictionary parameters)
        {
            if (parameters == null)
            {
                return null;
            }
            List<string> queryValues = new List<string>();
            foreach (string key in parameters.Keys)
            {
                queryValues.Add(key + "=" + HttpUtility.UrlEncode(parameters[key].ToString()));
            }
            return string.Join("&", queryValues);
        }
        #endregion Helper methods

        #region properties

        public static bool Testing
        {
            get;
            set;
        }

        public string Host
        {
            get
            {
                return Testing ? testhost : host;
            }
        }

        public string EffectiveUrl
        {
            get;
            set;
        }

        public string Response
        {
            get;
            set;
        }

        public string Endpoint
        {
            get;
            set;
        }

        private JsonDictionary _parameters;
        public JsonDictionary Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new JsonDictionary();
                }
                _parameters["method"] = Method;
                return _parameters;
            }
            set
            {
                _parameters = value;
            }
        }

        private string _method;
        public string Method
        {
            get
            {
                return _method==null ? "get" : _method;
            }
            set
            {
                _method = value;
            }
        }
        #endregion properties


    }
}
