using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{

    public class SpecialSlots
    {
        public static readonly string Bag = "_bag";
    }

    public class CharacterConfig
    {
        public int ActionPoints { get; set; } = 10;               //how many ap each character has at the beginning of a new round
        public int MovementActionPoints { get; set; } = 1;  //how many ap each movement costs (if not overwritten by a squaretype)

        public int StartingAbilityPoints { get; set; } = 10;  //points the player can spend on abilities at the beginning of a game

        //list of abilities the player can customize at the beginning of a game (each are 'ability' and values don't matter, they are passed in Game.SetupPlayers())
        public List<ObjectVariable> Abilities { get; set; } = new List<ObjectVariable>();

        //classvars the player can customize at the beginning of a game (only type and name are important here; values are passed in Game.SetupPlayers())
        public List<ObjectVariable> Classvars { get; set; } = new List<ObjectVariable>();

        //only types and names matter (eg. weapon hand, armor body)
        //characters will have variables that correspond to each defined slot
        public List<ObjectVariable> InventorySlots { get; set; } = new List<ObjectVariable>();

        public Scheme CommonScheme { get; set; }        //scheme for all characters
    }
}
