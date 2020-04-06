using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class Object
    {
        
        public string Id { get; set; }      //A unique identifier
        public string Name { get; set; }    //The name the creator gave in the editor

        public Scheme Scheme { get; set; }
        public Visual Visual { get; set; }

        public Text Description { get; set; }   //Current description (can be changed with scheme execution)

        public List<ObjectVariable> Variables = new List<ObjectVariable>();
        public List<ObjectAttribute> Attributes = new List<ObjectAttribute>();

        /* *** */

        public void Construct()
        {
            //TODO: compile scheme and call its constructor
        }

        public void ExecuteAction(string actionName, Object actor)
        {
            SchemeFunction action = Scheme.GetFunctionByName(actionName);

            if (action == null) throw new GameException("Given action does not exist.");

            action.Execute(this, actor);
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
