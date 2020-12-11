using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandAddItem : ISchemeCommand
    {

        string characterName, numberName, itemName;

        public CommandAddItem(string characterName, string numberName, string itemName)
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

            if (!(objectVariable.Value is Character)) throw SchemeExecutor.CreateException("Items can only be added to characters");

            Character character = (Character)(objectVariable.Value);

            ObjectVariable itemVariable = executor.GetVariableByName(itemName);

            if (!executor.CheckTypeCompatibility(VariableTypes.Object, itemVariable.Type))
                throw SchemeExecutor.CreateException($"Type '{itemVariable.Type}' is not an object type");

            character.AddItem(executor.Game, (Object)itemVariable.Value, requiredNumber);
        }

        public string GetAsString()
        {
            return $"ADD ITEM ( {characterName}, {numberName}, {itemName} )";
        }

        public void ChangeInputs(string current_val, string new_val)
        {
            if (numberName == current_val) numberName = new_val;
        }

        public void ChangeOutput(string current_val, string new_val)
        {
        }

        public bool HasOutput(string output_name)
        {
            return false;
        }

        public bool HasInput(string input_name)
        {
            return numberName == input_name;
        }

        public List<string> GetInputs()
        {
            return new List<string> { numberName };
        }

        public List<string> GetOutputs()
        {
            return null;
        }
    }
}
