using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandJumpIfFalse : ISchemeCommand
    {

        string value;
        int line;

        public CommandJumpIfFalse(string value, int line)
        {
            this.value = value;
            this.line = line;
        }

        public void Execute(SchemeExecutor executor)
        {
            ObjectVariable variable = executor.FindValueByString(value);

            if (variable.Type != VariableTypes.Logical) throw SchemeExecutor.CreateException("Cannot use non-logical value in JF");

            bool value_bool = (bool)variable.Value;

            if (!value_bool)
                executor.Jump(line);
        }

        public string GetAsString()
        {
            return $"JF ( {value}, {line} )";
        }

    }
}
