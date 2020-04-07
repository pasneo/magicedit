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

        public Config Config { get; private set; }

        public Object Object { get; private set; }
        private Object Actor;
        private List<ISchemeCommand> Commands;

        public static int GlobalCommandIndex { get; private set; }  //command index for exception creation
        public int CommandIndex { get; private set; }           //no. of current command
        private bool CommandIndexChanged;   //Indicates wether a jump instruction changed current cmd index
        private bool ExecutionFailed = false;

        private List<object> Registers = new List<object>();
        private List<ObjectVariable> LocalVariables = new List<ObjectVariable>();

        /* *** */

        public SchemeExecutor(Object @object, Object actor, List<ISchemeCommand> commands, Config config)
        {
            Object = @object;
            Actor = actor;
            Commands = commands;
            Config = config;
        }

        public static SchemeExecutionException CreateException(string message)
        {
            return new SchemeExecutionException(GlobalCommandIndex, message);
        }

        public bool Execute()
        {
            CommandIndex = 0;

            while(CommandIndex < Commands.Count)
            {
                CommandIndexChanged = false;

                GlobalCommandIndex = CommandIndex;
                Commands[CommandIndex].Execute(this);

                if (ExecutionFailed) return false;

                //If command index was NOT changed by a jump instruction, we go to the next instruction
                if (!CommandIndexChanged)
                {
                    ++CommandIndex;
                }
            }

            return true;

        }

        public void Fail()
        {
            ExecutionFailed = true;
        }

        public void Jump(int index)
        {
            CommandIndex = index;
            CommandIndexChanged = true;
        }

        public void CreateLocalVariable(string type, string name, object value)
        {

            if (GetVariableByName(name) != null) throw CreateException($"Variable '{name}' already exists");

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

                if (valueScheme == null) throw CreateException("Unknown type");

                //Check if the two (different) schemes are compatible (ie. variableType is the ancestor of valueType)
                return valueScheme.HasAncestor(variableType);
            }

            throw CreateException("Unknown type");

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
            if (s == "true" || s == "True") return new ObjectVariable("logical", "", true);
            if (s == "false" || s == "False") return new ObjectVariable("logical", "", false);

            //If s is a string const we return its content as a text variable
            if (s.Length >= 1 && s[0] == '$') return new ObjectVariable("text", "", Config.GetStringConstByName(s));

            throw CreateException($"Unidentifyable value '{s}'");
        }

        public void SetVariable(string variableName, string valueName)
        {
            ObjectVariable variable = GetVariableByName(variableName);

            if (variable != null)
            {
                ObjectVariable value = FindValueByString(valueName);

                if (!CheckTypeCompatibility(variable.Type, value.Type))
                    throw CreateException("Incompatible types");

                variable.Value = value.Value;

            }
            else
                throw CreateException($"Unidentifyable variable '{variableName}'");
        }

        //Returns the given property (as reference) of the object identified by objectName
        public ObjectVariable GetPropertyOf(string propertyName, string objectName)
        {
            //First we identify the object
            Object @object = null;

            ObjectVariable objectVariable = GetVariableByName(objectName);

            if (objectVariable == null)
                throw CreateException("Object not found");

            @object = (Object)objectVariable.Value;

            return @object.GetVariableByName(propertyName);
        }

    }
}
