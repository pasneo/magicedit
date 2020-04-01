using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandJump : ISchemeCommand
    {

        int line;

        public CommandJump(int line)
        {
            this.line = line;
        }

        public void Execute(SchemeExecutor executor)
        {
            executor.Jump(line);
        }

        public string GetAsString()
        {
            return $"JUMP ( {line} )";
        }

    }
}
