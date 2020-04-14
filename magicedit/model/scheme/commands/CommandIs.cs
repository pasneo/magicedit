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

            if (!executor.CheckTypeCompatibility(VariableTypes.Object, objectVariable.Type)) throw SchemeExecutor.CreateException("Object has to be compatible with 'object'");

            Object @object = (Object)objectVariable.Value;
            bool value = false;

            if (@object.Name == propertyName) value = true;
            else if (@object.HasAttribute(propertyName)) value = true;

            executor.SetVariable(target, new ObjectVariable(VariableTypes.Logical, "", value));

        }

        public string GetAsString()
        {
            return $"IS ( {objectName}, {propertyName}, {target} )";
        }

    }
}
