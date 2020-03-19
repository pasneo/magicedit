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

    }
}
