using Analyzer.Errors;
using Analyzer.Results;
using Analyzer.SyntaxFilters;
using System.Collections.Generic;
using Utility;

namespace Analyzer
{
    public class SyntaxAnalyzer
    {
        public SyntaxAnalyzer(List<ISyntaxFilter> filters) => this.filters = filters;

        public SyntaxAnalyzer()
        {
            filters = new List<ISyntaxFilter>
            {
                new BracketsFilter(),
                new VariableFilter(),
                new SignFilter()
            };
        }

        private readonly List<ISyntaxFilter> filters;

        public SyntaxCheckResult Analyze(string expression)
        {
            var errors = new List<SyntaxError>();
            var formated = StringFormater.RemoveSpaces(expression);

            foreach (var filter in filters)
            {
                var filtered = filter.Check(formated);

                if (!filtered.correct) errors.AddRange(filtered.errors);
            }

            return new SyntaxCheckResult(errors);
        }
    }
}