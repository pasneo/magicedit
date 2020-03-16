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

        private Object GetMatchingReachableObject(string id, Game game)
        {
            //Search reachable mapobjects, inventory items and known spells for matching id
            foreach (var mapObject in GetReachableMapObjects(game.GetMap()))
            {
                if (mapObject.Id == id) return mapObject;
            }

            foreach(var item in Character.Items)
            {
                if (item.Id == id) return item;
            }

            foreach (var spell in Character.Spells)
            {
                if (spell.Id == id) return spell;
            }

            return null;
        }

        public void Act(Game game)
        {
            //TODO: ask player for a list of (action,object) pairs ("I want to do [some action] with [some object]")

            //PROTO
            // 1. get number of actions
            // 2. get next action's action (by name)
            // 3. get next action's object (by id)
            // 4. execute action
            // 5. if there are actions left, goto 2.

            int numberOfACtions = Convert.ToInt32(Console.ReadLine());

            for(int i = 0; i < numberOfACtions; ++i)
            {
                string actionName = Console.ReadLine();

                if (actionName == "move")
                {
                    int x = Convert.ToInt32(Console.ReadLine());
                    int y = Convert.ToInt32(Console.ReadLine());

                    Character.Position.X = x;
                    Character.Position.Y = y;
                }
                else
                {
                    string objectId = Console.ReadLine();

                    Object @object = GetMatchingReachableObject(objectId, game);

                    if (@object != null)
                    {
                        @object.ExecuteAction(actionName, Character);
                    }
                }

            }

        }

    }
}
