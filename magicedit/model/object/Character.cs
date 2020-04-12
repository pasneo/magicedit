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
        public List<Spell> Spells = new List<Item>();

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

        public void AddItem(Item item)
        {
            Items.Add(item);
        }

    }
}
