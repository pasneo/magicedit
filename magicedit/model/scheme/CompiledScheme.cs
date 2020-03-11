using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CompiledScheme
    {
        private SchemeFunction InitFunction;
        private List<SchemeFunction> ActionFunctions;

        /* *** */

        public void Construct(Object @object, List<ObjectVariable> parameters)
        {
            //TODO
        }

        public void Init(Object @object)
        {
            //TODO
        }

        //Returns the function that matches the given name, or null if none matches
        public SchemeFunction GetFunctionByName(string functionName)
        {
            if (InitFunction.Name == functionName) return InitFunction;

            foreach(SchemeFunction actionFunction in ActionFunctions)
            {
                if (actionFunction.Name == functionName) return actionFunction;
            }

            return null;

        }

    }
}
