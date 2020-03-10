using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class Object
    {
        
        public Scheme Scheme { get; set; }
        public Visual Visual { get; set; }

        public Text Description { get; set; }

        public List<Property> Properties;

    }
}
