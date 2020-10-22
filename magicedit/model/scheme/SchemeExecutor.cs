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

        public Game Game { get; private set; }

        public Object Object { get; private set; }
        private Object Actor;
        private List<ISchemeCommand> Commands;

        public static int GlobalCommandIndex { get; private set; }  //command index for exception creation
        public int CommandIndex { get; private set; }           //no. of current command
        private bool CommandIndexChanged;   //Indicates wether a jump instruction changed current cmd index

        private bool ExecutionEnded = false;
        private bool ExecutionFailed = false;

        private Dictionary<string, ObjectVariable> Registers = new Dictionary<string, ObjectVariable>();
        private List<ObjectVariable> LocalVariables = new List<ObjectVariable>();

        public static readonly string[] ConstNames = { "actor", "me", "true", "false" };

        /* *** */

        public SchemeExecutor(Object @object, Object actor, List<ISchemeCommand> commands, Game game)
        {
            Object = @object;
            Actor = actor;
            Commands = commands;
            Game = game;
        }

        public static SchemeExecutionException CreateException(string message)
        {
            return new SchemeExecutionException(GlobalCommandIndex, message);
        }

        //Executes the contained function. Returns TRUE if actions is successful (and thus action points must be removed from actor)
        public bool Execute()
        {
            CommandIndex = 0;

            while (CommandIndex < Commands.Count)
            {
                CommandIndexChanged = false;

                GlobalCommandIndex = CommandIndex;
                Commands[CommandIndex].Execute(this);

                if (ExecutionEnded) return !ExecutionFailed;

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
            ExecutionEnded = true;
            ExecutionFailed = true;
        }

        public void End()
        {
            ExecutionEnded = true;
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

        public static bool IsRegister(string variableName)
        {
            return variableName.Length > 0 && variableName[0] == '_';
        }

        public static bool IsConst(string valueName)
        {
            return (ConstNames.Contains(valueName.ToLower())) ;
        }

        public static bool IsVariable(string valueName)
        {
            if (valueName == null || valueName.Length == 0 || valueName[0] == '$' || Char.IsNumber(valueName[0])) return false;
            if (IsConst(valueName)) return false;
            return true;
        }

        public ObjectVariable GetVariableByName(string name)
        {

            //If variable is "actor" or "me"
            if (name == "actor") return new ObjectVariable(Actor.Scheme.Name, "actor", Actor);
            if (name == "me") return new ObjectVariable(Object.Scheme?.Name == null ? VariableTypes.Object : Object.Scheme.Name, "me", Object);

            //If it is a register (_0, _1 ... _r0, _r1 ...) we return its value
            if (IsRegister(name))
            {
                if (Registers.ContainsKey(name)) return Registers[name];
                else throw CreateException($"Register '{name}' does not exist.");
            }

            //Check if object (me) has such variable
            ObjectVariable objectVariable = Object.GetVariableByName(name, Game.Config);
            if (objectVariable != null) return objectVariable;

            //Check local variables
            foreach (var localVariable in LocalVariables)
            {
                if (localVariable.Name == name) return localVariable;
            }

            //Check if it is a global object
            Object @object = Game.GetObjectById(name);

            if (@object != null)
            {
                //Type of an object is its scheme
                if (@object.Scheme?.Name != null)
                    return new ObjectVariable(@object.Scheme.Name, name, @object);
                return new ObjectVariable("unknown", name, @object);
            }

            //Check if variable is a classvar constant
            foreach(ClassList classlist in Game.Config.ClassLists)
            {
                foreach(Class @class in classlist.Classes)
                {
                    if (@class.Name == name)
                    {
                        return new ObjectVariable(classlist.Name, name, @class);
                    }
                }
            }

            return null;
        }

        public bool CheckTypeCompatibility(string variableType, string valueType)
        {
            return CheckTypeCompatibility(variableType, valueType, Game.Config);
        }

        // Checks if a variable with type variableType can be set to a variable with type valueType
        // (Is [variable = value] a valid statement?)
        public static bool CheckTypeCompatibility(string variableType, string valueType, Config config)
        {
            //Check if types are basic types (number, logical, text) or schemes

            if (variableType == VariableTypes.Number) return valueType == VariableTypes.Number;
            if (variableType == VariableTypes.Logical) return valueType == VariableTypes.Logical;
            if (variableType == VariableTypes.Text) return valueType == VariableTypes.Text;
            if (variableType == VariableTypes.Unknown || valueType == VariableTypes.Unknown) return false;

            //If variable is 'object', then a value with any object type can be assigned to it
            if (variableType == VariableTypes.Object)
            {
                if (valueType == VariableTypes.Object) return true;
                //Basic types are not compatible with 'object'
                if (valueType == VariableTypes.Number || valueType == VariableTypes.Logical || valueType == VariableTypes.Text) return false;
                Scheme valueScheme = config.GetSchemeByName(valueType);
                if (valueScheme == null) throw CreateException($"Unknown type '{valueType}'");
                return true;
            }

            //if variable is of a scheme type
            Scheme variableScheme = config.GetSchemeByName(variableType);

            if (variableScheme != null)
            {
                //If scheme exists and the two are equal, they must be compatible
                if (variableType == valueType) return true;

                //Basic types are not compatible with any scheme
                if (valueType == VariableTypes.Number || valueType == VariableTypes.Logical || valueType == VariableTypes.Text) return false;

                Scheme valueScheme = config.GetSchemeByName(valueType);

                if (valueScheme == null) throw CreateException($"Unknown type '{valueType}'");

                //Check if the two (different) schemes are compatible (ie. variableType is the ancestor of valueType)
                return valueScheme.HasAncestor(variableType);
            }

            //if variable type is class type (classlist)
            if (config.IsClassType(variableType)) return variableType == valueType;

            throw CreateException($"Unknown type '{variableType}'");

        }

        // Finds the type and value represented by the given string (including variables; and constants like 11, 5, true, $STRING etc.)
        // This method calculates the current value of abilities and return that value (thus cannot be used to change ability values)
        public ObjectVariable FindValueByString(string s)
        {
            //If there is a variable with the name s, we return its value
            ObjectVariable variable = GetVariableByName(s);
            if (variable != null)
            {
                return variable;
            }

            //If s is a number const we return it as a new number variable
            int number;
            bool canConvert = int.TryParse(s, out number);
            if (canConvert) return new ObjectVariable("number", "", number);

            //If s is a logical const, we return it as a new logical variable
            if (s.ToLower() == "true") return new ObjectVariable("logical", "", true);
            if (s.ToLower() == "false") return new ObjectVariable("logical", "", false);

            //If s is a string const we return its content as a text variable
            if (s.Length >= 1 && s[0] == '$') return new ObjectVariable("text", "", Game.Config.GetStringConstByName(s));

            //If s is a class variable we return it as class
            var c = Game.Config.GetClassByName(s);
            if (c != null)
            {
                return new ObjectVariable(c.Item1.Name, "", c.Item2);
            }

            throw CreateException($"Unidentifyable value '{s}'");
        }

        public void SetVariable(string variableName, string valueName)
        {
            //Check if variable is a register
            if (IsRegister(variableName))
            {
                //First we get value, and create register with same type and value
                ObjectVariable value = FindValueByString(valueName);
                Registers[variableName] = new ObjectVariable(value.Type, variableName, value.Value);
            }
            else
            {
                ObjectVariable variable = GetVariableByName(variableName);

                if (variable != null)
                {
                    ObjectVariable value = FindValueByString(valueName);

                    if (!CheckTypeCompatibility(variable.Type, value.Type, Game.Config))
                        throw CreateException("Incompatible types");

                    variable.Value = value.Value;

                }
                else
                    throw CreateException($"Unidentifyable variable '{variableName}'");
            }
        }

        public void SetVariable(string variableName, ObjectVariable value)
        {
            //Check if variable is a register
            if (IsRegister(variableName))
            {
                //First we get value, and create register with same type and value
                Registers[variableName] = new ObjectVariable(value.Type, variableName, value.Value);
            }
            else
            {
                ObjectVariable variable = GetVariableByName(variableName);

                if (variable != null)
                {
                    if (!CheckTypeCompatibility(variable.Type, value.Type, Game.Config))
                        throw CreateException("Incompatible types");

                    variable.Value = value.Value;
                }
                else
                    throw CreateException($"Unidentifyable variable '{variableName}'");
            }

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

            return @object.GetVariableByName(propertyName, Game.Config);
        }

    }
}
