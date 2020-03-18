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

        public void Execute(SchemeExecutor executioner)
        {
            throw new NotImplementedException();
        }

        public string GetAsString()
        {
            return "SET ( " + variable + ", " + value + " )";
        }
    }
}
