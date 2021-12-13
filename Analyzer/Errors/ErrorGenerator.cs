using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Analyzer.Errors
{
    static class ErrorGenerator
    {
        public static List<SyntaxError> GenerateFromMatches(
            MatchCollection matches,
            string errorType,
            ConsoleColor color) => 
            matches.ToList().ConvertAll(match =>
            new SyntaxError($"{errorType} [{match.Value}] at position [{match.Index}]", color));
       
    }
}
