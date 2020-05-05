using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandCmp : CommandComparison
    {

        string op;

        public CommandCmp(string value1, string op, string value2, string target) : base(value1, value2, target)
        {
            this.op = op;
        }

        public override string GetAsString()
        {
            return $"CMP ( {value1}, {op}, {value2}, {target} )";
        }

        protected override bool GetResult(ObjectVariable variable1, ObjectVariable variable2)
        {
            if (variable1.Type != VariableTypes.Number || variable2.Type != VariableTypes.Number)
                throw SchemeExecutor.CreateException("Cannot compare non-numbers");

            int value1_int = (int)variable1.Value;
            int value2_int = (int)variable2.Value;

            if (op == ">") return value1_int > value2_int;
            if (op == "<") return value1_int < value2_int;
            if (op == ">=") return value1_int >= value2_int;
            if (op == "<=") return value1_int <= value2_int;
            if (op == "!=") return value1_int != value2_int;
            if (op == "=") return value1_int == value2_int;

            return false;

        }

    }
}
