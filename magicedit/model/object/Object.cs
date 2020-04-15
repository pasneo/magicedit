using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class Object
    {
     
        /*
         * TODO: the Name property might be renamed to OriginalId or something similar to highlight to following:
         * 
         * 'Id' is a unique identifier, but it is used mainly (or only?) to identify pre-defined (in-editor) objects from code.
         * Eg. we define a sword item, and want to use it in code as 'add sword to actor'. In this case the Id of the item
         * will be 'sword'. But we also want to write code like 'actor has 2 sword'. In this case we want all the sword items
         * to be counted, thus we must use their Name, which will be 'sword' for each.
         * 
         * Conclusion: the Name must be the same as the Id of the original object. Copies of the original can (or must) have
         * newly defined Ids (eg. sword_1, sword_2 ...)
         */

        public string Id { get; set; }      //A unique identifier (eg. "sword", "sword (2)")
        public string Name { get; set; }    //The name the creator gave in the editor (eg. "sword")
        public Text ShownName { get; set; } //The name displayed during game (eg. "Sword of Death")

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

        public Object Copy()
        {
            Object copy = new Object();

            copy.Id = Id;
            copy.Name = Name;
            copy.ShownName = ShownName;

            //Reference vars (copy only reference):
            copy.Scheme = Scheme;
            copy.Visual = Visual;
            copy.Description = Description;
            copy.Parameters = Parameters;

            //Contained vars (copy values):
            foreach (ObjectVariable variable in Variables) copy.Variables.Add(variable.Copy());
            foreach (ObjectAttribute attr in Attributes) copy.Attributes.Add(attr);
            foreach (string action in AvailableActions) copy.AvailableActions.Add(action);

            return copy;
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

        public void SetAttribute(string attributeName)
        {
            foreach(ObjectAttribute attr in Attributes)
            {
                if (attr.Name == attributeName)
                {
                    if (attr.Type == ObjectAttributeType.Forbid) throw new GameException($"Attribute '{attributeName}' is forbidden");
                    return; //Attribute is already set, we are done
                }
            }
            Attributes.Add(new ObjectAttribute(ObjectAttributeType.Set, attributeName));
        }

        public void RemoveAttribute(string attributeName)
        {
            Attributes.RemoveAll(attr => (attr.Type == ObjectAttributeType.Set && attr.Name == attributeName));
        }

        public void ForbidAttribute(string attributeName)
        {
            //Forbidding an attribute causes that attribute to be removed if it is set
            Attributes.RemoveAll(attr => (attr.Name == attributeName));
            Attributes.Add(new ObjectAttribute(ObjectAttributeType.Forbid, attributeName));
        }

    }
}
