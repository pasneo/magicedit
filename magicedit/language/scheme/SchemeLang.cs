using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using magicedit.language.scheme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using magicedit.language;

namespace magicedit
{
    public class SchemeLang
    {

        public class MissingActionErrorDescriptor : ErrorDescriptor {

            string actionName;

            public MissingActionErrorDescriptor(string actionName, string message, ParserRuleContext context) : base(message, context)
            {
                this.actionName = actionName;
            }

            public bool IsError(Scheme scheme)
            {
                return scheme.CompiledScheme.GetFunctionByName(actionName) == null;
            }

        }

        public class CommandBlock
        {
            public List<ObjectVariable> LocalVariables = new List<ObjectVariable>();
        }

        // this class helps to manage command blocks and check for local variables and their validity
        public class VariableManager
        {
            public List<CommandBlock> Blocks = new List<CommandBlock>();

            public void NewBlock() { Blocks.Add(new CommandBlock()); }
            public void EndBlock() { Blocks.Remove(Blocks.Last()); }

            // checks whether given value can really be used as a value
            public bool IsValueValid(string variableName, Scheme scheme, Config config)
            {
                // check keywords
                if (variableName == "actor" || variableName == "me") return true;

                // check local variables
                foreach(var block in Blocks)
                {
                    if (block.LocalVariables.Where(v => v.Name == variableName).Count() > 0) return true;
                }

                // check global variables & parameters
                if (scheme.CompiledScheme.GetVariableByName(variableName) != null) return true;
                if (scheme.CompiledScheme.GetParameterByName(variableName) != null) return true;

                // check inherited global variables & parameters
                foreach(var parent in scheme.Parents)
                {
                    if (parent.CompiledScheme?.GetVariableByName(variableName) != null) return true;
                    if (parent.CompiledScheme?.GetParameterByName(variableName) != null) return true;
                }

                // check global objects
                if (config.GetObjectById(variableName) != null) return true;

                // check string consts
                if (config.GetStringConstByName(variableName) != null) return true;

                return false;
            }

            public bool IsVariable(string variableName, Scheme scheme)
            {
                // check local variables
                foreach (var block in Blocks)
                {
                    if (block.LocalVariables.Where(v => v.Name == variableName).Count() > 0) return true;
                }

                // check global variables & parameters
                if (scheme.CompiledScheme.GetVariableByName(variableName) != null) return true;

                // check inherited global variables & parameters
                foreach (var parent in scheme.Parents)
                {
                    if (parent.CompiledScheme?.GetVariableByName(variableName) != null) return true;
                }

                return false;
            }

            public void CheckValueValidity(string variableName, Visitor visitor, ParserRuleContext context)
            {
                if (!IsValueValid(variableName, visitor.Scheme, visitor.Config))
                {
                    visitor.Errors.Add(new ErrorDescriptor($"Variable '{variableName}' is not recognized.", context));
                }
            }

            public void CheckNewVariable(string variableName, Visitor visitor, ParserRuleContext context)
            {
                if (IsValueValid(variableName, visitor.Scheme, visitor.Config))
                {
                    visitor.Errors.Add(new ErrorDescriptor($"Variable '{variableName}' already exists.", context));
                }
                else if (Blocks.Count > 0)
                {
                    Blocks.Last().LocalVariables.Add(new ObjectVariable(VariableTypes.Unknown, variableName, null));
                }
            }

            public bool IsTypeValid(string type, Config config)
            {
                if (VariableTypes.Contains(type)) return true;
                if (config.GetSchemeByName(type) != null) return true;
                return false;
            }

        }

        public class SchemeLangException : Exception
        {
            public SchemeLangException(string message) : base(message) { }
        }

        public class Visitor : scheme_langBaseVisitor<object>
        {

            public Scheme Scheme { get; set; }
            public Config Config { get; set; }
            public CompiledScheme CompiledScheme { get; set; } = new CompiledScheme();

            private SchemeFunction bodyFunction;
            private SchemeFunction currentFunc;

            private VariableManager VariableManager = new VariableManager();

            public List<ErrorDescriptor> Errors { get; set; } = new List<ErrorDescriptor>();
            public List<MissingActionErrorDescriptor> MissingActionErrors { get; set; } = new List<MissingActionErrorDescriptor>();

            /* *** */

            public Visitor(Scheme scheme, Config config)
            {
                Scheme = scheme;
                Config = config;
                Scheme.CompiledScheme = CompiledScheme;

                bodyFunction = new SchemeFunction("_body");
                CompiledScheme.SetBody(bodyFunction);

                currentFunc = bodyFunction;
            }

            /* FUNCTIONS FOR VALIDATION */

            private bool IsValidVariable(string name)
            {

                return false;
            }

            /* CODE BUILDING */

            private ObjectVariable CreateVariableFromContext(scheme_langParser.Variable_definitionContext context)
            {
                string type = context.variable_type().GetText();
                string name = context.variable_name().GetText();
                object value = null;

                if (CompiledScheme.GetVariableByName(name) != null || CompiledScheme.GetParameterByName(name) != null)
                {
                    Errors.Add(new ErrorDescriptor($"Variable or parameter '{name}' already exists.", context));
                    return null;
                }

                return new ObjectVariable(type, name, value);
            }

            /* Scheme building */

            public override object VisitScheme_name([NotNull] scheme_langParser.Scheme_nameContext context)
            {

                string scheme_name = context?.GetText();

                Scheme.Name = scheme_name;
                Scheme.Parents.Clear();

                if (scheme_name != null)
                {
                    int scheme_count = Config.GetSchemeCountByName(scheme_name);
                    if (scheme_count > 1)
                    {
                        Errors.Add(new ErrorDescriptor($"Scheme '{scheme_name}' is already defined elsewhere.", context));
                    }
                }

                return base.VisitScheme_name(context);
            }

            public override object VisitParent_name([NotNull] scheme_langParser.Parent_nameContext context)
            {
                //Add parent to scheme
                string parentName = context.GetText();
                Scheme parent = Config.GetSchemeByName(parentName);

                if (parent == null)
                    Errors.Add(new ErrorDescriptor($"Scheme '{parentName}' does not exist.", context));
                else
                    Scheme.AddParent(parent);

                return base.VisitParent_name(context);
            }

            public override object VisitBody_variable_definition([NotNull] scheme_langParser.Body_variable_definitionContext context)
            {
                //Add predefined variable to scheme
                var variable_definition = context.variable_definition();

                string type = variable_definition?.variable_type()?.GetText();
                string name = variable_definition?.variable_name()?.GetText();

                if (type == null || name == null) return base.VisitBody_variable_definition(context);

                if (!VariableManager.IsTypeValid(type, Config))
                {
                    Errors.Add(new ErrorDescriptor($"Type '{type}' is not recognized.", variable_definition.variable_type()));
                }

                if (CompiledScheme.GetVariableByName(name) != null || CompiledScheme.GetParameterByName(name) != null)
                {
                    Errors.Add(new ErrorDescriptor($"Variable or parameter '{name}' already exists.", variable_definition.variable_name()));
                    if (variable_definition.EQUALS() != null) SetNewExpression(1);
                    return base.VisitBody_variable_definition(context);
                }
                
                ObjectVariable variable = new ObjectVariable(type, name, null);

                if (variable != null)
                {
                    CompiledScheme.AddVariable(variable);

                    CommandSetVariable cmd = new CommandSetVariable(variable_definition.variable_name().GetText(), GetRegName(0));
                    currentFunc.AddCommand(cmd);
                    SetNewExpression(1);
                }

                return base.VisitBody_variable_definition(context);
            }

            public override object VisitParameter_definition([NotNull] scheme_langParser.Parameter_definitionContext context)
            {
                //Add parameter to scheme
                string type = context.variable_type()?.GetText();
                string name = context.variable_name()?.GetText();

                if (type == null || name == null) return base.VisitParameter_definition(context);

                if (CompiledScheme.GetVariableByName(name) != null || CompiledScheme.GetParameterByName(name) != null)
                {
                    Errors.Add(new ErrorDescriptor($"Variable or parameter '{name}' already exists.", context.variable_name()));
                }
                else if (!VariableManager.IsTypeValid(type, Config))
                {
                    Errors.Add(new ErrorDescriptor($"Type '{type}' is not recognized.", context.variable_type()));
                }
                else
                {
                    ObjectVariable param = new ObjectVariable(type, name, null);
                    CompiledScheme.AddParameter(param);
                }

                return base.VisitParameter_definition(context);
            }

            public override object VisitInit_block([NotNull] scheme_langParser.Init_blockContext context)
            {

                if (CompiledScheme.GetFunctionByName("init") != null)
                {
                    Errors.Add(new ErrorDescriptor($"This scheme already contains an init block.", context));
                }

                //Sign start of init block
                currentFunc = new SchemeFunction("init");
                VariableManager.NewBlock();

                return base.VisitInit_block(context);
            }

            public override object VisitEnd_of_init([NotNull] scheme_langParser.End_of_initContext context)
            {
                //Add completed init function to scheme
                CompiledScheme.SetInit(currentFunc);
                currentFunc = null;
                VariableManager.EndBlock();
                return base.VisitEnd_of_init(context);
            }

            public override object VisitAction_block([NotNull] scheme_langParser.Action_blockContext context)
            {
                //Sign start of action
                string name = context.action_name()?.GetText();
                int actionPoints = 0;

                if (context.action_point() != null)
                {
                    int.TryParse(context.action_point().GetText(), out actionPoints);
                }
                //else
                    //Errors.Add(new ErrorDescriptor("Missing action points"));
                    
                if (CompiledScheme.GetFunctionByName(name) != null)
                {
                    Errors.Add(new ErrorDescriptor($"This scheme already contains an action named '{name}'.", context.action_name()));
                }

                currentFunc = new SchemeFunction(name, actionPoints);
                VariableManager.NewBlock();

                return base.VisitAction_block(context);
            }

            public override object VisitEnd_of_action([NotNull] scheme_langParser.End_of_actionContext context)
            {
                //Add completed action to scheme
                CompiledScheme.AddAction(currentFunc);
                currentFunc = null;
                VariableManager.EndBlock();
                return base.VisitEnd_of_action(context);
            }

            /* Commands */

            public override object VisitCmd_add_item([NotNull] scheme_langParser.Cmd_add_itemContext context)
            {

                if (context.character_name() == null) return base.VisitCmd_add_item(context);
                if (context.item_name() == null) return base.VisitCmd_add_item(context);

                string characterName = context.character_name().GetText();
                string itemName = context.item_name().GetText();
                string itemNumber = "1";

                VariableManager.CheckValueValidity(characterName, this, context.character_name());
                VariableManager.CheckValueValidity(itemName, this, context.item_name());

                if (context.item_number() != null) itemNumber = GetRegName(0);

                CommandAddItem cmd = new CommandAddItem(characterName, itemNumber, itemName);
                currentFunc.AddCommand(cmd);

                if (context.item_number() != null) SetNewExpression(1);

                return base.VisitCmd_add_item(context);
            }

            public override object VisitCmd_create_var([NotNull] scheme_langParser.Cmd_create_varContext context)
            {
                var var_def = context.variable_definition();

                string var_type = var_def.variable_type()?.GetText();
                string var_name = var_def.variable_name()?.GetText();

                if (var_type == null || var_name == null)
                {
                    if (var_def.EQUALS() != null) SetNewExpression(1);
                    return base.VisitCmd_create_var(context);
                }

                if (!VariableManager.IsTypeValid(var_type, Config))
                {
                    Errors.Add(new ErrorDescriptor($"Type '{var_type}' is not recognized.", var_def.variable_type()));
                }

                VariableManager.CheckNewVariable(var_name, this, var_def.variable_name());

                CommandCreateVariable create_cmd = new CommandCreateVariable(var_type, var_name);
                currentFunc.AddCommand(create_cmd);

                if (var_def.EQUALS() != null)
                {
                    CommandSetVariable cmd = new CommandSetVariable(var_name, GetRegName(0));
                    currentFunc.AddCommand(cmd);
                    SetNewExpression(1);
                }

                return base.VisitCmd_create_var(context);
            }

            public override object VisitCmd_desc([NotNull] scheme_langParser.Cmd_descContext context)
            {
                if (context.content() == null) return base.VisitCmd_desc(context);

                string string_const_name = context.content().GetText();

                CommandDesc cmd = new CommandDesc(string_const_name);

                if (Config.GetStringConstByName(string_const_name) == null)
                {
                    Errors.Add(new ErrorDescriptor($"String const '{string_const_name}' does not exist.", context.content()));
                }

                currentFunc.AddCommand(cmd);

                return base.VisitCmd_desc(context);
            }

            public override object VisitCmd_toggle([NotNull] scheme_langParser.Cmd_toggleContext context)
            {
                string object_name = context.object_name()?.GetText();

                if (object_name != null)
                {
                    VariableManager.CheckValueValidity(object_name, this, context.object_name());

                    CommandToggle cmd = new CommandToggle(object_name);
                    currentFunc.AddCommand(cmd);
                }

                return base.VisitCmd_toggle(context);
            }

            public override object VisitCmd_end([NotNull] scheme_langParser.Cmd_endContext context)
            {
                CommandEnd cmd = new CommandEnd();
                currentFunc.AddCommand(cmd);
                return base.VisitCmd_end(context);
            }

            public override object VisitCmd_fail([NotNull] scheme_langParser.Cmd_failContext context)
            {
                CommandFail cmd = new CommandFail();
                currentFunc.AddCommand(cmd);
                return base.VisitCmd_fail(context);
            }
            
            public override object VisitAdd_action([NotNull] scheme_langParser.Add_actionContext context)
            {
                string action_name = context.action_name().GetText();

                MissingActionErrors.Add(new MissingActionErrorDescriptor(action_name, $"Action '{action_name}' does not exist.", context.action_name()));

                CommandAddAction cmd = new CommandAddAction(action_name);
                currentFunc.AddCommand(cmd);
                return base.VisitAdd_action(context);
            }

            public override object VisitRemove_action([NotNull] scheme_langParser.Remove_actionContext context)
            {
                string action_name = context.action_name().GetText();
                
                MissingActionErrors.Add(new MissingActionErrorDescriptor(action_name, $"Action '{action_name}' does not exist.", context.action_name()));

                CommandRemoveAction cmd = new CommandRemoveAction(action_name);
                currentFunc.AddCommand(cmd);
                return base.VisitRemove_action(context);
            }

            public override object VisitClear_actions([NotNull] scheme_langParser.Clear_actionsContext context)
            {
                CommandClearActions cmd = new CommandClearActions();
                currentFunc.AddCommand(cmd);
                return base.VisitClear_actions(context);
            }

            public override object VisitCmd_remove_item([NotNull] scheme_langParser.Cmd_remove_itemContext context)
            {

                string characterName = context.character_name().GetText();
                string itemName = context.item_name().GetText();
                string itemNumber = "1";

                bool expl = true;

                VariableManager.CheckValueValidity(characterName, this, context.character_name());
                VariableManager.CheckValueValidity(itemName, this, context.item_name());

                if (context.item_number() != null)
                {
                    itemNumber = GetRegName(0);
                    expl = false;
                }

                CommandRemoveItem cmd = new CommandRemoveItem(characterName, itemNumber, itemName, expl);
                currentFunc.AddCommand(cmd);

                if (context.item_number() != null) SetNewExpression(1);

                return base.VisitCmd_remove_item(context);
            }

            public override object VisitCmd_remove_spell([NotNull] scheme_langParser.Cmd_remove_spellContext context)
            {
                string characterName = context.character_name().GetText();
                string spellName = context.spell_name().GetText();

                VariableManager.CheckValueValidity(characterName, this, context.character_name());
                VariableManager.CheckValueValidity(spellName, this, context.spell_name());

                CommandRemoveSpell cmd = new CommandRemoveSpell(characterName, spellName);
                currentFunc.AddCommand(cmd);

                return base.VisitCmd_remove_spell(context);
            }

            public override object VisitCmd_report([NotNull] scheme_langParser.Cmd_reportContext context)
            {
                if (context.content() == null) return base.VisitCmd_report(context);

                string string_const_name = context.content().GetText();

                if (Config.GetStringConstByName(string_const_name) == null)
                {
                    Errors.Add(new ErrorDescriptor($"String const '{string_const_name}' does not exist.", context.content()));
                }

                CommandReport cmd = new CommandReport(string_const_name);
                currentFunc.AddCommand(cmd);

                return base.VisitCmd_report(context);
            }

            public override object VisitCmd_set_attr([NotNull] scheme_langParser.Cmd_set_attrContext context)
            {


                string attr_name = context.attr_name().GetText();
                string attr_type = context.attr_type().GetText();

                int objectReg = 0;

                ISchemeCommand cmd = null;

                if (attr_type == "set") cmd = new CommandSetAttribute(GetRegName(objectReg), attr_name);
                else if (attr_type == "remove") cmd = new CommandRemoveAttribute(GetRegName(objectReg), attr_name);
                else if (attr_type == "forbid") cmd = new CommandForbidAttribute(GetRegName(objectReg), attr_name);

                currentFunc.AddCommand(cmd);

                SetNewExpression(1);

                return base.VisitCmd_set_attr(context);
            }

            public override object VisitCmd_set_var([NotNull] scheme_langParser.Cmd_set_varContext context)
            {
                string varName = context.variable_name().GetText();

                if (!VariableManager.IsVariable(varName, Scheme))
                {
                    Errors.Add(new ErrorDescriptor($"Variable '{varName}' is not recognized.", context.variable_name()));
                }

                CommandSetVariable cmd = new CommandSetVariable(varName, GetRegName(0));
                currentFunc.AddCommand(cmd);

                SetNewExpression(1);

                return base.VisitCmd_set_var(context);
            }

            public override object VisitCmd_set_of([NotNull] scheme_langParser.Cmd_set_ofContext context)
            {

                string propertyName = context.property_name().GetText();

                int objectReg = 0;
                int valueReg = 1;

                CommandSetOf cmd = new CommandSetOf(propertyName, GetRegName(objectReg), GetRegName(valueReg));
                currentFunc.AddCommand(cmd);

                SetNewExpression(2);

                return base.VisitCmd_set_of(context);
            }

            public override object VisitCmd_teach_spell([NotNull] scheme_langParser.Cmd_teach_spellContext context)
            {

                if (context.character_name() == null || context.spell_name() == null) return base.VisitCmd_teach_spell(context);

                string characterName = context.character_name().GetText();
                string spellName = context.spell_name().GetText();

                VariableManager.CheckValueValidity(characterName, this, context.character_name());
                VariableManager.CheckValueValidity(spellName, this, context.spell_name());

                CommandAddSpell cmd = new CommandAddSpell(characterName, spellName);
                currentFunc.AddCommand(cmd);

                return base.VisitCmd_teach_spell(context);
            }

            /* If-expression */

            private List<CommandJumpBase> jump_cmds = new List<CommandJumpBase>();

            private void SetLatestJumpPosition(int line)
            {
                jump_cmds.Last().SetLine(line);
                jump_cmds.RemoveAt(jump_cmds.Count - 1);
            }

            public override object VisitCmd_if([NotNull] scheme_langParser.Cmd_ifContext context)
            {
                string value = GetRegName(0);
                CommandJumpIfFalse cmd = new CommandJumpIfFalse(value,-1);
                currentFunc.AddCommand(cmd);

                jump_cmds.Add(cmd);

                SetNewExpression(1);

                VariableManager.NewBlock();

                return base.VisitCmd_if(context);
            }

            public override object VisitElse([NotNull] scheme_langParser.ElseContext context)
            {
                CommandJump jump = new CommandJump(-1);
                currentFunc.AddCommand(jump);

                //we must set jump position of latest if (jump-if-false command)
                SetLatestJumpPosition(currentFunc.GetCommandCount());

                jump_cmds.Add(jump);

                VariableManager.EndBlock(); // end if-block
                VariableManager.NewBlock(); // start else-block

                return base.VisitElse(context);
            }

            public override object VisitEndif([NotNull] scheme_langParser.EndifContext context)
            {
                SetLatestJumpPosition(currentFunc.GetCommandCount());

                VariableManager.EndBlock();

                return base.VisitEndif(context);
            }

            /* Expressions */

            List<int> param_regs = new List<int>();
            int expression_index;

            private string GetRegName(int reg)
            {
                return "_" + reg;
            }

            private int GetLastParamReg() { return param_regs.Last(); }

            private int PopLastParamReg()
            {
                if (param_regs.Count > 0)
                {
                    int last = param_regs.Last();
                    param_regs.RemoveAt(param_regs.Count - 1);
                    return last;
                }
                throw new SchemeLangException("Empty param reg list");
            }

            private void PushParamRegs(params int[] regs)
            {
                foreach(int reg in regs)
                {
                    param_regs.Add(reg);
                }
            }

            private int GetFirstFreeReg()
            {
                for(int i = 0; ; ++i)
                {
                    if (!param_regs.Contains(i)) return i;
                }
            }
            
            private void SetNewExpression(int paramNum)
            {
                param_regs.Clear();
                for(int i = paramNum - 1; i >= 0; --i)
                    param_regs.Add(i);
                expression_index = currentFunc.GetCommandCount() - 1;
                if (expression_index < 0) expression_index = 0;
            }

            /* String const expression */

            public override object VisitString_const_expr([NotNull] scheme_langParser.String_const_exprContext context)
            {

                int result_reg = PopLastParamReg();

                string string_const_name = context.string_const().GetText();

                if (Config.GetStringConstByName(string_const_name) == null)
                {
                    Errors.Add(new ErrorDescriptor($"String const '{string_const_name}' does not exist.", context));
                }

                CommandSetVariable cmd = new CommandSetVariable(GetRegName(result_reg), string_const_name);
                currentFunc.AddCommand(cmd, expression_index);

                return base.VisitString_const_expr(context);
            }

            /* Numeric expressions */

            public override object VisitComplex_numeric_expr([NotNull] scheme_langParser.Complex_numeric_exprContext context)
            {
                //Insert ADD/SUB to computation stack and registers to register stack

                //result reg = params.back()
                //param1 = result reg
                //param2 = first free reg (that is not in params)
                //params.push_back(param2)

                int param2 = GetFirstFreeReg();
                int param1 = PopLastParamReg();

                PushParamRegs(param2, param1);

                ISchemeCommand cmd = null;

                if (context.PLUS() != null)
                    cmd = new CommandAdd(GetRegName(param1), GetRegName(param2), GetRegName(param1));
                else if (context.MINUS() != null)
                    cmd = new CommandSubtract(GetRegName(param1), GetRegName(param2), GetRegName(param1));

                currentFunc.AddCommand(cmd, expression_index);

                return base.VisitComplex_numeric_expr(context);
            }

            public override object VisitComplex_multiplying_expr([NotNull] scheme_langParser.Complex_multiplying_exprContext context)
            {

                int param2 = GetFirstFreeReg();
                int param1 = PopLastParamReg();

                PushParamRegs(param2, param1);

                ISchemeCommand cmd = null;

                if (context.MUL() != null)
                    cmd = new CommandMultiply(GetRegName(param1), GetRegName(param2), GetRegName(param1));
                else if (context.DIV() != null)
                    cmd = new CommandDivide(GetRegName(param1), GetRegName(param2), GetRegName(param1));

                currentFunc.AddCommand(cmd, expression_index);

                return base.VisitComplex_multiplying_expr(context);
            }

            public override object VisitInverted_atom([NotNull] scheme_langParser.Inverted_atomContext context)
            {

                int param = GetLastParamReg();

                CommandSubtract cmd = new CommandSubtract("0", GetRegName(param), GetRegName(param));
                currentFunc.AddCommand(cmd, expression_index);

                return base.VisitInverted_atom(context);
            }

            public override object VisitInteger_expr([NotNull] scheme_langParser.Integer_exprContext context)
            {

                int result_reg = PopLastParamReg();

                CommandSetVariable cmd = new CommandSetVariable(GetRegName(result_reg), context.integer().GetText());
                currentFunc.AddCommand(cmd, expression_index);

                return base.VisitInteger_expr(context);
            }

            public override object VisitVariable_expr([NotNull] scheme_langParser.Variable_exprContext context)
            {

                //variable_expr can also be 'of' expression, in that case we do nothing here
                if (context.variable_name() != null)
                {
                    int result_reg = PopLastParamReg();

                    string varName = context.variable_name().GetText();

                    VariableManager.CheckValueValidity(varName, this, context.variable_name());

                    CommandSetVariable cmd = new CommandSetVariable(GetRegName(result_reg), varName);
                    currentFunc.AddCommand(cmd, expression_index);
                }

                return base.VisitVariable_expr(context);
            }

            public override object VisitProperty_of([NotNull] scheme_langParser.Property_ofContext context)
            {

                string propertyName = context.property_name().GetText();
                int targetReg = GetLastParamReg();

                CommandOf cmd = new CommandOf(propertyName, GetRegName(targetReg), GetRegName(targetReg));

                currentFunc.AddCommand(cmd, expression_index);

                return base.VisitProperty_of(context);
            }

            public override object VisitObject_atom([NotNull] scheme_langParser.Object_atomContext context)
            {

                int result_reg = PopLastParamReg();

                string objName = context.object_name().GetText();

                VariableManager.CheckValueValidity(objName, this, context.object_name());

                CommandSetVariable cmd = new CommandSetVariable(GetRegName(result_reg), objName);
                currentFunc.AddCommand(cmd, expression_index);

                return base.VisitObject_atom(context);
            }

            /* Logical expressions */

            public override object VisitComplex_logical_expr([NotNull] scheme_langParser.Complex_logical_exprContext context)
            {

                //add OR-expression

                int param2 = GetFirstFreeReg();
                int param1 = PopLastParamReg();
                PushParamRegs(param2, param1);

                CommandOr cmd = new CommandOr(GetRegName(param1), GetRegName(param2), GetRegName(param1));
                currentFunc.AddCommand(cmd, expression_index);

                return base.VisitComplex_logical_expr(context);
            }

            public override object VisitComplex_and_expr([NotNull] scheme_langParser.Complex_and_exprContext context)
            {

                //add AND-expression

                int param2 = GetFirstFreeReg();
                int param1 = PopLastParamReg();
                PushParamRegs(param2, param1);

                CommandAnd cmd = new CommandAnd(GetRegName(param1), GetRegName(param2), GetRegName(param1));
                currentFunc.AddCommand(cmd, expression_index);

                return base.VisitComplex_and_expr(context);
            }

            public override object VisitInverted_logical_atom([NotNull] scheme_langParser.Inverted_logical_atomContext context)
            {

                int param = GetLastParamReg();

                CommandNot cmd = new CommandNot(GetRegName(param), GetRegName(param));
                currentFunc.AddCommand(cmd, expression_index);

                return base.VisitInverted_logical_atom(context);
            }

            public override object VisitLogical_const_expr([NotNull] scheme_langParser.Logical_const_exprContext context)
            {

                int param = PopLastParamReg();

                string logical_const = context.logical_const().GetText();

                CommandSetVariable cmd = new CommandSetVariable(GetRegName(param), logical_const);
                currentFunc.AddCommand(cmd, expression_index);

                return base.VisitLogical_const_expr(context);
            }

            public override object VisitHas_item([NotNull] scheme_langParser.Has_itemContext context)
            {

                int param = PopLastParamReg();

                string character_name = context.character_name().GetText();
                string item_name = context.item_name().GetText();
                string number = "1";

                VariableManager.CheckValueValidity(character_name, this, context.character_name());
                VariableManager.CheckValueValidity(item_name, this, context.item_name());

                if (context.item_number() != null)
                {
                    int reg = GetFirstFreeReg();
                    PushParamRegs(reg);
                    number = GetRegName(reg);
                }

                if (context.NOT() != null)
                {
                    currentFunc.AddCommand(new CommandNot(GetRegName(param), GetRegName(param)), expression_index);
                }

                CommandOwns cmd = new CommandOwns(character_name, number, item_name, GetRegName(param));
                currentFunc.AddCommand(cmd, expression_index);

                return base.VisitHas_item(context);
            }

            public override object VisitKnows_spell([NotNull] scheme_langParser.Knows_spellContext context)
            {

                int param = PopLastParamReg();

                string character_name = context.character_name().GetText();
                string spell_name = context.spell_name().GetText();

                VariableManager.CheckValueValidity(character_name, this, context.character_name());
                VariableManager.CheckValueValidity(spell_name, this, context.spell_name());

                if (context.NOT() != null)
                {
                    currentFunc.AddCommand(new CommandNot(GetRegName(param), GetRegName(param)), expression_index);
                }

                CommandKnows cmd = new CommandKnows(character_name, spell_name, GetRegName(param));
                currentFunc.AddCommand(cmd, expression_index);

                return base.VisitKnows_spell(context);
            }

            public override object VisitIs([NotNull] scheme_langParser.IsContext context)
            {

                int param = GetLastParamReg();

                CommandIs cmd = new CommandIs(GetRegName(param), context.identifier().GetText(), GetRegName(param));
                currentFunc.AddCommand(cmd, expression_index);

                return base.VisitIs(context);
            }

            public override object VisitComparison([NotNull] scheme_langParser.ComparisonContext context)
            {

                int param2 = GetFirstFreeReg();
                int param1 = PopLastParamReg();
                PushParamRegs(param2, param1);

                if (context.relational_operator().EQUALS() != null)
                {
                    currentFunc.AddCommand(new CommandEquals(GetRegName(param1), GetRegName(param2), GetRegName(param1)), expression_index);
                }
                else if (context.relational_operator().GREATER() != null)
                {
                    currentFunc.AddCommand(new CommandCmp(GetRegName(param1), ">", GetRegName(param2), GetRegName(param1)), expression_index);
                }
                else if (context.relational_operator().GREATER_EQUALS() != null)
                {
                    currentFunc.AddCommand(new CommandCmp(GetRegName(param1), ">=", GetRegName(param2), GetRegName(param1)), expression_index);
                }
                else if (context.relational_operator().LOWER() != null)
                {
                    currentFunc.AddCommand(new CommandCmp(GetRegName(param1), "<", GetRegName(param2), GetRegName(param1)), expression_index);
                }
                else if (context.relational_operator().LOWER_EQUALS() != null)
                {
                    currentFunc.AddCommand(new CommandCmp(GetRegName(param1), "<=", GetRegName(param2), GetRegName(param1)), expression_index);
                }

                return base.VisitComparison(context);
            }

        }
        
        /* Compilation */
        
        public static IParseTree GetAST(string code)
        {
            var inputStream = new AntlrInputStream(code);
            var lexer = new scheme_langLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new scheme_langParser(tokenStream);
            var context = parser.doc();
            return context;
        }

        public static IParseTree GenerateAST(string code, ref List<ErrorDescriptor> errors)
        {
            var inputStream = new AntlrInputStream(code);
            var lexer = new scheme_langLexer(inputStream);

            lexer.RemoveErrorListeners();
            LexerErrorListener errorListener = new LexerErrorListener(errors);
            lexer.AddErrorListener(errorListener);

            var tokenStream = new CommonTokenStream(lexer);
            var parser = new scheme_langParser(tokenStream);

            parser.RemoveErrorListeners();
            ParserErrorListener errorListener2 = new ParserErrorListener(errors);
            parser.AddErrorListener(errorListener2);

            var context = parser.doc();

            return context;
        }

        public static CompiledScheme Compile(Scheme scheme, Config config)
        {
            var ast = GetAST(scheme.Code);
            var visitor = new Visitor(scheme, config);

            try
            {
                visitor.Visit(ast);
            }
            catch (SchemeLangException ex)
            {
                Log.Write($"Compilation was terminated by an exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
            }

            return visitor.CompiledScheme;
        }

        public static List<ErrorDescriptor> CompileWithErrors(Scheme scheme, Config config)
        {
            List<ErrorDescriptor> errors = new List<ErrorDescriptor>();

            var ast = GenerateAST(scheme.Code, ref errors);
            var visitor = new Visitor(scheme, config);
            visitor.Errors = errors;

            try
            {
                visitor.Visit(ast);
            }
            catch (SchemeLangException ex)
            {
                Log.Write($"Compilation was terminated by an exception: {ex.Message}");
            }

            foreach (var missingActionError in visitor.MissingActionErrors)
            {
                if (missingActionError.IsError(scheme)) visitor.Errors.Add(missingActionError);
            }

            return visitor.Errors;
        }

    }
}
