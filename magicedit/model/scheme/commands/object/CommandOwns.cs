﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandOwns : ISchemeCommand
    {

        string objectName, numberName, itemName, targetName;

        public CommandOwns(string objectName, string numberName, string itemName, string targetName)
        {
            this.objectName = objectName;
            this.numberName = numberName;
            this.itemName = itemName;
            this.targetName = targetName;
        }

        public void Execute(SchemeExecutor executor)
        {
            ObjectVariable @object = executor.GetVariableByName(objectName);
            ObjectVariable numberVariable = executor.FindValueByString(numberName);

            if (@object == null) throw SchemeExecutor.CreateException($"Object '{objectName}' not found");
            if (numberVariable.Type != VariableTypes.Number) throw SchemeExecutor.CreateException($"Number is expected to be of type number");

            int requiredNumber = (int)numberVariable.Value;

            if (!(@object.Value is Character)) throw new SchemeExecutionException($"Object '{objectName}' is not a character");

            Character character = (Character)(@object.Value);

            int ownedNumber = character.CountItem(itemName);

            bool owns = ownedNumber >= requiredNumber;

            executor.SetVariable(targetName, new ObjectVariable(VariableTypes.Logical, "", owns));
        }

        public string GetAsString()
        {
            return $"HAS ( {objectName}, {numberName}, {itemName}, {targetName} )";
        }

        public void ChangeInputs(string current_val, string new_val)
        {
            if (numberName == current_val) numberName = new_val;
            if (objectName == current_val) objectName = new_val;
        }

        public bool HasOutput(string output_name)
        {
            return targetName == output_name;
        }

        public void ChangeOutput(string current_val, string new_val)
        {
            if (targetName == current_val) targetName = new_val;
        }

        public bool HasInput(string input_name)
        {
            return numberName == input_name || objectName == input_name;
        }

        public List<string> GetInputs()
        {
            return new List<string> { objectName, numberName };
        }

        public List<string> GetOutputs()
        {
            return new List<string> { targetName };
        }

    }
}
