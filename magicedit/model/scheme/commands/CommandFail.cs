﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandFail : ISchemeCommand
    {
        public void Execute(SchemeExecutor executor)
        {
            executor.Fail();
        }

        public string GetAsString()
        {
            return $"FAIL ( )";
        }

        public void ChangeInputs(string current_val, string new_val)
        {
        }

        public void ChangeOutput(string current_val, string new_val)
        {
        }

        public bool HasInput(string input_name)
        {
            return false;
        }

        public bool HasOutput(string output_name)
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
