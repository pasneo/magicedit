using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandClearActions : ISchemeCommand
    {

        public CommandClearActions() { }

        public void Execute(SchemeExecutor executor)
        {
            executor.Object.ClearActions();
        }

        public string GetAsString()
        {
            return $"CLEAR ACTIONS ( )";
        }
    }
}
