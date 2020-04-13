using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public enum ObjectAttributeType
    {
        Set,
        Forbid
    }

    public class ObjectAttribute
    {
        public ObjectAttributeType Type;
        public string Name;

        public ObjectAttribute() { }

        public ObjectAttribute(ObjectAttributeType type, string name)
        {
            Type = type;
            Name = name;
        }

    }
}
