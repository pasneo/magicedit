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
        private Object Actor;
        private List<ISchemeCommand> Commands;

        private int CommandIndex;           //no. of current command
        private bool CommandIndexChanged;   //Indicates wether a jump instruction changed current cmd index

        private List<object> Registers = new List<object>();
        private List<ObjectVariable> LocalVariables = new List<ObjectVariable>();

        /* *** */

        public SchemeExecutor(Object @object, Object actor, List<ISchemeCommand> commands)
        {
            Object = @object;
            Actor = actor;
            Commands = commands;
        }

        public void Execute()
        {
            CommandIndex = 0;

            while(CommandIndex < Commands.Count)
            {
                CommandIndexChanged = false;

                Commands[CommandIndex].Execute(this);
                
                //If command index was NOT changed by a jump instruction, we go to the next instruction
                if (!CommandIndexChanged)
                {
                    ++CommandIndex;
                }
            }

        }

        public void CreateLocalVariable(string type, string name, object value)
        {
            ObjectVariable variable = new ObjectVariable(type, name, value);
            LocalVariables.Add(variable);
        }

        public ObjectVariable GetVariableByName(string name)
        {

            //If variable is "actor" or "me"
            if (name == "actor") return new ObjectVariable("object", "actor", Actor);
            if (name == "me") return new ObjectVariable("object", "me", Object);

            //TODO: If it is a register (_0, _1 ... _r0, _r1 ...) we return its value

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

                if (valueScheme == null) throw new SchemeExecutionException("Unknown type");

                //Check if the two (different) schemes are compatible (ie. variableType is the ancestor of valueType)
                return valueScheme.HasAncestor(variableType);
            }

            throw new SchemeExecutionException("Unknown type");

        }

        // Finds the type and value represented by the given string
        public ObjectVariable FindValueByString(string s)
        {
            //If there is a variable with the name s, we return its value
            ObjectVariable variable = GetVariableByName(s);
            if (variable != null) return variable;

            //If s is a number const we return it as a new number variable
            int number;
            bool canConvert = int.TryParse(s, out number);
            if (canConvert) return new ObjectVariable("number", "", number);

            //If s is a logical const, we return it as a new logical variable
            if (s == "true") return new ObjectVariable("logical", "", true);
            if (s == "false") return new ObjectVariable("logical", "", false);

            //If s is a string const we return its content as a text variable
            if (s.Length >= 1 && s[0] == '$') return new ObjectVariable("text", "", Config.GetStringConstByName(s));

            throw new SchemeExecutionException("Unidentifyable value");
        }

        public void SetVariable(string variableName, string valueName)
        {
            ObjectVariable variable = GetVariableByName(variableName);

            if (variable != null)
            {
                ObjectVariable value = FindValueByString(valueName);

                if (!CheckTypeCompatibility(variable.Type, value.Type))
                    throw new SchemeExecutionException("Incompatible types");

                variable.Value = value.Value;

            }
            else
                throw new SchemeExecutionException("Unidentifyable variable");
        }

        //Returns the given property (as reference) of the object identified by objectName
        public ObjectVariable GetPropertyOf(string propertyName, string objectName)
        {
            //First we identify the object
            Object @object = null;

            ObjectVariable objectVariable = GetVariableByName(objectName);

            if (objectVariable == null)
                throw new SchemeExecutionException("Object not found");

            @object = (Object)objectVariable.Value;

            return @object.GetVariableByName(propertyName);
        }

    }
}
