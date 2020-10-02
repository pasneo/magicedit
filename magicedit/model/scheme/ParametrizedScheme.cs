using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class ParametrizedScheme
    {
        
        private Scheme _scheme;
        public Scheme Scheme
        {
            get { return BuildScheme(); }
        }

        public string Name => Scheme.Name;



        /* *** */

        public Scheme BuildScheme()
        {
            throw new NotImplementedException();
        }

    }
}
