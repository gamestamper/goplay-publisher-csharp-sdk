using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GoPlay.Serialization.Mixins
{
    public static class JsonWrapperExtensions
    {
        public static dynamic TransformToGoplayType(this IJsonWrapper item, dynamic data)
        {
            Type dataType = data.GetType();
            if (dataType == typeof(JObject))
            {
                return TransformToJsonDictionary(item, data);
            }
            else if (dataType == typeof(JArray))
            {
                return TransformToJsonArray(item, data);
            }
            else if (dataType == typeof(JValue))
            {
                return data.Value;
            }
            return data;
        }

        public static JsonDictionary TransformToJsonDictionary(this IJsonWrapper item, JObject data)
        {
            JsonDictionary dataAsDictionary = (JsonDictionary)data.ToObject(typeof(JsonDictionary));
            JsonDictionary result = new JsonDictionary();
            foreach (string key in dataAsDictionary.Keys)
            {
                result.Add(key, TransformToGoplayType(item, data[key]));
            }
            return result;
        }

        public static JsonArray TransformToJsonArray(this IJsonWrapper item, JArray data)
        {
            JsonArray dataAsList = (JsonArray)data.ToObject(typeof(JsonArray));
            JsonArray result = new JsonArray();
            for (int j = 0; j < dataAsList.Count; j++)
            {
                result.Add(TransformToGoplayType(item, data[j]));
            }
            return result;
        }
    }
}
