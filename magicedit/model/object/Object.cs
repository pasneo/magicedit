﻿using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{

    public enum ObjectTypeTags {
        Undefined,
        Item,
        Spell,
        MapObject
    }

    [JsonConverter(typeof(JsonSubtypes), "ObjectType")]
    public class Object
    {

        public virtual string ObjectType { get; } = "Object";

        public virtual ObjectTypeTags TypeTag { get; set; }     //used to identify items, spells etc. in the editor

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

        public bool IsVisible { get; set; } = true;         //if this is false, the object is not showing on the map

        [JsonProperty]
        private bool isPermeable = false;
        [JsonIgnore]
        public bool IsPermeable
        {
            get { return !IsVisible || isPermeable; }
            set { isPermeable = value; }
        }

        /* *** */

        public Object() { }

        public Object(string id, string name, ObjectTypeTags typeTag = ObjectTypeTags.Undefined)
        {
            Id = id;
            Name = name;
            TypeTag = typeTag;
        }

        public virtual Object Copy(Game game)
        {
            Object copy = new Object();

            CopyTo(copy, game);

            return copy;
        }

        public virtual void CopyTo(Object copy, Game game)
        {
            copy.TypeTag = TypeTag;

            copy.Id = (game == null) ? Id : IdGenerator.Generate(Id, id => game.GetObjectById(id) == null);
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
        }

        public void Create(Game game)
        {
            if (Scheme == null) return;

            //Compile scheme and call its constructor
            Scheme.Construct(this, Parameters, game);
            Scheme.Init(this, game);
        }

        public int ExecuteAction(string actionName, Object actor, Game game)
        {
            SchemeFunction action = Scheme?.GetFunctionByName(actionName);

            if (action == null) throw new GameException("Given action does not exist.");

            return action.Execute(this, actor, game);
        }

        //Calculates actual value of ability, and returns it as a (nameless) number variable
        private ObjectVariable CalculateAbility(ObjectVariable ability, Config config)
        {
            int modifValue = 0;
            foreach (ObjectVariable var in Variables)
            {
                if (config.IsClassType(var.Type))
                {
                    Class @class = (Class)var.Value;
                    if (@class != null && @class.GetAbilityModifier(ability.Name) != null)
                        modifValue += @class.GetAbilityModifier(ability.Name).Value;
                }
            }
            ObjectVariable actualVar = new ObjectVariable(VariableTypes.Number, "", modifValue + (int)ability.Value);
            return actualVar;
        }

        public ObjectVariable GetVariableByName(string name, Config config)
        {
            foreach(var variable in Variables)
            {
                if (variable.Name == name)
                {
                    //ability variables should be returned with their actual (calculated) value
                    if (variable.Type == VariableTypes.Ability) return CalculateAbility(variable, config);
                    return variable;
                }
            }
            return null;
        }

        public ObjectVariable GetParameterByName(string name)
        {
            return Parameters.Where(param => param.Name == name).FirstOrDefault();
        }

        public bool HasAttribute(string attributeName)
        {
            foreach(ObjectAttribute attr in Attributes)
            {
                if (attr.Type == ObjectAttributeType.Set && attr.Name == attributeName) return true;
            }
            return false;
        }

        public void RemoveValuesContaining(object value)
        {
            Parameters.RemoveAll(param => param.Value == value);
            Variables.RemoveAll(variable => variable.Value == value);
        }

        public void AddAction(string actionName)
        {
            if (Scheme?.GetFunctionByName(actionName) == null) throw new GameException($"Action '{actionName}' not found");
            AvailableActions.Add(actionName);
        }

        public void RemoveAction(string actionName)
        {
            AvailableActions.Remove(actionName);
        }

        public void ClearActions()
        {
            AvailableActions.Clear();
        }

        public void RemoveValuesOfType(string name)
        {
            Parameters.RemoveAll(param => param.Type == name);
            Variables.RemoveAll(variable => variable.Type == name);
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
