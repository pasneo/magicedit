using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public abstract class CommandJumpBase : ISchemeCommand
    {

        public int Line { get; private set; }

        public CommandJumpBase(int line)
        {
            this.Line = line;
        }

        public void Execute(SchemeExecutor executor)
        {
            if (Evaluate(executor))
                executor.Jump(Line);
        }

        public void SetLine(int line)
        {
            this.Line = line;
        }

        protected abstract bool Evaluate(SchemeExecutor executor);

        public abstract string GetAsString();

        public virtual void ChangeInputs(string current_val, string new_val)
        {
        }

        public virtual void ChangeOutput(string current_val, string new_val)
        {
        }

        public virtual bool HasOutput(string output_name)
        {
            return false;
        }

        public virtual bool HasInput(string input_name)
        {
            return false;
        }

        public virtual List<string> GetInputs()
        {
            return null;
        }

        public virtual List<string> GetOutputs()
        {
            return null;
        }

    }
}
