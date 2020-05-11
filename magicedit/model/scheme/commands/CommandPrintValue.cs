using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandPrintValue : ISchemeCommand
    {

        string valueName;

        public CommandPrintValue(string valueName)
        {
            this.valueName = valueName;
        }

        public void Execute(SchemeExecutor executor)
        {
            var variable = executor.FindValueByString(valueName);
            Console.WriteLine(variable.Name + " = " + variable.Value);
        }

        public string GetAsString()
        {
            return "PRINT ( " + valueName + " )";
        }

        public void ChangeInputs(string current_val, string new_val)
        {
            if (valueName == current_val) valueName = new_val;
        }

        public void ChangeOutput(string current_val, string new_val)
        {
        }

        public bool HasInput(string input_name)
        {
            return valueName == input_name;
        }

        public bool HasOutput(string output_name)
        {
            return false;
        }

    }
}
