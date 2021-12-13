using Analyzer.Errors;
using System.Collections.Generic;

namespace Analyzer.Results
{
    public class SyntaxCheckResult
    {
        public SyntaxCheckResult(bool correct, List<SyntaxError> errors)
        {
            this.correct = correct;
            this.errors = errors;
        }

        public SyntaxCheckResult(List<SyntaxError> errors)
        {
            correct = errors.Count is 0;
            this.errors = errors;
        }

        public readonly bool correct;
        public readonly List<SyntaxError> errors;
    }
}