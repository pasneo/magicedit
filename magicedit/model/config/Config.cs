using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class Config
    {

        public Map Map;

        public CharacterConfig CharacterConfig { get; set; } = new CharacterConfig();
        private ItemSpellConfig ItemConfig;
        private ItemSpellConfig SpellConfig;

        private List<Visual> Visuals;
        private Dictionary<string, Text> Texts;

        private List<Scheme> Schemes;
        private List<string> StringConsts;

        /* *** */

        public Scheme GetSchemeByName(string name)
        {
            foreach(Scheme scheme in Schemes)
            {
                if (scheme.Name == name) return scheme;
            }
            return null;
        }

        public Text GetStringConstByName(string name)
        {
            if (Texts.ContainsKey(name)) return Texts[name];
            return null;
        }

    }
}
