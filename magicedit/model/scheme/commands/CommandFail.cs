using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandFail : ISchemeCommand
    {
        public void Execute(SchemeExecutor executor)
        {
            executor.Fail();
        }

        public string GetAsString()
        {
            return $"FAIL ( )";
        }
    }
}
