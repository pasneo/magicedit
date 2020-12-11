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

        //The action in the scheme 'map' that executes when a character steps on the squre (actor = character, object = <null>)
        public string ActionName;

        //Only characters that bear all AllowedAttributes can step on square
        public List<string> AllowedAttributes = new List<string>();
        //Only characters that do not bear any ForbiddenAttributes can step on square
        public List<string> ForbiddenAttributes = new List<string>();

        /* *** */

        public SquareType() { }

        public SquareType(string name, Visual visual = null) { Name = name; Visual = visual; }

        // returns true if given character is allowed to step on this type of square
        public bool AllowsCharacter(Character character)
        {
            foreach(string attr in AllowedAttributes)
            {
                if (attr == null || attr == "") continue;
                if (!character.HasAttribute(attr)) return false;
            }
            foreach(string attr in ForbiddenAttributes)
            {
                if (attr == null || attr == "") continue;
                if (character.HasAttribute(attr)) return false;
            }
            return true;
        }

    }
}
