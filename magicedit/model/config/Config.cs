using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace magicedit
{
    
    public class Config
    {

        public Map Map = new Map();

        public CharacterConfig CharacterConfig { get; set; } = new CharacterConfig();
        public ItemSpellConfig ItemConfig { get; set; } = new ItemSpellConfig();
        public ItemSpellConfig SpellConfig { get; set; } = new ItemSpellConfig();

        [JsonProperty]
        public List<Visual> Visuals = new List<Visual>();

        [JsonProperty]
        public List<Text> StringConsts = new List<Text>();

        public List<ClassList> ClassLists = new List<ClassList>();

        [JsonProperty]
        public List<Scheme> Schemes = new List<Scheme>();

        //All the predefined objects are stored here (but references to them may exist elsewhere too)
        [JsonProperty]
        public List<Object> Objects = new List<Object>();

        /* *** */
        
        /* <PERSISTENCE> */

        public static Config Load(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                Config config = JsonConvert.DeserializeObject<Config>(json);
                config.AfterLoaded();
                return config;
            }

            return null;
        }

        public void Save(string filePath)
        {
            string json = JsonConvert.SerializeObject(
                this,
                Formatting.Indented,
                new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects }  //required for preserving references
                );
            File.WriteAllText(filePath, json);
        }

        // called after this config has been loaded from file
        private void AfterLoaded()
        {
            Map?.RecollectMapObjects(Objects);
        }

        /* </PERSISTENCE> */

        public Visual GetVisualById(string id)
        {
            foreach(Visual visual in Visuals)
            {
                if (visual.ID == id) return visual;
            }
            return null;
        }

        public List<Object> CopyObjects()
        {
            List<Object> objects = new List<Object>();
            foreach(Object obj in Objects)
            {
                objects.Add(obj.Copy());
            }
            return objects;
        }

        public void AddClassList(ClassList classList)
        {
            ClassLists.Add(classList);
        }

        public bool IsClassType(string name)
        {
            foreach(ClassList classList in ClassLists)
            {
                if (classList.Name == name) return true;
            }
            return false;
        }

        //Searches for given class in all classlists (returns null if not found), return both classlist and class in a tuple
        public Tuple<ClassList, Class> GetClassByName(string name)
        {
            foreach(ClassList classList in ClassLists)
            {
                Class @class = classList.GetClassByName(name);
                if (@class != null) return new Tuple<ClassList, Class>(classList, @class);
            }
            return null;
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
            if (name.Length > 0 && name[0] == '$') name = name.Substring(1);
            return StringConsts.Where(sc => sc.ID == name).FirstOrDefault();
        }

        public void AddStringConst(string name, Text text)
        {
            text.ID = name;
            StringConsts.Add(text);
        }

        public void AddScheme(Scheme scheme)
        {
            Schemes.Add(scheme);
        }

        public void AddObject(Object @object)
        {
            Objects.Add(@object);
        }

        public Object GetObjectById(string id)
        {
            return Objects.Where(o => o.Id == id).FirstOrDefault();
        }

        public Object GetItemById(string id)
        {
            return GetObjectById(id);
        }

    }
}
