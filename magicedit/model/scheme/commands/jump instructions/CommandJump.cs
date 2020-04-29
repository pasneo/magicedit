using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandJump : CommandJumpBase
    {

        public CommandJump(int line) : base(line)
        {
        }

        protected override bool Evaluate(SchemeExecutor executor)
        {
            return true;
        }

        public override string GetAsString()
        {
            return $"JUMP ( {line} )";
        }

    }
}
