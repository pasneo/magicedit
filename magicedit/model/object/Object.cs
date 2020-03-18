using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class Object
    {
        
        public string Id { get; set; }

        public Scheme Scheme { get; set; }
        public Visual Visual { get; set; }

        public Text Description { get; set; }

        public List<ObjectVariable> Variables;
        public List<ObjectAttribute> Attributes;

        /* *** */

        public void Construct()
        {
            //TODO: compile scheme and call its constructor
        }

        public void ExecuteAction(string actionName, Object actor)
        {
            //TODO: get action by name, and execute it
        }

        public ObjectVariable GetVariableByName(string name)
        {
            foreach(var variable in Variables)
            {
                if (variable.Name == name) return variable;
            }
            return null;
        }

    }
}
