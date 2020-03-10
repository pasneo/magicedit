using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class Scheme
    {

        public string Code { get; private set; }
        private List<Scheme> Parents;
        public CompiledScheme CompiledScheme;

        public CompiledScheme Compile()
        {
            //TODO
            return null;
        }

    }
}
