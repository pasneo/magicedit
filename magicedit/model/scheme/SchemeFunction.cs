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
            SchemeExecutor executor = new SchemeExecutor(@object, actor);
            foreach(ISchemeCommand command in Commands)
            {
                command.Execute(executor);
            }
        }

        public void AddCommand(ISchemeCommand command)
        {
            Commands.Add(command);
        }

    }
}
