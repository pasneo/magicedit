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

    }
}