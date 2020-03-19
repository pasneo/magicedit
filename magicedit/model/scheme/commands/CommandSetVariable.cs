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
    }
}
