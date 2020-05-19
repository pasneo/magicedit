using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandSetVariable : ISchemeCommand
    {

        string variable, value;

        public CommandSetVariable(string variable, string value)
        {
            this.variable = variable;
            this.value = value;
        }

        public void Execute(SchemeExecutor executor)
        {
            executor.SetVariable(variable, value);
        }

        public string GetAsString()
        {
            return "SET ( " + variable + ", " + value + " )";
        }

        public string GetVariableName() { return variable; }
        public string GetValue() { return value; }

        public void ChangeInputs(string current_val, string new_val)
        {
            if (value == current_val) value = new_val;
        }

        public void ChangeOutput(string current_val, string new_val)
        {
            if (variable == current_val) variable = new_val;
        }

        public bool HasOutput(string output_name)
        {
            return variable == output_name;
        }

        public bool HasInput(string input_name)
        {
            return value == input_name;
        }

        public List<string> GetInputs()
        {
            return new List<string> { value };
        }

        public List<string> GetOutputs()
        {
            return new List<string> { variable };
        }

    }
}
