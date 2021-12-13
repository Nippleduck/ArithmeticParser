using System;

namespace Analyzer.Errors
{
    public class SyntaxError
    {
        public SyntaxError(string error, ConsoleColor color)
        {
            this.error = error;
            this.color = color;
        }

        public readonly string error;
        public readonly ConsoleColor color;
    }
}
