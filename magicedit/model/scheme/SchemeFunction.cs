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
        public int ActionPoints { get; set; }

        /* *** */

        public SchemeFunction() { }

        public SchemeFunction(string name) { Name = name; }

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
