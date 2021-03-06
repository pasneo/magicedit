﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandOf : ISchemeCommand
    {

        string propertyName, objectName, target;

        public CommandOf(string propertyName, string objectName, string target)
        {
            this.propertyName = propertyName;
            this.objectName = objectName;
            this.target = target;
        }

        public void Execute(SchemeExecutor executor)
        {
            ObjectVariable property = executor.GetPropertyOf(propertyName, objectName);
            executor.SetVariable(target, property);
        }

        public string GetAsString()
        {
            return "OF ( " + propertyName + ", " + objectName + ", " + target + " )";
        }

        public void ChangeInputs(string current_val, string new_val)
        {
            if (objectName == current_val) objectName = new_val;
        }

        public void ChangeOutput(string current_val, string new_val)
        {
            if (target == current_val) target = new_val;
        }

        public bool HasInput(string input_name)
        {
            return objectName == input_name;
        }

        public bool HasOutput(string output_name)
        {
            return target == output_name;
        }

        public List<string> GetInputs()
        {
            return new List<string> { objectName };
        }

        public List<string> GetOutputs()
        {
            return new List<string> { target };
        }

    }
}
