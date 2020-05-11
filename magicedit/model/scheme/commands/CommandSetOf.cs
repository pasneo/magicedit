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

        public void ChangeInputs(string current_val, string new_val)
        {
            if (valueName == current_val) valueName = new_val;
            if (objectName == current_val) objectName = new_val;
        }

        public void ChangeOutput(string current_val, string new_val)
        {
        }

        public bool HasOutput(string output_name)
        {
            return false;
        }

        public bool HasInput(string input_name)
        {
            return (valueName == input_name) || (objectName == input_name);
        }

    }
}
