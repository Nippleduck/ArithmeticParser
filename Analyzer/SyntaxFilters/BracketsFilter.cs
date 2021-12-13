using Analyzer.Errors;
using Analyzer.Results;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Analyzer.SyntaxFilters
{
    class BracketsFilter : ISyntaxFilter
    {
        private const string SingularBracketsPattern = "(.?\\(.?)|(.?\\).?)";
        private const string DoubleBracketsPattern = "(\\(\\))";
        private const string NestedExpressionPattern = "[-+/*]?\\((((-?[a-zA-Z]+)|(-?([a-z$&!]+)?\\d+(.?\\d*)?([a-z$&!]+)?))" +
                                                       "[-+/*]+((-?[a-zA-Z]+)|(-?([a-z$&!]+)?\\d+(.?\\d*)?([a-z$&!]+)?))[-+*/]*)+\\)";

        public SyntaxCheckResult Check(string expression)
        {
            var errors = new List<SyntaxError>();
            var formated = RemoveEmptyBrackets(ref expression);

            if (!formated.correct) errors.AddRange(formated.errors);

            var nonNested = RemoveNestedExpressions(expression);
            var excessive = CheckForExcessiveBrackets(nonNested);

            if (!excessive.correct) errors.AddRange(excessive.errors);

            return new SyntaxCheckResult(errors);
        }

        private static SyntaxCheckResult RemoveEmptyBrackets(ref string expression)
        {
            var errors = new List<SyntaxError>();
            var matches = Regex.Matches(expression, DoubleBracketsPattern);

            if (matches.Count is not 0)
            {
                foreach (Match match in matches)
                {
                    errors.Add(new SyntaxError(
                        $"Empty brackets [{match.Value}] at position [{match.Index}]",
                        ConsoleColor.DarkYellow));
                }

                expression = Regex.Replace(expression, DoubleBracketsPattern, string.Empty);
            }

            return new SyntaxCheckResult(errors);
        }

        private static string RemoveNestedExpressions(string expression)
        {
            var nested = Regex.Matches(expression, NestedExpressionPattern).Count;

            if (nested is 0) return expression;

            var nonNested = Regex.Replace(expression, NestedExpressionPattern, string.Empty);

            return RemoveNestedExpressions(nonNested);
        }

        public static SyntaxCheckResult CheckForExcessiveBrackets(string expression)
        {
            var errors = new List<SyntaxError>();

            var excessive = Regex.Matches(expression, SingularBracketsPattern);

            if (excessive.Count is not 0) 
                errors.AddRange(ErrorGenerator.GenerateFromMatches(
                excessive, "Excessive bracket(s)", ConsoleColor.DarkYellow));

            return new SyntaxCheckResult(errors);
        }
    }
}
