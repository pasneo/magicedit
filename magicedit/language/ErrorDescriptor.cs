using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit.language
{
    public class ErrorDescriptor
    {
        public string Message { get; set; }
        public int Line { get; set; }
        public int Position { get; set; }

        public bool HasExactPosition { get; set; } = false;
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }

        public ErrorDescriptor(string message, int line, int position) { Message = message; Line = line; Position = position; }
        public ErrorDescriptor(string message, int line, int position, int startPosition, int endPosition)
        {
            Message = message; Line = line; Position = position; StartPosition = startPosition; EndPosition = endPosition;
            HasExactPosition = true;
        }

        public ErrorDescriptor(string message, ParserRuleContext context)
        {
            Message = message;
            Line = context.Start.Line;
            Position = context.Start.Column;
            StartPosition = context.Start.StartIndex;
            EndPosition = context.Start.StopIndex;
            HasExactPosition = true;
        }

    }
}
