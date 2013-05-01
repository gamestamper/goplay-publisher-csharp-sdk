using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;

namespace GoPlay.Serialization
{
    class Parser
    {
        //<summary>
        // deserializes a string, either as a JObject, a JArray, or a Dictionary<string,object>, depending on which is most appropriate
        //</summary>
        public static dynamic Parse(string value)
        {
            dynamic data = value;
            if (value.StartsWith("{"))
            {
                data = JObject.Parse(value);
            }
            else if (value.StartsWith("["))
            {
                data = JArray.Parse(value);
            }
            else if (Regex.Match(value,"[&=]").Success)
            {
                //parse as query string
                data = ParseQueryString(value);
            }
            return data;
        }

        public static JsonDictionary ParseQueryString(string value)
        {
            NameValueCollection vals = HttpUtility.ParseQueryString(value);
            JsonDictionary result = new JsonDictionary();
            NameValueCollection.KeysCollection keys = vals.Keys;
            foreach (string key in keys)
            {
                result[key] = vals[key];
            }
            return result;
        }
    }
}
