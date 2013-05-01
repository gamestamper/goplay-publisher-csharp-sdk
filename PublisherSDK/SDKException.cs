using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using GoPlay.Serialization;
using Newtonsoft.Json.Linq;

namespace GoPlay
{
    public class SDKException : Exception
    {
        public SDKException(string message, long code, string type)
            : base(message)
        {
            ResponseCode = code;
            Type = type;
        }

        public SDKException(JObject errorResponse)
            : base(errorResponse["message"].ToString())
        {

            ResponseCode = (long)errorResponse["code"];
            Type = errorResponse["type"].ToString();
        }

        public SDKException(JsonDictionary errorResponse)
            : base(errorResponse["message"].ToString())
        {

            ResponseCode = (long)errorResponse["code"];
            Type = errorResponse["type"].ToString();
        }

        public long ResponseCode
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }
    }
}
