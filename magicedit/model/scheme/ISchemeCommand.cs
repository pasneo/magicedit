using System.Collections.Generic;

namespace magicedit
{

    public enum SchemeCommandType
    {
        None,

        CreateVariable,
        CreateClassVariable,

        SetVariable,

        Add,
        Subtract,
        Multiply,
        Divide


    }

    public interface ISchemeCommand
    {
        //public SchemeCommandType Type;
        //public List<object> Parameters;

        void Execute(SchemeExecutor executor);
        string GetAsString();

        void ChangeInputs(string current_val, string new_val);
        void ChangeOutput(string current_val, string new_val);
        bool HasOutput(string output_name);
        bool HasInput(string input_name);

    }
}