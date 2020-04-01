using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandAdd : CommandOperation
    {

        public CommandAdd(string value1, string value2, string target) : base(value1, value2, target)
        {
        }

        public override string GetAsString()
        {
            return "ADD ( " + value1 + ", " + value2 + ", " + target + " )";
        }

        protected override int GetResult(int value1, int value2)
        {
            return value1 + value2;
        }
    }
}
