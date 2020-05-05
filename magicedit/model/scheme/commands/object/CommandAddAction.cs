﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class CommandAddAction : ISchemeCommand
    {

        string actionName;

        public CommandAddAction(string actionName)
        {
            this.actionName = actionName;
        }

        public void Execute(SchemeExecutor executor)
        {
            executor.Object.AddAction(actionName);
        }

        public string GetAsString()
        {
            return $"ADD ACTION ( {actionName} )";
        }

    }
}