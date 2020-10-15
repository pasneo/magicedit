using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit.language
{
    public class LexerErrorListener : IAntlrErrorListener<int>
    {
        public List<ErrorDescriptor> Errors { get; private set; }

        public LexerErrorListener(List<ErrorDescriptor> errors)
        {
            Errors = errors;
        }

        public void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] int offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            if (e != null && e.OffendingToken != null)
                Errors.Add(new ErrorDescriptor(msg, line, charPositionInLine, e.OffendingToken.StartIndex, e.OffendingToken.StopIndex));
            else
                Errors.Add(new ErrorDescriptor(msg, line, charPositionInLine));
        }
    }

    public class ParserErrorListener : IAntlrErrorListener<IToken>
    {
        public List<ErrorDescriptor> Errors { get; private set; }

        public ParserErrorListener(List<ErrorDescriptor> errors)
        {
            Errors = errors;
        }

        public void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            if (offendingSymbol != null)
                Errors.Add(new ErrorDescriptor(msg, line, charPositionInLine, offendingSymbol.StartIndex, offendingSymbol.StopIndex));
            else
                Errors.Add(new ErrorDescriptor(msg, line, charPositionInLine));
        }
    }
}
