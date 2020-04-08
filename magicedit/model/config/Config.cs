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
        private Dictionary<string, Text> StringConsts = new Dictionary<string, Text>();

        private List<Scheme> Schemes;

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
            if (StringConsts.ContainsKey(name)) return StringConsts[name];
            return null;
        }

        public void AddStringConst(string name, Text text)
        {
            StringConsts[name] = text;
        }

    }
}
