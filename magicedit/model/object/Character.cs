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

        public override string ObjectType => "Character";

        public List<Item> Items = new List<Item>();
        public HashSet<Spell> Spells = new HashSet<Item>();

        /* *** */

        public Character() { }

        public Character(string id, string name) : base(id, name) { }

        //this will create variables that represent inv slots in character
        public void CreateInventorySlots(Config config)
        {
            foreach(var slot in config.CharacterConfig.InventorySlots)
            {
                if (GetVariableByName(slot.Name, config) == null)
                {
                    Variables.Add(new ObjectVariable(slot.Type, slot.Name, null));
                }
            }
        }

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

        // used to evaluate classes that provide items for characters
        public void EvaluateClassItemModifiers(Config config)
        {
            foreach (ObjectVariable var in Variables)
            {
                if (config.IsClassType(var.Type))
                {
                    Class @class = (Class)var.Value;
                    List<ItemModifier> itemModifiers = @class.GetItemModifiers();
                    foreach(ItemModifier im in itemModifiers)
                    {
                        Item item = config.GetItemById(im.ItemName);
                        AddItem(item, im.ItemNumber);
                    }
                }
            }
        }

        public Item GetItemBySlot(string slotName, Config config)
        {
            var variable = GetVariableByName(slotName, config);
            return (Item)variable.Value;
        }

        public bool MoveItemToSlot(Item item, string slotName, Config config)
        {
            var variable = GetVariableByName(slotName, config);

            if (item.Scheme == null || SchemeExecutor.CheckTypeCompatibility(variable.Type, item.Scheme.Name, config)) return false;

            variable.Value = item;
            return true;
        }

    }
}
