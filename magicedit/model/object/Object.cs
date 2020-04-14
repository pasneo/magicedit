using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class Object
    {
        
        public string Id { get; set; }      //A unique identifier (eg. "sword (1)", "sword (2)")
        public string Name { get; set; }    //The name the creator gave in the editor (eg. "sword")
        public Text ShwonName { get; set; } //The name displayed during game (eg. "Sword of Death")

        public Scheme Scheme { get; set; }
        public Visual Visual { get; set; }

        public Text Description { get; set; }   //Current description (can be changed with scheme execution)

        public List<ObjectVariable> Variables = new List<ObjectVariable>();
        public List<ObjectAttribute> Attributes = new List<ObjectAttribute>();
        public List<ObjectVariable> Parameters = new List<ObjectVariable>();

        public HashSet<string> AvailableActions = new HashSet<string>();

        /* *** */

        public Object() { }

        public Object(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public void Create(Game game)
        {
            //Compile scheme and call its constructor
            Scheme.Construct(this, Parameters);
            Scheme.Init(this, game);
        }

        public int ExecuteAction(string actionName, Object actor, Game game)
        {
            SchemeFunction action = Scheme.GetFunctionByName(actionName);

            if (action == null) throw new GameException("Given action does not exist.");

            return action.Execute(this, actor, game);
        }

        public ObjectVariable GetVariableByName(string name)
        {
            foreach(var variable in Variables)
            {
                if (variable.Name == name) return variable;
            }
            return null;
        }

        public bool HasAttribute(string attributeName)
        {
            foreach(ObjectAttribute attr in Attributes)
            {
                if (attr.Type == ObjectAttributeType.Set && attr.Name == attributeName) return true;
            }
            return false;
        }

        public void AddAction(string actionName)
        {
            if (Scheme.GetFunctionByName(actionName) == null) throw new GameException($"Action '{actionName}' not found");
            AvailableActions.Add(actionName);
        }

        public void RemoveAction(string actionName)
        {
            AvailableActions.Remove(actionName);
        }

    }
}
