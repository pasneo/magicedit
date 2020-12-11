using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    
    public class ObjectJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        //[return: NullableAttribute(2)]
        public override object ReadJson(JsonReader reader, Type objectType, /*[NullableAttribute(2)]*/ object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, /*[NullableAttribute(2)]*/ object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
