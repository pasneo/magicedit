using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandJumpIfFalse : CommandJumpBase
    {

        string value;

        public CommandJumpIfFalse(string value, int line) : base(line)
        {
            this.value = value;
        }

        protected override bool Evaluate(SchemeExecutor executor)
        {
            ObjectVariable variable = executor.FindValueByString(value);

            if (variable.Type != VariableTypes.Logical) throw SchemeExecutor.CreateException("Cannot use non-logical value in JF");

            bool value_bool = (bool)variable.Value;

            return !value_bool;
        }

        public override string GetAsString()
        {
            return $"JF ( {value}, {Line} )";
        }

    }
}
