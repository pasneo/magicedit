using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandNot : ISchemeCommand
    {
        string value, target;

        public CommandNot(string value, string target)
        {
            this.value = value;
            this.target = target;
        }

        public void Execute(SchemeExecutor executor)
        {
            ObjectVariable variable = executor.FindValueByString(value);

            if (variable.Type != VariableTypes.Logical) throw SchemeExecutor.CreateException("Cannot invert non-logical");

            bool value_bool = (bool)variable.Value;
            executor.SetVariable(target, new ObjectVariable(VariableTypes.Logical, "", (!value_bool)));
        }

        public string GetAsString()
        {
            return $"NOT ( {value}, {target} )";
        }
    }
}
