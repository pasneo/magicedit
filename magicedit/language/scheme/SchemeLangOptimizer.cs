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

        //Returns starting position of each basic block in the given function's code
        private static HashSet<int> GetBlockPositions(SchemeFunction function)
        {
            HashSet<int> positions = new HashSet<int>();

            var commands = function.Commands;

            for (int i = 0; i < commands.Count; ++i)
            {
                ISchemeCommand cmd = commands[i];

                if (cmd is CommandJumpBase)
                {
                    int line = ((CommandJumpBase)cmd).Line;
                    positions.Add(line);
                    positions.Add(i + 1);
                }

            }

            return positions;

        }

        public static void SolveSimpleValueRegs(SchemeFunction function)
        {

            //We iterate commands
            //If we find a SET(<reg>, <simple value>) command, we
            //    //remove this SET command
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

        public static void SolveRegisterBuffers(SchemeFunction function)
        {
            // if an operation stores its result in a register <reg>, and it is followed by a SET(<var>, <reg>) command
            //      we delete the SET command and set the opperation's output to <var>
            //      we iterate over the following commands and change any <reg> to <var>
            //      we stop if <reg> is set again or we leave the block

            var commands = function.Commands;
            HashSet<int> removableIndexes = new HashSet<int>();

            for (int i = 0; i < commands.Count; ++i)
            {
                ISchemeCommand cmd = commands[i];

                var outputs = cmd.GetOutputs();

                if (outputs == null) continue;

                // check if any output is a register
                foreach(var reg in outputs)
                {
                    if (SchemeExecutor.IsRegister(reg))
                    {
                        string variable = null;
                        for(int j = i + 1; j < commands.Count; ++j)
                        {
                            ISchemeCommand cmd2 = commands[j];

                            if (cmd2 is CommandJumpBase) break; // break if we leave the block
                            if (cmd2.HasOutput(reg)) break;  // break if the register is set again

                            if (cmd2 is CommandSetVariable && cmd2.HasInput(reg))
                            {
                                CommandSetVariable setCmd = (CommandSetVariable)cmd2;
                                variable = setCmd.GetVariableName();
                                if (SchemeExecutor.IsRegister(variable))
                                {
                                    variable = null;
                                }
                                else
                                {
                                    cmd.ChangeOutput(reg, variable);
                                    removableIndexes.Add(j);
                                }
                            }

                            if (variable != null && cmd2.HasInput(reg))
                            {
                                cmd2.ChangeInputs(reg, variable);
                            }
                        }
                    }
                }
            }

            RemoveCommandsAt(function, removableIndexes);
        }

        public static void SolveDeadCode(SchemeFunction function)
        {

            //For each command C:
            //. If C uses local variable X that is in V, as input, we remove X from V and the corresponding line from L
            //. If C has local variable output X that is in V, then we remove the line of code that is connected to X (found in L)
            //. If C has local variable output, we store the output variable's name in array V, and the code line in L
            //. If we reach new basic block, we clear V and L

            bool changed = true;
            while (changed)
            {
                changed = false;

                HashSet<int> blocks = GetBlockPositions(function);

                var commands = function.Commands;
                HashSet<int> removableIndexes = new HashSet<int>();
                
                Dictionary<string, int> lastSetLines = new Dictionary<string, int>();

                for (int i = 0; i < commands.Count; ++i)
                {

                    if (blocks.Contains(i)) lastSetLines.Clear();

                    ISchemeCommand cmd = commands[i];

                    if (cmd is CommandCreateVariable) continue; //We don't want to remove CREATE commands

                    List<string> inputs = cmd.GetInputs();
                    List<string> outputs = cmd.GetOutputs();

                    if (inputs != null)
                    {
                        foreach(string input in inputs)
                        {
                            if (SchemeExecutor.IsVariable(input))
                                lastSetLines.Remove(input);
                        }
                    }

                    if (outputs != null)
                    {
                        foreach (string output in outputs)
                        {
                            if (SchemeExecutor.IsVariable(output))
                            {
                                if (lastSetLines.ContainsKey(output))
                                {
                                    removableIndexes.Add(lastSetLines[output]);
                                }
                                lastSetLines[output] = i;
                            }
                        }
                    }

                }

                if (removableIndexes.Count > 0)
                {
                    changed = true;
                    RemoveCommandsAt(function, removableIndexes);
                }

            }

        }

        public static void Optimize(SchemeFunction function)
        {
            if (function == null) return;

            SolveDoubleJumps(function);
            SolveSimpleValueRegs(function);
            SolveRegisterBuffers(function);
            SolveDeadCode(function);
        }

        public static void Optimize(Scheme scheme)
        {
            //if (!scheme.IsCompiledValid) return;

            Optimize(scheme.CompiledScheme.InitFunction);
            Optimize(scheme.CompiledScheme.BodyFunction);

            foreach(SchemeFunction func in scheme.CompiledScheme.ActionFunctions)
            {
                Optimize(func);
            }

        }

    }
}
