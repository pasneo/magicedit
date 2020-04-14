using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandReport : ISchemeCommand
    {

        Text message;

        public CommandReport(Text message)
        {
            this.message = message;
        }

        public void Execute(SchemeExecutor executor)
        {
            Console.WriteLine(message.Content);
        }

        public string GetAsString()
        {
            return $"REPORT ( {message} )";
        }
    }
}
