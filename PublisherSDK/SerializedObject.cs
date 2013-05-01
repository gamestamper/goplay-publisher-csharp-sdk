using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoPlay.Serialization
{
    class SerializedObject : DynamicObject
    {
        private IDictionary<string, object> Dictionary { get; set; }

        public SerializedObject(IDictionary<string, object> dictionary)
        {
            this.Dictionary = dictionary;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this.Dictionary[binder.Name];

            if (result is IDictionary<string, object>)
            {
                result = new SerializedObject(result as IDictionary<string, object>);
            }
            else if (result is ArrayList && (result as ArrayList) is IDictionary<string, object>)
            {
                result = new List<SerializedObject>((result as ArrayList).ToArray().Select(x => new SerializedObject(x as IDictionary<string, object>)));
            }
            else if (result is ArrayList)
            {
                result = new List<object>((result as ArrayList).ToArray());
            }

            return this.Dictionary.ContainsKey(binder.Name);
        }
    }
}
