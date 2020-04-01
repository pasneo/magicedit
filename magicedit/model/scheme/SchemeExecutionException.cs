using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class SchemeExecutionException : Exception
    {

        public int CommandIndex { get; private set; } = -1;

        public SchemeExecutionException(string message) : base(message)
        {
        }

        public SchemeExecutionException(int commandIndex, string message) : base(message)
        {
            CommandIndex = commandIndex;
        }
    }
}
