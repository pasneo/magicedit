using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandPrint : ISchemeCommand
    {

        string text;

        public CommandPrint(string text)
        {
            this.text = text;
        }

        public void Execute(SchemeExecutor executor)
        {
            Console.WriteLine(text);
        }

        public string GetAsString()
        {
            return "PRINT ( '" + text + "' )";
        }

        public void ChangeInputs(string current_val, string new_val)
        {
        }

        public void ChangeOutput(string current_val, string new_val)
        {
        }

        public bool HasInput(string input_name)
        {
            return false;
        }

        public bool HasOutput(string output_name)
        {
            return false;
        }

    }
}
