﻿using System;
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

        public void AddItem(Item item, int number = 1)
        {
            for(int i = 0; i < number; ++i)
                Items.Add(item.Copy());
        }

        //This method chooses "randomly" which items to remove, so should only be used for items that can't have unique states
        public void RemoveItem(string itemName, int number)
        {
            for(int i = Items.Count - 1; (i >= 0) && (number > 0); --i)
            {
                if (Items[i].Name == itemName)
                {
                    Items.RemoveAt(i);
                    --number;
                }
            }
        }

        public void AddSpell(Spell spell)
        {
            Spells.Add(spell);
        }

        public void RemoveSpell(Spell spell)
        {
            Spells.Remove(spell);
        }

    }
}
