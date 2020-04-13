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

            executor.SetVariable(targetName, owns.ToString());
        }

        public string GetAsString()
        {
            return $"KNOWS ( {objectName}, {spellName} )";
        }

    }
}
