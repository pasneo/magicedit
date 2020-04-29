using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandReport : ISchemeCommand
    {

        string messageStringConstName;

        public CommandReport(string messageStringConstName)
        {
            this.messageStringConstName = messageStringConstName;
        }

        public void Execute(SchemeExecutor executor)
        {
            Console.WriteLine(executor.Game.Config.GetStringConstByName(messageStringConstName).Content);
        }

        public string GetAsString()
        {
            return $"REPORT ( {messageStringConstName} )";
        }
    }
}
