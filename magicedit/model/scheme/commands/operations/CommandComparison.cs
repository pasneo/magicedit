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

            executor.SetVariable(target, new ObjectVariable(VariableTypes.Logical, "", result));
        }

        protected abstract bool GetResult(ObjectVariable variable1, ObjectVariable variable2);
        public abstract string GetAsString();

        public void ChangeInputs(string current_val, string new_val)
        {
            if (value1 == current_val) value1 = new_val;
            if (value2 == current_val) value2 = new_val;
        }

        public bool HasOutput(string output_name)
        {
            return target == output_name;
        }

        public void ChangeOutput(string current_val, string new_val)
        {
            if (target == current_val) target = new_val;
        }

        public bool HasInput(string input_name)
        {
            return (value1 == input_name) || (value2 == input_name);
        }

        public List<string> GetInputs()
        {
            return new List<string> { value1, value2 };
        }

        public List<string> GetOutputs()
        {
            return new List<string> { target };
        }

    }
}
