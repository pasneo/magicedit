using System.Collections.Generic;

namespace magicedit
{

    public enum SchemeCommandType
    {
        None
    }

    public class SchemeCommand
    {
        public SchemeCommandType Type;
        public List<object> Parameters;
    }
}