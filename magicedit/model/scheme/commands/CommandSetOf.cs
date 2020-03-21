using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandSetOf : ISchemeCommand
    {

        string propertyName, objectName, valueName;

        public CommandSetOf(string propertyName, string objectName, string valueName)
        {
            this.propertyName = propertyName;
            this.objectName = objectName;
            this.valueName = valueName;
        }

        public void Execute(SchemeExecutor executor)
        {
            ObjectVariable property = executor.GetPropertyOf(propertyName, objectName);
            ObjectVariable value = executor.FindValueByString(valueName);
            property.Value = value.Value;
        }

        public string GetAsString()
        {
            return "SET OF ( " + propertyName + ", " + objectName + ", " + valueName + " )";
        }

    }
}
