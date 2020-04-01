using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public abstract class CommandComparison : ISchemeCommand
    {

        protected string value1, value2, target;

        public CommandComparison(string value1, string value2, string target)
        {
            this.value1 = value1;
            this.value2 = value2;
            this.target = target;
        }


        public void Execute(SchemeExecutor executor)
        {
            ObjectVariable variable1 = executor.FindValueByString(value1);
            ObjectVariable variable2 = executor.FindValueByString(value2);

            bool result = GetResult(variable1, variable2);

            executor.SetVariable(target, result.ToString());
        }

        protected abstract bool GetResult(ObjectVariable variable1, ObjectVariable variable2);
        public abstract string GetAsString();

    }
}
