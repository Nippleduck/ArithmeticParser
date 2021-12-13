using Analyzer.Errors;
using Analyzer.Results;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Analyzer.SyntaxFilters
{
    class SignFilter : ISyntaxFilter
    {
        private enum SignCheckType
        {
            DoubleSign,
            StandaloneSign,
            NoUseSign
        }

        private readonly Dictionary<SignCheckType, string> patterns =
            new() 
            {
                { SignCheckType.DoubleSign, "([-+*\\/%]=?){2,}" },
                { SignCheckType.NoUseSign, "(?<=[\\d\\w.$&]+)[-+/*](?![-+/*\\d\\w.]+|\\([\\d\\w-+])" },
                { SignCheckType.StandaloneSign, "((?<![\\d\\w.]+)[-+/*](?![\\d\\w.]+|\\([\\d\\w-+]))" +
                                                "|((?<![-+/*\\d\\w().])[/*](?=[\\d\\w.]+|\\([\\d\\w-+]))"}
            };

        public SyntaxCheckResult Check(string expression)
        {
            var errors = new List<SyntaxError>();
            
            foreach(var pattern in patterns)
            {
                var checkResult = CheckForSignErrors(expression, pattern.Value, pattern.Key);

                if(!checkResult.correct) errors.AddRange(checkResult.errors);
            }

            return new SyntaxCheckResult(errors);
        }

        private static SyntaxCheckResult CheckForSignErrors(string expression, string pattern, SignCheckType checkType)
        {
            var errors = new List<SyntaxError>();
            var matches = Regex.Matches(expression, pattern);

            if (matches.Count is not 0) 
                errors.AddRange(ErrorGenerator.GenerateFromMatches(
                    matches, checkType.ToString(), ConsoleColor.DarkMagenta));

            return new SyntaxCheckResult(errors);
        }
    }
}
