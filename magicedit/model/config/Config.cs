﻿using System;
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
        private ItemSpellConfig ItemConfig { get; set; } = new ItemSpellConfig();
        private ItemSpellConfig SpellConfig { get; set; } = new ItemSpellConfig();

        private List<Visual> Visuals = new List<Visual>();
        private Dictionary<string, Text> StringConsts = new Dictionary<string, Text>();

        public List<ClassList> ClassLists = new List<ClassList>();

        private List<Scheme> Schemes = new List<Scheme>();

        //All the predefined objects are stored here (but references to them may exist elsewhere too)
        private List<Object> Objects = new List<Object>();

        /* *** */

        public void AddClassList(ClassList classList)
        {
            ClassLists.Add(classList);
        }

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

        public void AddScheme(Scheme scheme)
        {
            Schemes.Add(scheme);
        }

    }
}
