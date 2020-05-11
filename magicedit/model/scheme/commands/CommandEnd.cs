using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandEnd : ISchemeCommand
    {
        public void Execute(SchemeExecutor executor)
        {
            executor.End();
        }

        public string GetAsString()
        {
            return $"END ( )";
        }

        public void ChangeInputs(string current_val, string new_val) { }

        public bool HasOutput(string output_name)
        {
            return false;
        }

        public void ChangeOutput(string current_val, string new_val)
        {
        }

        public bool HasInput(string input_name)
        {
            return false;
        }
    }
}
