using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandRemoveAttribute : ISchemeCommand
    {

        string objectName, attributeName;

        public CommandRemoveAttribute(string objectName, string attributeName)
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

            @object.RemoveAttribute(attributeName);
        }

        public string GetAsString()
        {
            return $"REMOVE ATTR ( {objectName}, {attributeName} )";
        }

        public void ChangeInputs(string current_val, string new_val)
        {
            if (objectName == current_val) objectName = new_val;
        }

        public bool HasOutput(string output_name)
        {
            return false;
        }

        public void ChangeOutput(string current_val, string new_val)
        {
        }

        public bool HasInput(string input_name)
        {
            return (objectName == input_name);
        }

        public List<string> GetInputs()
        {
            return new List<string> { objectName };
        }

        public List<string> GetOutputs()
        {
            return null;
        }
    }
}
