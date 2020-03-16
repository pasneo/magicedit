using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class Game
    {
        private Config Config;
        private List<Player> Players;
        private List<Object> Objects;

        public int Round { get; private set; }

        /* *** */

        public void Start()
        {
            Round = 0;
            //TODO
        }

        private void NextRound()
        {
            foreach(Player player in Players)
            {
                player.Act(this);
            }
        }

        public Map GetMap()
        {
            return Config.Map;
        }

    }
}
