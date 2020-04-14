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

    }
}
