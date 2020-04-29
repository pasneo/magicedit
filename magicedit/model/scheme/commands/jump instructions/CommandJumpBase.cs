using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public abstract class CommandJumpBase : ISchemeCommand
    {

        protected int line;

        public CommandJumpBase(int line)
        {
            this.line = line;
        }

        public void Execute(SchemeExecutor executor)
        {
            if (Evaluate(executor))
                executor.Jump(line);
        }

        public void SetLine(int line)
        {
            this.line = line;
        }

        protected abstract bool Evaluate(SchemeExecutor executor);

        public abstract string GetAsString();

    }
}
