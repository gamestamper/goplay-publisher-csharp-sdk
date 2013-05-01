using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoPlay.Serialization;
using Newtonsoft.Json.Linq;

namespace GoPlay
{
    public class GraphResponse : SDKComponent
    {
        public GraphResponse(dynamic input, GraphRequest request)
        {
            bool isObject = input.GetType() == typeof(JObject);
            bool isArray = input.GetType() == typeof(JArray);
            if (isObject)
            {
                JsonDictionary dInput = new JsonDictionary();
                dInput.Load(input);
                input = dInput;
            }
            else if (isArray)
            {

                JsonArray dInput = new JsonArray();
                dInput.Load(input);
                input = dInput;
            }

            Response = input;
            Request = request;

            if (isObject && input.ContainsKey("data"))
            {
                Data = input["data"];
                dynamic paging = null;
                if (((JsonDictionary)input).TryGetValue("paging", out paging)) {
                    Paging = paging;
                }
            }
            else
            {
                Data = input;
            }

            if (isObject && input.ContainsKey("error"))
            {
                Error = new SDKException(input["error"]);
            }
        }

        public dynamic GetDataValue(string key)
        {
            if (Data.GetType() == typeof(JsonArray) || Data.GetType() == typeof(JsonDictionary))
            {
                return SafeGet(Data, key);
            }
            return null;
        }


        public GraphResponse Next()
        {
            string url = (string)SafeGet(Paging, "next");
            return url == null ? null : DoPaging(url);
        }

        public GraphResponse Previous()
        {
            string url = (string)SafeGet(Paging, "previous");
            return url == null ? null : DoPaging(url);
        }

        public GraphResponse DoPaging(string url)
        {
            Uri uri = new Uri(url);
            JsonDictionary qs = Parser.Parse(uri.Query);
            return new GraphRequest(uri.AbsolutePath, qs).GetResponse();
        }

        #region properties

        public dynamic Response
        {
            get;
            set;
        }

        public dynamic Data
        {
            get;
            set;
        }

        public JsonDictionary Paging
        {
            get;
            set;
        }

        public GraphRequest Request
        {
            get;
            set;
        }

        public SDKException Error
        {
            get;
            set;
        }

        #endregion properties
    }
}
