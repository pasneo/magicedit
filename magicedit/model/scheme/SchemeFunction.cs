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

        public void Execute(Object @object, Object actor)
        {
            SchemeExecutor executor = new SchemeExecutor(@object, actor, Commands);
            executor.Execute();
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
