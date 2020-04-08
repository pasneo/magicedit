using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandDesc : ISchemeCommand
    {

        string descStringConstName;

        public CommandDesc(string descStringConstName)
        {
            this.descStringConstName = descStringConstName;
        }

        public void Execute(SchemeExecutor executor)
        {
            executor.Object.Description = executor.Game.Config.GetStringConstByName(descStringConstName);
        }

        public string GetAsString()
        {
            return $"DESC ( {descStringConstName} )";
        }

    }
}
