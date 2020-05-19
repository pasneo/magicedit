using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandKnows : ISchemeCommand
    {

        string objectName, spellName, targetName;

        public CommandKnows(string objectName, string spellName, string targetName)
        {
            this.objectName = objectName;
            this.spellName = spellName;
            this.targetName = targetName;
        }

        public void Execute(SchemeExecutor executor)
        {
            ObjectVariable @object = executor.GetVariableByName(objectName);

            if (@object == null) throw SchemeExecutor.CreateException($"Object '{objectName}' not found");
            
            if (!(@object.Value is Character)) throw new SchemeExecutionException($"Object '{objectName}' is not a character");

            Character character = (Character)(@object.Value);
            
            bool owns = character.KnowsSpell(spellName);

            executor.SetVariable(targetName, new ObjectVariable(VariableTypes.Logical, "", owns));
        }

        public string GetAsString()
        {
            return $"KNOWS ( {objectName}, {spellName} )";
        }

        public void ChangeInputs(string current_val, string new_val)
        {
            if (objectName == current_val) objectName = new_val;
        }

        public void ChangeOutput(string current_val, string new_val)
        {
            if (targetName == current_val) targetName = new_val;
        }

        public bool HasOutput(string output_name)
        {
            return targetName == output_name;
        }

        public bool HasInput(string input_name)
        {
            return objectName == input_name;
        }

        public List<string> GetInputs()
        {
            return new List<string> { objectName };
        }

        public List<string> GetOutputs()
        {
            return new List<string> { targetName };
        }

    }
}
