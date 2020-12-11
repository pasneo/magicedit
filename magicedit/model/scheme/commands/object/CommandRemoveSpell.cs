using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandRemoveSpell : ISchemeCommand
    {

        string characterName, spellName;

        public CommandRemoveSpell(string characterName, string spellName)
        {
            this.characterName = characterName;
            this.spellName = spellName;
        }

        public void Execute(SchemeExecutor executor)
        {
            ObjectVariable objectVariable = executor.GetVariableByName(characterName);

            if (objectVariable == null) throw SchemeExecutor.CreateException($"Character '{characterName}' not found");

            if (!(objectVariable.Value is Character)) throw SchemeExecutor.CreateException("Spells can only be added to characters");

            Character character = (Character)(objectVariable.Value);

            ObjectVariable spellVariable = executor.GetVariableByName(spellName);

            if (!executor.CheckTypeCompatibility(VariableTypes.Object, spellVariable.Type))
                throw SchemeExecutor.CreateException($"Type '{spellVariable.Type}' is not an object type");

            character.RemoveSpell((Object)spellVariable.Value);
        }

        public string GetAsString()
        {
            return $"REMOVE SPELL ( {characterName}, {spellName} )";
        }

        public void ChangeInputs(string current_val, string new_val)
        {
        }

        public bool HasOutput(string output_name)
        {
            return false;
        }

        public void ChangeOutput(string current_val, string new_val)
        {
        }

        public bool HasInput(string input_name)
        {
            return false;
        }

        public List<string> GetInputs()
        {
            return null;
        }

        public List<string> GetOutputs()
        {
            return null;
        }

    }
}
