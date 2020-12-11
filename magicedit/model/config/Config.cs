using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace magicedit
{
    
    public class SpecialSchemes
    {
        public static readonly string Map = "map";
        public static readonly string Character = "character";
    }

    public class Config
    {

        public Map Map = new Map();

        public CharacterConfig CharacterConfig { get; set; } = new CharacterConfig();
        
        public List<Visual> Visuals = new List<Visual>();
        
        public List<Text> StringConsts = new List<Text>();

        public List<ClassList> ClassLists = new List<ClassList>();
        
        public List<Scheme> Schemes = new List<Scheme>();

        //All the predefined objects are stored here (but references to them may exist elsewhere too)
        public List<Object> Objects = new List<Object>();

        /* *** */
        
        /* <PERSISTENCE> */

        [JsonConstructor]
        public Config(
            Map map, CharacterConfig characterConfig, ItemSpellConfig itemConfig, ItemSpellConfig spellConfig, List<Visual> visuals,
            List<Text> stringConsts, List<ClassList> classLists, List<Scheme> schemes, List<Object> objects)
        {
            Map = map; CharacterConfig = characterConfig; ItemConfig = itemConfig; SpellConfig = spellConfig; Visuals = visuals;
            StringConsts = stringConsts; ClassLists = classLists; Schemes = schemes; Objects = objects;
        }

        public Config() { }

        public static Config Load(string filePath)
        {
            if (File.Exists(filePath))
            {
                //Uri uri = new Uri(filePath, UriKind.Absolute);
                string json = File.ReadAllText(filePath);
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
                jsonSettings.PreserveReferencesHandling = PreserveReferencesHandling.All;
                Config config = JsonConvert.DeserializeObject<Config>(json, jsonSettings);
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
            foreach (Scheme scheme in Schemes)
            {
                scheme.Compile(this);
            }

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

        public List<Object> CopyObjects(Game game)
        {
            List<Object> objects = new List<Object>();
            foreach(Object obj in Objects)
            {
                objects.Add(obj.Copy(game));
            }
            return objects;
        }

        public void RemoveAbility(string ability)
        {
            CharacterConfig.Abilities.RemoveAll(ab => ab.Name == ability);
            ClassLists.ForEach(cl =>
            {
                cl.Classes.ForEach(c =>
                {
                    c.RemoveAbilityModifier(ability);
                });
            });
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

        public void RemoveStringConst(Text text)
        {
            StringConsts.Remove(text);

            Objects.ForEach(obj => {
                obj.RemoveValuesContaining(text);
                if (obj.ShownName == text) obj.ShownName = null;
            });

            ClassLists.ForEach(cl =>
            {
                if (cl.ShownName == text) cl.ShownName = null;
                cl.Classes.ForEach(c =>
                {
                    if (c.ShownName == text) c.ShownName = null;
                });
            });
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

        public int GetSchemeCountByName(string name)
        {
            int n = 0;
            foreach(Scheme scheme in Schemes)
            {
                if (scheme.Name == name) ++n;
            }
            return n;
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
            return Objects.Where(o => o?.Id == id).FirstOrDefault();
        }

        public Object GetItemById(string id)
        {
            return GetObjectById(id);
        }

        public void RemoveScheme(Scheme scheme)
        {
            Schemes.Remove(scheme);

            Objects.ForEach(obj =>
            {
                if (obj.Scheme == scheme) obj.Scheme = null;
                obj.RemoveValuesOfType(scheme.Name);
            });

            CharacterConfig.InventorySlots.RemoveAll(slot => slot.Type == scheme.Name);

            Schemes.ForEach(sch =>
            {
                sch.CompiledScheme?.Parameters.RemoveAll(param => param.Type == scheme.Name);
            });
        }

        // removes object from list and all its references/occurences
        public void RemoveObject(Object @object)
        {
            Objects.Remove(@object);

            //remove it from map
            if (@object is MapObject) Map.RemoveObject((MapObject)@object);

            //remove it from variables and parameters
            Objects.ForEach(obj => obj.RemoveValuesContaining(@object));
        }

        /* VALIDATION */

        public List<EditorErrorDescriptor> Validate()
        {
            List<EditorErrorDescriptor> eeds = new List<EditorErrorDescriptor>();

            ValidateObjectNames(eeds);

            //schemes should be validated last
            ValidateSchemes(eeds);

            return eeds;
        }

        private void ValidateSchemes(List<EditorErrorDescriptor> eeds)
        {
            List<string> collisions = new List<string>();

            foreach (Scheme scheme in Schemes)
            {
                if (scheme.Name == null || scheme.Name.Length == 0)
                {
                    eeds.Add(new InvalidSchemeEED(scheme));
                    continue;
                }

                if (!collisions.Contains(scheme.Name))
                {

                    var schemeNameCollisionEED = new SchemeNameCollisionEED(scheme);
                    Schemes.ForEach(s =>
                    {
                        if (s != scheme && s.Name == scheme.Name) schemeNameCollisionEED.AddScheme(s);
                    });

                    if (schemeNameCollisionEED.ContainsCollision)
                    {
                        collisions.Add(scheme.Name);
                        eeds.Add(schemeNameCollisionEED);
                    }

                }

                Objects.Where(obj => obj.Id == scheme.Name).ToList().ForEach(obj => eeds.Add(new SchemeObjectNameCollisionEED(obj, scheme)));

                var errors = SchemeLang.CompileWithErrors(scheme, this);

                if (errors != null && errors.Count > 0)
                {
                    eeds.Add(new InvalidSchemeEED(scheme));
                }
            }
        }

        private void ValidateObjectNames(List<EditorErrorDescriptor> eeds)
        {
            // check name collisions
            List<string> collisions = new List<string>();

            foreach(var obj in Objects)
            {
                if (collisions.Contains(obj.Id)) continue;

                ObjectNameCollisionEED eed = new ObjectNameCollisionEED(obj);
                Objects.ForEach(o =>
                {
                    if (o != obj && o.Id == obj.Id) eed.AddObject(o);
                });

                if (eed.ContainsCollision)
                {
                    collisions.Add(obj.Id);
                    eeds.Add(eed);
                }
            }
        }

    }
}
