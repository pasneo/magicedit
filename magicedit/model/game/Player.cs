using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class Player
    {

        public Character Character { get; private set; } = new Character();
        public int AvailableActionPoints { get; set; }

        /* *** */

        public Player() { }

        public Player(Character character)
        {
            Character = character;
        }

    }
}
