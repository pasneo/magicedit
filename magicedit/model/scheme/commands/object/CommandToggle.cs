using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandToggle : ISchemeCommand
    {

        string objectName;

        public CommandToggle(string objectName)
        {
            this.objectName = objectName;
        }

        public void Execute(SchemeExecutor executor)
        {
            var objVar = executor.GetVariableByName(objectName);

            if (objVar != null)
            {
                var obj = (Object)objVar.Value;
                obj.IsVisible = !obj.IsVisible;
            }

        }

        public string GetAsString()
        {
            return $"TOGGLE ( {objectName} )";
        }

        public void ChangeInputs(string current_val, string new_val)
        {
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
            return false;
        }

        public List<string> GetInputs()
        {
            return null;
        }

        public List<string> GetOutputs()
        {
            return null;
        }

    }
}
