using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    class CommandForbidAttribute : ISchemeCommand
    {

        string objectName, attributeName;

        public CommandForbidAttribute(string objectName, string attributeName)
        {
            this.objectName = objectName;
            this.attributeName = attributeName;
        }

        public void Execute(SchemeExecutor executor)
        {
            ObjectVariable objectVariable = executor.GetVariableByName(objectName);

            if (!executor.CheckTypeCompatibility(VariableTypes.Object, objectVariable.Type))
                throw SchemeExecutor.CreateException($"Object has to be compatible with 'object' (actual type: '{objectVariable.Type}')");

            Object @object = (Object)objectVariable.Value;

            @object.ForbidAttribute(attributeName);
        }

        public string GetAsString()
        {
            return $"FORBID ATTR ( {objectName}, {attributeName} )";
        }
    }
}
