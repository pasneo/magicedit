using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandAnd : CommandLogicalOperation
    {

        public CommandAnd(string value1, string value2, string target) : base(value1, value2, target)
        {
        }

        public override string GetAsString()
        {
            return $"AND ( {value1}, {value2}, {target} )";
        }

        protected override bool GetResult(bool value1, bool value2)
        {
            return value1 && value2;
        }

    }
}
