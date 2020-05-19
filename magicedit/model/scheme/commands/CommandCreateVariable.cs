using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandCreateVariable : ISchemeCommand
    {

        string type, varName;

        public CommandCreateVariable(string type, string varName)
        {
            this.type = type;
            this.varName = varName;
        }

        public void Execute(SchemeExecutor executor)
        {
            executor.CreateLocalVariable(type, varName, null);
        }

        public string GetAsString()
        {
            return "CREATE ( "+type+", "+varName+" )";
        }
        
        public void ChangeInputs(string current_val, string new_val) { }

        public bool HasOutput(string output_name)
        {
            return varName == output_name;
        }

        public void ChangeOutput(string current_val, string new_val) { }

        public bool HasInput(string input_name)
        {
            return false;
        }

        public string GetVariableName() { return varName; }

        public List<string> GetInputs()
        {
            return null;
        }

        public List<string> GetOutputs()
        {
            return new List<string> { varName };
        }

    }
}
