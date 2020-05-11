using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class SchemeLangOptimizer
    {

        public static void SolveDoubleJumps(SchemeFunction function)
        {
            bool changed = true;

            var commands = function.Commands;

            while (changed)
            {
                changed = false;

                for (int i = 0; i < commands.Count; ++i)
                {
                    ISchemeCommand cmd = commands[i];
                    if (cmd is CommandJumpBase)
                    {
                        CommandJumpBase jumpCmd = (cmd as CommandJumpBase);
                        int line = jumpCmd.Line;
                        if (line < commands.Count && commands[line] is CommandJump)
                        {
                            CommandJump jumpCmd2 = commands[line] as CommandJump;
                            if (jumpCmd.Line != jumpCmd2.Line)
                            {
                                jumpCmd.SetLine(jumpCmd2.Line);
                                changed = true;
                            }
                        }
                    }
                }

            }
        }

        public static void SolveAdjacentJumps(SchemeFunction function)
        {

            //TODO

        }

        private static void DecreaseJumpLocations(List<ISchemeCommand> commands, int loc)
        {
            foreach(var cmd in commands)
            {
                if (cmd is CommandJumpBase)
                {
                    CommandJumpBase cmdJumpBase = (CommandJumpBase)cmd;
                    if (cmdJumpBase.Line > loc) cmdJumpBase.SetLine(cmdJumpBase.Line - 1);
                }
            }
        }

        private static void RemoveCommandsAt(SchemeFunction function, HashSet<int> indexes)
        {
            var commands = function.Commands;
            for (int i = commands.Count - 1; i >= 0; --i)
            {
                if (indexes.Contains(i))
                {
                    commands.RemoveAt(i);
                    DecreaseJumpLocations(commands, i);
                }
            }
        }

        public static void SolveSimpleValueRegs(SchemeFunction function)
        {

            //We iterate commands
            //If we find a SET(<reg>, <simple value>) command, we
            //    //TODO: remove this SET command
            //    //Iterate over the subsequent commands in the base block
            //    //If we find any command with input <reg> we change it to <simple value>
            //    //If we find any command with output <reg> we stop and goto 1. (continuing from the SET command we found)
            
            var commands = function.Commands;
            HashSet<int> removableIndexes = new HashSet<int>();

            for(int i=0; i<commands.Count; ++i)
            {
                ISchemeCommand cmd = commands[i];

                if (cmd is CommandSetVariable)
                {
                    CommandSetVariable cmdSet = (CommandSetVariable)cmd;
                    string targetVar = cmdSet.GetVariableName();
                    string value = cmdSet.GetValue();

                    if (SchemeExecutor.IsRegister(targetVar))
                    {
                        removableIndexes.Add(i);
                        for(int i2 = i + 1; i2 < commands.Count; ++i2)
                        {
                            ISchemeCommand cmd2 = commands[i2];
                            cmd2.ChangeInputs(targetVar, value);
                            if (cmd2 is CommandJumpBase || cmd2.HasOutput(targetVar)) break;
                        }
                    }
                }
            }

            RemoveCommandsAt(function, removableIndexes);

        }

        public static void Optimize(SchemeFunction function)
        {
            SolveDoubleJumps(function);
            SolveSimpleValueRegs(function);
        }

    }
}
