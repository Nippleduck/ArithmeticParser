using System;
using System.Text.RegularExpressions;

namespace Utility
{
    public static class StringFormater
    {
        public static string RemoveSpaces(string expression)
        {
            return Regex.Replace(expression, " ", "");
        }
    }
}
