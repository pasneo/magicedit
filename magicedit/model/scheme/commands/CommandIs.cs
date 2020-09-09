using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandIs : ISchemeCommand
    {

        string objectName, propertyName, target;

        public CommandIs(string objectName, string propertyName, string target)
        {
            this.objectName = objectName;
            this.propertyName = propertyName;
            this.target = target;
        }

        public void Execute(SchemeExecutor executor)
        {

            ObjectVariable objectVariable = executor.GetVariableByName(objectName);

            // ObjectVariable property = executor

            //Uses of 'is':
            //  object has attribute                actor is forest_wanderer
            //  class var                           race of actor is dwarf          // -> OF(race, actor, _0) EQUALS(_0, dwarf, _0)
            //  item/spell is the same as...        some_item is sword_1            //some_item.Name == "sword_1"

            /*
             * 1. Check if object's name equals to propertyName
             * 2. Check if object has attribute named propertyName
             */
            
            bool value = false;

            //If we check class variable
            if (executor.Game.Config.IsClassType(objectVariable.Type))
            {
                //we search for the required class
                var c = executor.FindValueByString(propertyName);
                //then we compare it to the classvar
                value = objectVariable.Value == c.Value;
            }
            else
            {
                if (!executor.CheckTypeCompatibility(VariableTypes.Object, objectVariable.Type))
                {
                    throw SchemeExecutor.CreateException("Object has to be compatible with 'object'");
                }

                Object @object = (Object)objectVariable.Value;

                if (@object.Name == propertyName) value = true;
                else if (@object.HasAttribute(propertyName)) value = true;
            }

            executor.SetVariable(target, new ObjectVariable(VariableTypes.Logical, "", value));

        }

        public string GetAsString()
        {
            return $"IS ( {objectName}, {propertyName}, {target} )";
        }

        public void ChangeInputs(string current_val, string new_val)
        {
            if (objectName == current_val) objectName = new_val;
        }

        public void ChangeOutput(string current_val, string new_val)
        {
            if (target == current_val) target = new_val;
        }

        public bool HasInput(string input_name)
        {
            return objectName == input_name;
        }

        public bool HasOutput(string output_name)
        {
            return target == output_name;
        }

        public List<string> GetInputs()
        {
            return new List<string> { objectName };
        }

        public List<string> GetOutputs()
        {
            return new List<string> { target };
        }

    }
}
