using Analyzer.Errors;
using Analyzer.Results;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Analyzer.SyntaxFilters
{
    class VariableFilter : ISyntaxFilter
    {
        private enum VariableCheckType
        {
            IncorrectVariable,
            IncorrectDecimal,
            NoActionVariable,
            StandaloneVariable
        }

        private readonly Dictionary<VariableCheckType, string> patterns =
            new()
            {
                { VariableCheckType.IncorrectVariable, "((?<=[\\d\\w$&!@]+)[a-zA-Z$&!]+)|" +
                                                        "((?<=[\\d-+*/])[a-zA-Z]+(?=[\\d\\w$&!@]+))" },
                { VariableCheckType.IncorrectDecimal, "(-?[\\d\\w$!@]\\.+(?=[-+*/a-zA-Z() ]))|(\\d\\.\\d\\.)+" },
                { VariableCheckType.NoActionVariable, "((?<=[)])[\\d\\w$!@])|([\\d\\w$!@](?=[(]-?[\\d\\w]))" },
                { VariableCheckType.StandaloneVariable, "(?<=[(])-?[\\d\\w$!@.]+(?=[)])" }
            };

        public SyntaxCheckResult Check(string expression)
        {
            var errors = new List<SyntaxError>();

            foreach (var pattern in patterns)
            {
                var matches = Regex.Matches(expression, pattern.Value);

                if (matches.Count is not 0)
                errors.AddRange(ErrorGenerator.GenerateFromMatches(
                    matches, pattern.Value, ConsoleColor.DarkGray));
            }

            return new SyntaxCheckResult(errors);
        }
    }
}
