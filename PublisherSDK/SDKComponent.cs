using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace GoPlay
{
    public class SDKComponent : DynamicObject
    {
        public static string EnsureSlash(string value)
        {
            return String.IsNullOrEmpty(value) || value.StartsWith("/") ? value : "/" + value;
        }

        public static dynamic SafeGet(dynamic collection, string key)
        {
            return SafeGet(collection, key, null);
        }

        public static dynamic SafeGet(dynamic collection, string key, dynamic defaultResult) {
            return collection.ContainsKey(key) ? collection[key] : defaultResult;
        }
    }
}
