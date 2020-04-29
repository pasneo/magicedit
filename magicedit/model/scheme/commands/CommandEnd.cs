using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandEnd : ISchemeCommand
    {
        public void Execute(SchemeExecutor executor)
        {
            executor.End();
        }

        public string GetAsString()
        {
            return $"END ( )";
        }
    }
}
