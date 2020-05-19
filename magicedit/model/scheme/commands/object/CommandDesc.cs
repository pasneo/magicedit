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

        public void ChangeInputs(string current_val, string new_val)
        {
        }

        public void ChangeOutput(string current_val, string new_val)
        {
        }

        public bool HasOutput(string output_name)
        {
            return false;
        }

        public bool HasInput(string input_name)
        {
            return false;
        }

        public List<string> GetInputs()
        {
            return null;
        }

        public List<string> GetOutputs()
        {
            return null;
        }

    }
}
