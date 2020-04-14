using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandRemoveAction : ISchemeCommand
    {

        string actionName;

        public CommandRemoveAction(string actionName)
        {
            this.actionName = actionName;
        }

        public void Execute(SchemeExecutor executor)
        {
            executor.Object.RemoveAction(actionName);
        }

        public string GetAsString()
        {
            return $"REMOVE ACTION ( {actionName} )";
        }

    }
}
