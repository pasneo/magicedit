using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class Player
    {

        public Character Character { get; private set; }

        /* *** */

        private List<Object> GetReachableMapObjects(Map map)
        {
            //TODO: collect objects on the map the player can interact with
            return null;
        }

        public void Act(Game game)
        {
            //TODO: ask player for a list of (object,action) pairs ("I want to do [some action] with [some object]")

            //PROTO
            // 1. get number of actions
            // 2. get next action's object
            // 3. get next action's action
            // 4. execute action
            // 5. if there are actions left, goto 2.

            int numberOfACtions = Convert.ToInt32(Console.ReadLine());

            for(int i = 0; i < numberOfACtions; ++i)
            {
                string objectName = Console.ReadLine();
                string action = Console.ReadLine();
            }

        }

    }
}
