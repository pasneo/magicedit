using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class SchemeFunction
    {
        public string Name { get; set; }
        private List<ISchemeCommand> Commands = new List<ISchemeCommand>();
        public int ActionPoints { get; set; } = 0;

        /* *** */

        public SchemeFunction() { }

        public SchemeFunction(string name) { Name = name; }

        public SchemeFunction(string name, int actionPoints) { Name = name; ActionPoints = actionPoints; }

        //Executes function and returns the action points to be removed from current player
        public int Execute(Object @object, Object actor, Game game)
        {
            SchemeExecutor executor = new SchemeExecutor(@object, actor, Commands, game);
            if (executor.Execute()) return ActionPoints;
            return 0;
        }

        public void AddCommand(ISchemeCommand command)
        {
            Commands.Add(command);
        }

        public void AddCommand(ISchemeCommand command, int commandIndex)
        {
            Commands.Insert(commandIndex, command);
        }

        public int GetCommandCount()
        {
            return Commands.Count;
        }

        public string GetCode(int tab = 0)
        {
            string code = "";
            int line = 0;
            foreach (var command in Commands)
            {
                for (int t = 0; t < tab; ++t) code += "\t";
                code += $"{line++}\t {command.GetAsString()}\r\n";
            }
            return code;
        }

        public void Print()
        {
            int line = 0;
            foreach(var command in Commands)
            {
                Console.WriteLine(line++ + "\t" + command.GetAsString());
            }
        }

    }
}
