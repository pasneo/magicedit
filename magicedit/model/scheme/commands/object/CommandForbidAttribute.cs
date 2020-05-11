using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    class CommandForbidAttribute : ISchemeCommand
    {

        string objectName, attributeName;

        public CommandForbidAttribute(string objectName, string attributeName)
        {
            this.objectName = objectName;
            this.attributeName = attributeName;
        }

        public void Execute(SchemeExecutor executor)
        {
            ObjectVariable objectVariable = executor.GetVariableByName(objectName);

            if (!executor.CheckTypeCompatibility(VariableTypes.Object, objectVariable.Type))
                throw SchemeExecutor.CreateException($"Object has to be compatible with 'object' (actual type: '{objectVariable.Type}')");

            Object @object = (Object)objectVariable.Value;

            @object.ForbidAttribute(attributeName);
        }

        public string GetAsString()
        {
            return $"FORBID ATTR ( {objectName}, {attributeName} )";
        }

        public void ChangeInputs(string current_val, string new_val)
        {
            if (objectName == current_val) objectName = new_val;
        }

        public void ChangeOutput(string current_val, string new_val)
        {
        }

        public bool HasOutput(string output_name)
        {
            return false;
        }

        public bool HasInput(string input_name)
        {
            return objectName == input_name;
        }
    }
}
