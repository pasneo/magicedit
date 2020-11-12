using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{

    [JsonConverter(typeof(JsonSubtypes), "ModifierType")]
    public abstract class ClassModifier
    {
        public enum Type
        {
            AbilityModifier,
            AttributeModifier,
            ItemModifier
        }

        public virtual string ModifierType => "ClassModifier";

        public abstract string GetCode();

    }

    public class AbilityModifier : ClassModifier
    {
        public override string ModifierType => "AbilityModifier";

        public string AbilityName { get; set; }
        public int Value { get; set; } = 0;

        public AbilityModifier() { }
        public AbilityModifier(string abilityName, int value = 0)
        {
            this.AbilityName = abilityName;
            Value = value;
        }

        public override string GetCode()
        {
            return AbilityName + (Value < 0 ? " " : " +") + Value;
        }
    }

    public class AttributeModifier : ClassModifier
    {
        public override string ModifierType => "AttributeModifier";

        public enum AttributeModifierOptions
        {
            NONE,
            SET,
            FORBID
        }

        public string AttributeName { get; set; }
        public AttributeModifierOptions Option { get; set; } = AttributeModifierOptions.NONE;

        /* *** */

        public AttributeModifier() { }

        public AttributeModifier(string attributeName)
        {
            this.AttributeName = attributeName;
        }

        public AttributeModifier(AttributeModifierOptions option, string attributeName)
        {
            Option = option;
            this.AttributeName = attributeName;
        }

        public override string GetCode()
        {
            string code = "";
            if (Option == AttributeModifierOptions.NONE) return code;
            else if (Option == AttributeModifierOptions.SET) code = "set ";
            else if (Option == AttributeModifierOptions.FORBID) code = "forbid ";
            code += AttributeName;
            return code;
        }
    }

    public class ItemModifier : ClassModifier
    {
        public override string ModifierType => "ItemModifier";

        public string ItemName { get; set; }
        public int ItemNumber { get; set; } = 1;

        public ItemModifier() { }
        public ItemModifier(string itemName, int itemNumber = 1)
        {
            this.ItemName = itemName;
            ItemNumber = itemNumber;
        }

        public override string GetCode()
        {
            if (ItemNumber == 1)
                return "add " + ItemName;
            return $"add {ItemNumber} {ItemName}";
        }
    }

    public class SpellModifier : ClassModifier
    {
        public override string ModifierType => "SpellModifier";

        public string SpellName { get; set; }

        public SpellModifier() { }
        public SpellModifier(string spellName)
        {
            SpellName = spellName;
        }

        public override string GetCode()
        {
            return $"teach {SpellName}";
        }
    }

    public class Class
    {
        public Text ShownName { get; set; }     //This name is shown during game
        public string Name { get; set; } = "";  //This is the name used in code

        public List<ClassModifier> Modifiers { get; set; } = new List<ClassModifier>();

        /* *** */

        public Class(string name, params ClassModifier[] args)
        {
            Name = name;
            foreach(ClassModifier modif in args)
            {
                AddModifier(modif);
            }
        }

        public Class() { }

        public bool ContainsAbilityModifier(string abilityName)
        {
            foreach (ClassModifier modifier in Modifiers)
            {
                if (modifier is AbilityModifier)
                {
                    AbilityModifier abilityModifier = (AbilityModifier)modifier;
                    if (abilityModifier.AbilityName == abilityName) return true;
                }
            }
            return false;
        }

        public void RemoveAbilityModifier(string abilityName)
        {
            Modifiers.RemoveAll(modif =>
            {
                if (modif is AbilityModifier)
                {
                    AbilityModifier abilityModifier = (AbilityModifier)modif;
                    if (abilityModifier.AbilityName == abilityName) return true;
                }
                return false;
            });
        }

        public bool ContainsAttributeModifier(string attributeName)
        {
            foreach (ClassModifier modifier in Modifiers)
            {
                if (modifier is AttributeModifier)
                {
                    AttributeModifier attributeModifier = (AttributeModifier)modifier;
                    if (attributeModifier.AttributeName == attributeName) return true;
                }
            }
            return false;
        }

        public bool ContainsItemModifier(string itemName)
        {
            foreach (ClassModifier modifier in Modifiers)
            {
                if (modifier is ItemModifier)
                {
                    ItemModifier itemModifier = (ItemModifier)modifier;
                    if (itemModifier.ItemName == itemName) return true;
                }
            }
            return false;
        }

        public AbilityModifier GetAbilityModifier(string abilityName)
        {
            foreach (ClassModifier modifier in Modifiers)
            {
                if (modifier is AbilityModifier)
                {
                    AbilityModifier abilityModifier = (AbilityModifier)modifier;
                    if (abilityModifier.AbilityName == abilityName) return abilityModifier;
                }
            }
            return null;
        }

        public AttributeModifier GetAttributeModifier(string attributeName)
        {
            foreach (ClassModifier modifier in Modifiers)
            {
                if (modifier is AttributeModifier)
                {
                    AttributeModifier attributeModifier = (AttributeModifier)modifier;
                    if (attributeModifier.AttributeName == attributeName) return attributeModifier;
                }
            }
            return null;
        }

        public ItemModifier GetItemModifier(string itemName)
        {
            foreach (ClassModifier modifier in Modifiers)
            {
                if (modifier is ItemModifier)
                {
                    ItemModifier itemModifier = (ItemModifier)modifier;
                    if (itemModifier.ItemName == itemName) return itemModifier;
                }
            }
            return null;
        }

        public void UpdateAbilityModifier(AbilityModifier abilityModifier)
        {
            if (abilityModifier.Value != 0)
            {
                //if given modifier does not exist, we add it
                if (!Modifiers.Contains(abilityModifier)) Modifiers.Add(abilityModifier);
            }
            else
            {
                //if given modifier exists in our list, and it's option is NONE, we delete it
                if (Modifiers.Contains(abilityModifier)) Modifiers.Remove(abilityModifier);
            }
        }

        public void UpdateAttributeModifier(AttributeModifier attributeModifier)
        {
            if (attributeModifier.Option != AttributeModifier.AttributeModifierOptions.NONE)
            {
                //if given modifier does not exist, we add it
                if (!Modifiers.Contains(attributeModifier)) Modifiers.Add(attributeModifier);
            }
            else
            {
                //if given modifier exists in our list, and it's option is NONE, we delete it
                if (Modifiers.Contains(attributeModifier)) Modifiers.Remove(attributeModifier);
            }
        }

        public void AddModifier(ClassModifier modifier)
        {
            Modifiers.Add(modifier);
        }

        public List<ItemModifier> GetItemModifiers()
        {
            List<ItemModifier> itemModifiers = new List<ItemModifier>();
            foreach (ClassModifier modifier in Modifiers)
            {
                if (modifier is ItemModifier) itemModifiers.Add((ItemModifier)modifier);
            }
            return itemModifiers;
        }

        public List<SpellModifier> GetSpellModifiers()
        {
            List<SpellModifier> spellModifiers = new List<SpellModifier>();
            foreach (ClassModifier modifier in Modifiers)
            {
                if (modifier is SpellModifier) spellModifiers.Add((SpellModifier)modifier);
            }
            return spellModifiers;
        }

    }

    public class ClassList
    {
        public Text ShownName { get; set; }         //This name is shown during game
        public string Name { get; set; } = "";      //This name is used in code

        public List<Class> Classes { get; set; } = new List<Class>();

        /* *** */

        public ClassList() { }
        public ClassList(string name)
        {
            Name = name;
        }

        public bool IsClassExisting(string className)
        {
            foreach (Class c in Classes)
            {
                if (c.Name == className) return true;
            }
            return false;
        }

        public Class GetClassByName(string className)
        {
            foreach (Class c in Classes)
            {
                if (c.Name == className) return c;
            }
            return null;
        }

        public void AddClass(Class c) { Classes.Add(c); }
        
    }

}
