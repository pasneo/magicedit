using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public abstract class CommandLogicalOperation : ISchemeCommand
    {

        protected string value1, value2, target;

        public CommandLogicalOperation(string value1, string value2, string target)
        {
            this.value1 = value1;
            this.value2 = value2;
            this.target = target;
        }


        public void Execute(SchemeExecutor executor)
        {
            ObjectVariable variable1 = executor.FindValueByString(value1);
            ObjectVariable variable2 = executor.FindValueByString(value2);

            if (variable1.Type != VariableTypes.Logical)
                throw new Exception("Cannot do logical operation with non-logical variables");

            bool value1_bool = (bool)variable1.Value;
            bool value2_bool = (bool)variable2.Value;

            bool result = GetResult(value1_bool, value2_bool);

            executor.SetVariable(target, new ObjectVariable(VariableTypes.Logical, "", result));
        }

        protected abstract bool GetResult(bool value1, bool value2);
        public abstract string GetAsString();

    }
}
