using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace magicedit
{
    public class Scheme
    {

        public string Name { get; set; }

        private string code;
        public string Code
        {
            get { return code; }
            //Modifying the code will invalidate the compiled scheme
            set
            {
                code = value;
                IsCompiledValid = false;
            }
        }

        public List<Scheme> Parents = new List<Scheme>();
        
        public CompiledScheme CompiledScheme { get; set; }
        
        [JsonIgnore]
        public bool IsCompiledValid { get; set; } = false;

        /* *** */

        public Scheme() { }

        public Scheme(string name)
        {
            Name = name;
        }

        public void AddParent(Scheme parent)
        {
            Parents.Add(parent);
        }

        public void Compile(Config config)
        {
            // compile self
            if (Code != null && Code != "")
                SchemeLang.Compile(this, config);

            IsCompiledValid = true;

            // compile parents
            foreach (var parent in Parents)
            {
                parent.Compile(config);
            }
        }

        public void Construct(Object @object, List<ObjectVariable> parameters, Game game)
        {
            if (!IsCompiledValid) Compile(game.Config);

            //construct object based on this scheme
            CompiledScheme.Construct(@object, parameters, game);

            //call parents
            foreach (var parent in Parents)
            {
                parent.Construct(@object, parameters, game);
            }

        }

        public void Init(Object @object, Game game)
        {
            //initialize object with this scheme
            CompiledScheme.Init(@object, game);

            //call parents
            foreach (var parent in Parents)
            {
                parent.Init(@object, game);
            }
        }

        public SchemeFunction GetFunctionByName(string functionName)
        {
            SchemeFunction matchingFunction = CompiledScheme.GetFunctionByName(functionName);

            //If given name was not found in this scheme, we try the parents
            if (matchingFunction == null)
            {
                foreach (Scheme parent in Parents)
                {
                    matchingFunction = parent.GetFunctionByName(functionName);
                    if (matchingFunction != null) return matchingFunction;
                }
            }
            else return matchingFunction;

            return null;

        }

        public bool HasAncestor(string ancestorName)
        {
            foreach(Scheme parent in Parents)
            {
                if (parent.Name == ancestorName) return true;
                if (parent.HasAncestor(ancestorName)) return true;
            }
            return false;
        }

        public bool HasAncestor(Scheme ancestor)
        {
            foreach (Scheme parent in Parents)
            {
                if (parent == ancestor) return true;
                if (parent.HasAncestor(ancestor)) return true;
            }
            return false;
        }

    }
}
