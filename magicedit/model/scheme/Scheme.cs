using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class Scheme
    {

        private string code;
        public string Code
        {
            get { return code; }
            //Modifying the code will invalidate the compiled scheme
            private set
            {
                code = Code;
                IsCompiledValid = false;
            }
        }

        private List<Scheme> Parents;

        private CompiledScheme compiledScheme;
        private CompiledScheme CompiledScheme
        {
            //Accessing CompiledScheme causes a recompilation if it is out of date
            get
            {
                if (!IsCompiledValid) Compile();
                return compiledScheme;
            }
            set
            {
                compiledScheme = value;
            }
        }

        public bool IsCompiledValid { get; set; } = false;

        /* *** */

        public void Compile()
        {
            //If compiled scheme is up to date, we don't need compiling
            if (IsCompiledValid) return;
            
            // compile
            //TODO
            IsCompiledValid = true;

            // compile parents
            foreach (var parent in Parents)
            {
                parent.Compile();
            }
        }

        public void Construct(Object @object, List<ObjectVariable> parameters)
        {
            //construct object based on this scheme
            CompiledScheme.Construct(@object, parameters);

            //call parents
            foreach (var parent in Parents)
            {
                parent.Construct(@object, parameters);
            }

        }

        public void Init(Object @object)
        {
            //initialize object with this scheme
            CompiledScheme.Init(@object);

            //call parents
            foreach (var parent in Parents)
            {
                parent.Init(@object);
            }
        }

        public SchemeFunction GetFunctionByName(string functionName)
        {
            SchemeFunction matchingFunction = CompiledScheme.GetFunctionByName(functionName);

            //If given name was not found in this scheme, we try the parents
            if (matchingFunction == null)
            {
                foreach(Scheme parent in Parents)
                {
                    matchingFunction = parent.GetFunctionByName(functionName);
                    if (matchingFunction != null) return matchingFunction;
                }
            }

            return null;

        }

    }
}
