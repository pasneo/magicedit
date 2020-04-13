using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Item = magicedit.Object;
using Spell = magicedit.Object;

namespace magicedit
{
    public class Character : MapObject
    {
        public List<Item> Items = new List<Item>();
        public HashSet<Spell> Spells = new HashSet<Item>();

        public bool CanReachObject(Game game, Object @object)
        {

            MapObject mapObject = game.GetMap().GetMapObject(@object);

            if (mapObject != null)
            {
                return (Position.GetDistance(mapObject.Position) <= 1);
            }

            foreach(Item item in Items)
            {
                if (@object == item) return true;
            }

            foreach (Spell spell in Spells)
            {
                if (@object == spell) return true;
            }

            return false;
        }

        public int CountItem(string itemName)
        {
            int n = 0;
            foreach(Item item in Items)
            {
                if (item.Name == itemName) ++n;
            }
            return n;
        }

        public bool KnowsSpell(string spellName)
        {
            foreach (Spell spell in Spells)
            {
                if (spell.Name == spellName) return true;
            }
            return false;
        }

        public void AddItem(Item item)
        {
            Items.Add(item);
        }

        public void AddSpell(Spell spell)
        {
            Spells.Add(spell);
        }

    }
}
