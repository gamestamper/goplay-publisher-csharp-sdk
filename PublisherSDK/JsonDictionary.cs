using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using GoPlay.Serialization.Mixins;
using Newtonsoft.Json;

namespace GoPlay.Serialization
{
    public class JsonDictionary : Dictionary<string, object>, IJsonWrapper
    {
        public JsonDictionary()
            : base()
        {
        }

        public JsonDictionary(string json) {
            Load(json); 
        }

        public void Load(string input)
        {
            Load(JObject.Parse(input));
        }

        public void Load(JObject input)
        {
            Dictionary<string, object> result = this.TransformToJsonDictionary(input);
            foreach (string key in result.Keys)
            {
                this.Add(key, result[key]);
            }
        }

        public string ToString(JsonFormat format)
        {
            JObject obj = JObject.FromObject(this);
            return obj.ToString(format == JsonFormat.Indented ? Formatting.Indented : Formatting.None);
        }

        public override string ToString()
        {
            return this.ToString(JsonFormat.None);
        }

    }
}
