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

        public void ChangeInputs(string current_val, string new_val)
        {
        }

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
