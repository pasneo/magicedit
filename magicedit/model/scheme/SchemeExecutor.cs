using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{

    /*
     * This class is responsible for storing variables and states during the 
     * execution of a scheme function. Commands during execution reach and
     * modify these states via the SchemeExecutor.
     * 
     */

    public class SchemeExecutor
    {

        private Config Config;

        private Object Object;
        private List<ObjectVariable> LocalVariables;

        /* *** */

        public void CreateLocalVariable(string type, string name, object value)
        {
            ObjectVariable variable = new ObjectVariable(type, name, value);
            LocalVariables.Add(variable);
        }

        public ObjectVariable GetVariableByName(string name)
        {
            //First we check if object has such variable
            ObjectVariable objectVariable = Object.GetVariableByName(name);
            if (objectVariable != null) return objectVariable;

            //If variable was not found, we check local variables
            foreach (var localVariable in LocalVariables)
            {
                if (localVariable.Name == name) return localVariable;
            }

            return null;
        }

        // Checks if a variable with type variableType can be set to a variable with type valueType
        // (Is [variable = value] a valid statement?)
        public bool CheckTypeCompatibility(string variableType, string valueType)
        {
            //Check if types are basic types (number, logical, text) or schemes

            if (variableType == "number") return valueType == "number";
            if (variableType == "logical") return valueType == "logical";
            if (variableType == "text") return valueType == "text";

            Scheme variableScheme = Config.GetSchemeByName(variableType);

            if (variableScheme != null)
            {
                //If scheme exists and the two are equal, they must be compatible
                if (variableType == valueType) return true;

                //Basic types are not compatible with any scheme
                if (valueType == "number" || valueType == "logical" || valueType == "text") return false;

                Scheme valueScheme = Config.GetSchemeByName(valueType);

                if (valueScheme == null) throw new Exception("Unknown type");

                //Check if the two (different) schemes are compatible (ie. variableType is the ancestor of valueType)
                return valueScheme.HasAncestor(variableType);
            }

            throw new Exception("Unknown type");

        }

        // Finds the type and value represented by the given string
        public ObjectVariable FindValueByString(string s)
        {
            //If there is a variable with the name s, we return its value
            ObjectVariable variable = GetVariableByName(s);
            if (variable != null) return variable;

            //If s is a number we return it as a new number variable
            int number;
            bool canConvert = int.TryParse(s, out number);
            if (canConvert) return new ObjectVariable("number", "", number);

            //If s is a logical const, we return it as a new logical variable
            if (s == "true") return new ObjectVariable("logical", "", true);
            if (s == "false") return new ObjectVariable("logical", "", false);

            //If s is a string const
            if (s.Length >= 1 && s[0] == '$') return new ObjectVariable("text", "", Config.GetStringConstByName(s));

            throw new Exception("Unidentifyable value");
        }

        public void SetVariable(string variableName, string valueName)
        {
            ObjectVariable variable = GetVariableByName(variableName);

            if (variable != null)
            {
                ObjectVariable value = FindValueByString(valueName);

                if (!CheckTypeCompatibility(variable.Type, value.Type))
                    throw new Exception("Incompatible types");

                variable.Value = value.Value;

            }
        }

    }
}
