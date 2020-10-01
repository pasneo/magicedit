using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{

    public class ItemSpellCategory
    {
        public string Name { get; set; }
        public Scheme Scheme { get; set; } = new Scheme();

        public ItemSpellCategory(string name)
        {
            Name = name;
        }
    }

    public class ItemSpellConfig
    {
        //This scheme is used for all items or spells
        public Scheme CommonScheme { get; set; }

        public List<ItemSpellCategory> Categories { get; set; } = new List<ItemSpellCategory>();

    }
}
