using Analyzer.Results;

namespace Analyzer.SyntaxFilters
{
    public interface ISyntaxFilter
    {
        SyntaxCheckResult Check(string expression);
    }
}