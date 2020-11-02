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

        // returns whether this character could step to given position (ignoring available action points)
        public bool CanStepToPosition(Position position, Game game)
        {
            if (!game.GetMap().IsPositionWithin(position)) return false;
            if (Position.GetDistance(position) != 1) return false;

            if (!game.GetMap().IsPositionPermeable(position)) return false;

            SquareType squareType = game.GetMap().GetSquareTypeAt(position);

            if (squareType != null)
            {
                if (!squareType.AllowsCharacter(this)) return false;
            }

            return true;
        }

        public bool CanReachObject(Game game, Object @object)
        {

            MapObject mapObject = game.GetMap().GetMapObject(@object);

            if (mapObject != null)
            {
                return (Position.GetDistance(mapObject.Position) <= 1);
            }

            if (Items.Contains(@object)) return true;

            if (Spells.Contains(@object)) return true;

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

        public void AddItem(Game game, Item item, int number = 1)
        {
            for (int i = 0; i < number; ++i)
            {
                Item itemCopy = item.Copy(game);
                game._AddObject(itemCopy);
                Items.Add(itemCopy);
            }
        }

        public void AddItemWithoutCopy(Item item)
        {
            Items.Add(item);
        }

        //This method chooses "randomly" which items to remove, so should only be used for items that can't have unique states
        public void RemoveItem(string itemName, int number)
        {
            //todo: remove item from slot (if inside one)
            for(int i = Items.Count - 1; (i >= 0) && (number > 0); --i)
            {
                if (Items[i].Name == itemName)
                {
                    Items.RemoveAt(i);
                    --number;
                }
            }
        }

        public void RemoveItem(Item item)
        {
            Items.Remove(item);
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
        public void EvaluateClassItemModifiers(Game game)
        {
            foreach (ObjectVariable var in Variables)
            {
                if (game.Config.IsClassType(var.Type))
                {
                    Class @class = (Class)var.Value;
                    List<ItemModifier> itemModifiers = @class.GetItemModifiers();
                    foreach(ItemModifier im in itemModifiers)
                    {
                        Item item = game.GetObjectById(im.ItemName);
                        AddItem(game, item, im.ItemNumber);
                    }
                }
            }
        }

        public void MoveItemToBag(Item item, Config config)
        {
            // we search for the slot in which the item is, and remove the item from it
            foreach(var slot in config.CharacterConfig.InventorySlots)
            {
                var slotVariable = GetVariableByName(slot.Name, config);
                if (slotVariable.Value == item)
                {
                    slotVariable.Value = null;
                    return;
                }
            }
        }

        public Item GetItemBySlot(string slotName, Config config)
        {
            var variable = GetVariableByName(slotName, config);
            return (Item)variable?.Value;
        }

        public bool MoveItemToSlot(Item item, string slotName, Config config)
        {
            var variable = GetVariableByName(slotName, config);

            if (item.Scheme == null || !SchemeExecutor.CheckTypeCompatibility(variable.Type, item.Scheme.Name, config)) return false;

            variable.Value = item;
            return true;
        }

        //returns variable without calculating actual value (compare Object.GetVariableByName)
        public ObjectVariable GetAbilityByName(string name)
        {
            foreach(var variable in Variables)
            {
                if (variable.Name == name) return variable;
            }
            return null;
        }

        public ObjectVariable AddAbility(string abilityName)
        {
            var ability = new ObjectVariable(VariableTypes.Ability, abilityName, 0);
            Variables.Add(ability);
            return ability;
        }

    }
}
