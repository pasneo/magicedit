using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CompiledScheme
    {

        //Variables that are created when constructing object
        private List<ObjectVariable> Variables = new List<ObjectVariable>();
        //Parameters that are created and set when constructing object
        public List<ObjectVariable> Parameters { get; private set; } = new List<ObjectVariable>();
        
        public SchemeFunction BodyFunction { get; private set; } = null;    //This is created from code in the scheme's body (outside of any other function)
        public SchemeFunction InitFunction { get; private set; } = null;
        
        public List<SchemeFunction> ActionFunctions { get; private set; } = new List<SchemeFunction>();

        /* *** */

        public CompiledScheme() { }

        public void AddVariable(ObjectVariable variable)
        {
            Variables.Add(variable);
        }

        public ObjectVariable GetVariableByName(string name)
        {
            return Variables.Where(v => v.Name == name).FirstOrDefault();
        }

        public void AddParameter(ObjectVariable parameter)
        {
            Parameters.Add(parameter);
        }

        public ObjectVariable GetParameterByName(string name)
        {
            return Parameters.Where(p => p.Name == name).FirstOrDefault();
        }

        public void SetBody(SchemeFunction bodyFunction)
        {
            BodyFunction = bodyFunction;
        }

        public void SetInit(SchemeFunction init)
        {
            InitFunction = init;
        }

        public void AddAction(SchemeFunction action)
        {
            ActionFunctions.Add(action);
        }

        //Creates variables in object, fills parameter variables with given values
        public void Construct(Object @object, List<ObjectVariable> parameters, Game game)
        {

            foreach (ObjectVariable variable in Variables)
            {
                @object.Variables.Add(variable);
            }

            if (Parameters.Count > parameters.Count)
                throw new SchemeExecutionException($"Not all parameters are set for object with id '{@object.Id}'");

            foreach (ObjectVariable param in parameters)
            {
                //Check if parameter is present in the Parameters list
                if (Parameters.Any(p => (p.Name == param.Name && p.Type == param.Type)))
                {
                    @object.Variables.Add(param);
                }
            }
            
            BodyFunction?.Execute(@object, null, game);

        }

        public void Init(Object @object, Game game)
        {
            InitFunction?.Execute(@object, null, game);
        }

        //Returns the function that matches the given name, or null if none matches
        public SchemeFunction GetFunctionByName(string functionName)
        {
            if (InitFunction != null && InitFunction.Name == functionName) return InitFunction;

            foreach(SchemeFunction actionFunction in ActionFunctions)
            {
                if (actionFunction.Name == functionName) return actionFunction;
            }

            return null;

        }

        public string GetFullCode()
        {
            string code = "scheme {\r\n";

            foreach(ObjectVariable param in Parameters)
            {
                code += $"\tparam {param.Type} {param.Name}\r\n";
            }

            foreach(ObjectVariable variable in Variables)
            {
                code += $"\t{variable.Type} {variable.Name}\r\n";
            }

            code += "\r\n";
            if (BodyFunction != null)
            {
                string bodyCode = BodyFunction.GetCode(1);
                if (bodyCode != "") code += bodyCode + "\r\n";
            }

            code += "\tinit {\r\n";
            if (InitFunction != null) code += InitFunction.GetCode(2);
            code += "\t}\r\n";

            foreach(SchemeFunction action in ActionFunctions)
            {
                code += $"\r\n\taction {action.Name} ({action.ActionPoints}) " + "{\r\n";
                code += action.GetCode(2);
                code += "\t}\r\n";
            }

            code += "}";
            return code;

        }

    }
}
