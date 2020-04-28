using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using magicedit.language.scheme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class SchemeLang
    {

        public class SchemeLangException : Exception
        {
            public SchemeLangException(string message) : base(message) { }
        }

        public class Visitor : scheme_langBaseVisitor<object>
        {

            public Scheme Scheme { get; set; }
            public Config Config { get; set; }
            public CompiledScheme CompiledScheme { get; set; } = new CompiledScheme();

            private SchemeFunction currentFunc;


            /* *** */

            public Visitor(Scheme scheme, Config config)
            {
                Scheme = scheme;
                Config = config;
                Scheme.CompiledScheme = CompiledScheme;
            }

            private ObjectVariable CreateVariableFromContext(scheme_langParser.Variable_definitionContext context)
            {
                string type = context.variable_type().GetText();
                string name = context.variable_name().GetText();
                object value = null;

                if (context.expression() != null)
                {
                    value = EvaluateExpression(context.expression());
                }

                return new ObjectVariable(type, name, value);
            }

            private int EvaluateNumericExpression(scheme_langParser.Numeric_expressionContext expr)
            {
                return 0;   //TODO
            }

            private object EvaluateExpression(scheme_langParser.ExpressionContext expression)
            {
                //TODO: evaluate expression's value
                if (expression.string_const() != null)
                {

                }
                else if (expression.logical_expression() != null)
                {

                }
                else if (expression.numeric_expression() != null)
                {
                    return EvaluateNumericExpression(expression.numeric_expression());
                }
                return null;
            }

            /* Scheme building */

            public override object VisitParent_name([NotNull] scheme_langParser.Parent_nameContext context)
            {
                //Add parent to scheme
                string parentName = context.GetText();
                Scheme parent = Config.GetSchemeByName(parentName);
                Scheme.AddParent(parent);
                return base.VisitParent_name(context);
            }

            public override object VisitBody_variable_definition([NotNull] scheme_langParser.Body_variable_definitionContext context)
            {
                //Add predefined variable to scheme
                var variable_definition = context.variable_definition();
                ObjectVariable variable = CreateVariableFromContext(variable_definition);
                CompiledScheme.AddVariable(variable);
                return base.VisitBody_variable_definition(context);
            }

            public override object VisitParameter_definition([NotNull] scheme_langParser.Parameter_definitionContext context)
            {
                //Add parameter to scheme
                string type = context.variable_type().GetText();
                string name = context.variable_name().GetText();
                ObjectVariable param = new ObjectVariable(type, name, null);
                CompiledScheme.AddParameter(param);
                return base.VisitParameter_definition(context);
            }

            public override object VisitInit_block([NotNull] scheme_langParser.Init_blockContext context)
            {
                //Sign start of init block
                currentFunc = new SchemeFunction("init");
                return base.VisitInit_block(context);
            }

            public override object VisitEnd_of_init([NotNull] scheme_langParser.End_of_initContext context)
            {
                //Add completed init function to scheme
                CompiledScheme.SetInit(currentFunc);
                currentFunc = null;
                return base.VisitEnd_of_init(context);
            }

            public override object VisitAction_block([NotNull] scheme_langParser.Action_blockContext context)
            {
                //Sign start of action
                string name = context.action_name().GetText();
                int actionPoints = int.Parse(context.action_point().GetText());
                currentFunc = new SchemeFunction(name, actionPoints);
                return base.VisitAction_block(context);
            }

            public override object VisitEnd_of_action([NotNull] scheme_langParser.End_of_actionContext context)
            {
                //Add completed action to scheme
                CompiledScheme.AddAction(currentFunc);
                currentFunc = null;
                return base.VisitEnd_of_action(context);
            }

            /* Commands */

            public override object VisitCmd_add_item([NotNull] scheme_langParser.Cmd_add_itemContext context)
            {
                return base.VisitCmd_add_item(context);
            }

            public override object VisitCmd_create_classvar([NotNull] scheme_langParser.Cmd_create_classvarContext context)
            {
                return base.VisitCmd_create_classvar(context);
            }

            public override object VisitCmd_create_var([NotNull] scheme_langParser.Cmd_create_varContext context)
            {
                return base.VisitCmd_create_var(context);
            }

            public override object VisitCmd_desc([NotNull] scheme_langParser.Cmd_descContext context)
            {
                return base.VisitCmd_desc(context);
            }

            //public override object VisitCmd_destroy([NotNull] scheme_langParser.Cmd_destroyContext context)
            //{
            //    return base.VisitCmd_destroy(context);
            //}

            public override object VisitCmd_fail([NotNull] scheme_langParser.Cmd_failContext context)
            {
                return base.VisitCmd_fail(context);
            }

            public override object VisitCmd_if([NotNull] scheme_langParser.Cmd_ifContext context)
            {
                return base.VisitCmd_if(context);
            }

            public override object VisitCmd_manage_actions([NotNull] scheme_langParser.Cmd_manage_actionsContext context)
            {
                return base.VisitCmd_manage_actions(context);
            }

            public override object VisitCmd_modify_var([NotNull] scheme_langParser.Cmd_modify_varContext context)
            {
                return base.VisitCmd_modify_var(context);
            }

            public override object VisitCmd_remove_item([NotNull] scheme_langParser.Cmd_remove_itemContext context)
            {
                return base.VisitCmd_remove_item(context);
            }

            public override object VisitCmd_remove_spell([NotNull] scheme_langParser.Cmd_remove_spellContext context)
            {
                return base.VisitCmd_remove_spell(context);
            }

            public override object VisitCmd_report([NotNull] scheme_langParser.Cmd_reportContext context)
            {
                return base.VisitCmd_report(context);
            }

            public override object VisitCmd_set_attr([NotNull] scheme_langParser.Cmd_set_attrContext context)
            {
                return base.VisitCmd_set_attr(context);
            }

            public override object VisitCmd_set_var([NotNull] scheme_langParser.Cmd_set_varContext context)
            {
                
                CommandSetVariable cmd = new CommandSetVariable(context.variable_name().GetText(), GetRegName(0));
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
                return base.VisitCmd_teach_spell(context);
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
            }

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

            public override object VisitInteger_expr([NotNull] scheme_langParser.Integer_exprContext context)
            {

                int result_reg = PopLastParamReg();

                CommandSetVariable cmd = new CommandSetVariable(GetRegName(result_reg), context.integer().GetText());
                currentFunc.AddCommand(cmd, expression_index);

                return base.VisitInteger_expr(context);
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

                CommandSetVariable cmd = new CommandSetVariable(GetRegName(result_reg), context.object_name().GetText());
                currentFunc.AddCommand(cmd, expression_index);

                return base.VisitObject_atom(context);
            }

        }
        
        public static IParseTree GetAST(string code)
        {
            var inputStream = new AntlrInputStream(code);
            var lexer = new scheme_langLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new scheme_langParser(tokenStream);
            var context = parser.doc();
            return context;
        }

        public static CompiledScheme Compile(Scheme scheme, Config config)
        {
            var ast = GetAST(scheme.Code);
            var visitor = new Visitor(scheme, config);
            visitor.Visit(ast);
            return visitor.CompiledScheme;
        }

    }
}
