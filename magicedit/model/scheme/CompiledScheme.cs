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
        private List<ObjectVariable> Parameters = new List<ObjectVariable>();

        private SchemeFunction InitFunction;
        private List<SchemeFunction> ActionFunctions = new List<SchemeFunction>();

        /* *** */

        public CompiledScheme() { }

        public CompiledScheme(string code)
        {
            var ast = SchemeLang.GetAST(code);
        }

        public void AddVariable(ObjectVariable variable)
        {
            Variables.Add(variable);
        }

        public void AddParameter(ObjectVariable parameter)
        {
            Parameters.Add(parameter);
        }

        public void AddAction(SchemeFunction action)
        {
            ActionFunctions.Add(action);
        }

        //Creates variables in object, fills parameter variables with given values
        public void Construct(Object @object, List<ObjectVariable> parameters)
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

    }
}
