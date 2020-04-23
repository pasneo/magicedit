using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class SchemeLang
    {

        public class Visitor : scheme_langBaseVisitor<object>
        {

            public override object VisitParent_name([NotNull] scheme_langParser.Parent_nameContext context)
            {
                //TODO: add parent to scheme
                return base.VisitParent_name(context);
            }

            public override object VisitBody_variable_definition([NotNull] scheme_langParser.Body_variable_definitionContext context)
            {
                //TODO: add predefined variable to scheme
                return base.VisitBody_variable_definition(context);
            }

            public override object VisitParameter_definition([NotNull] scheme_langParser.Parameter_definitionContext context)
            {
                //TODO: add parameter to scheme
                return base.VisitParameter_definition(context);
            }

            public override object VisitInit_block([NotNull] scheme_langParser.Init_blockContext context)
            {
                //TODO: sign start of init block
                return base.VisitInit_block(context);
            }

            public override object VisitAction_block([NotNull] scheme_langParser.Action_blockContext context)
            {
                //TODO: sign start of action
                return base.VisitAction_block(context);
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

    }
}
