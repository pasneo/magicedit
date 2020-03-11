using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class SquareType
    {

        public string Name { get; set; }
        public Visual Visual { get; set; }

        //Code written in scheme lang. Executes when a character steps on the squre
        private string StepFunctionCode;

        //Only characters that bear all AllowedAttributes can step on square
        private List<ObjectAttribute> AllowedAttributes;
        //Only characters that do not bear any ForbiddenAttributes can step on square
        private List<ObjectAttribute> ForbiddenAttributes;

    }
}
