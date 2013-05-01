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
    public class JsonArray : ArrayList, IJsonWrapper
    {
        public JsonArray() : base()
        {
        }

        public JsonArray(object[] array) : base() {
            Load(JArray.FromObject(array));
        }

        public JsonArray(string json)
            : base()
        {
            Load(json);
        }


        public void Load(object[] array)
        {
            Load(JArray.FromObject(array));
        }

        public void Load(string input)
        {
            Load(JArray.Parse(input));
        }

        public void Load(JArray input)
        {
            JsonArray result = this.TransformToJsonArray(input);
            foreach (object key in result)
            {
                this.Add(key);
            }
        }

        public string ToString(JsonFormat format)
        {
            JArray arr = JArray.FromObject(this);
            return arr.ToString(format == JsonFormat.Indented ? Formatting.Indented : Formatting.None);
        }

        public override string ToString()
        {
            return this.ToString(JsonFormat.None);
        }

    }
}
