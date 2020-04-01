using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandEquals : CommandComparison
    {

        public CommandEquals(string value1, string value2, string target) : base(value1, value2, target)
        {
        }

        public override string GetAsString()
        {
            return $"EQUALS ( {value1}, {value2}, {target} )";
        }

        protected override bool GetResult(ObjectVariable variable1, ObjectVariable variable2)
        {
            //In case of numbers and logicals we compare values
            if (variable1.Type == VariableTypes.Number)
            {
                if (variable2.Type != VariableTypes.Number) throw SchemeExecutor.CreateException("Cannot compare number and non-number");
                int value1_int = (int)variable1.Value;
                int value2_int = (int)variable2.Value;
                return value1_int == value2_int;
            }
            if (variable1.Type == VariableTypes.Logical)
            {
                if (variable2.Type != VariableTypes.Logical) throw SchemeExecutor.CreateException("Cannot compare logical and non-logical");
                bool value1_bool = (bool)variable1.Value;
                bool value2_bool = (bool)variable2.Value;
                return value1_bool == value2_bool;
            }

            //In case of other types, we compare if they are the same object
            return variable1.Value == variable2.Value;
        }

    }
}
