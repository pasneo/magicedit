using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandOf : ISchemeCommand
    {

        string propertyName, objectName, target;

        public CommandOf(string propertyName, string objectName, string target)
        {
            this.propertyName = propertyName;
            this.objectName = objectName;
            this.target = target;
        }

        public void Execute(SchemeExecutor executor)
        {
            ObjectVariable property = executor.GetPropertyOf(propertyName, objectName);
            executor.SetVariable(target, property);
        }

        public string GetAsString()
        {
            return "OF ( " + propertyName + ", " + objectName + ", " + target + " )";
        }

    }
}
