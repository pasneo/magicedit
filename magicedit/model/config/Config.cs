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

        private CharacterConfig CharacterConfig;
        private ItemSpellConfig ItemConfig;
        private ItemSpellConfig SpellConfig;

        private List<Visual> Visuals;
        private List<Text> Texts;

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

        public string GetStringConstByName(string name)
        {
            foreach (string s in StringConsts)
            {
                if (s == name) return s;
            }
            return null;
        }

    }
}
