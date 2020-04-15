﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandRemoveItem : ISchemeCommand
    {
        string characterName, numberName, itemName;

        public CommandRemoveItem(string characterName, string numberName, string itemName)
        {
            this.characterName = characterName;
            this.numberName = numberName;
            this.itemName = itemName;
        }

        public void Execute(SchemeExecutor executor)
        {
            ObjectVariable objectVariable = executor.GetVariableByName(characterName);
            ObjectVariable numberVariable = executor.FindValueByString(numberName);

            if (objectVariable == null) throw SchemeExecutor.CreateException($"Character '{characterName}' not found");
            if (numberVariable.Type != VariableTypes.Number) throw SchemeExecutor.CreateException($"Number is expected to be of type number");

            int requiredNumber = (int)numberVariable.Value;

            if (!(objectVariable.Value is Character)) throw SchemeExecutor.CreateException("Items can only be removed from characters");

            Character character = (Character)(objectVariable.Value);

            ObjectVariable itemVariable = executor.GetVariableByName(itemName);
            //TODO: Maybe check if the object identified by itemName is really an item (now we check only if it's an object)
            if (executor.CheckTypeCompatibility(VariableTypes.Object, itemVariable.Type))
                throw SchemeExecutor.CreateException($"Type '{itemVariable.Type}' is not an object type");
            
            character.RemoveItem(itemName, requiredNumber);
        }

        public string GetAsString()
        {
            return $"REMOVE ITEM ( {characterName}, {numberName}, {itemName} )";
        }
    }
}
