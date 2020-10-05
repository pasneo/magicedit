using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CharacterConfig
    {
        public int ActionPoints { get; set; }           //how many ap each character has at the beginning of a new round
        public int MovementActionPoints { get; set; }   //how many ap each movement costs

        public int StartingAbilityPoints { get; set; }  //points the player can spend on abilities at the beginning of a game

        //list of abilities the player can customize at the beginning of a game (each are 'number' and values don't matter, they are passed in Game.SetupPlayers())
        public List<ObjectVariable> Abilities { get; set; } = new List<ObjectVariable>();

        //classvars the player can customize at the beginning of a game (only type and name are important here; values are passed in Game.SetupPlayers())
        public List<ObjectVariable> Classvars { get; set; } = new List<ObjectVariable>();

        public Scheme CommonScheme { get; set; }        //scheme for all characters
    }
}
