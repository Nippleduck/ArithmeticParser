using Analyzer;
using ASTBuilder;
using System;

namespace arithmetic_parser
{
    class Program
    {
        static void Main(string[] args)
        {
            bool going = true;

            while (going)
            {
                var analyzer = new SyntaxAnalyzer();

                Console.WriteLine("Enter your expression:");
                var expression = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(expression))
                    throw new Exception("expression is empty");

                var analized = analyzer.Analyze(expression);

                if (!analized.correct) analized.errors.ForEach(error => PrintError(error.error));
                else
                {
                    PrintSuccess("Expression is correct!");

                    PrintInfo("Tokenizing expression...");
                    var tokenized = new Tokenizer(expression).TokenizeExpression();

                    tokenized.ForEach(token => Console.WriteLine(
                        $"Token: [{token.value}] Type: [{token.type}] Priority: [{token.priority}]"));

                    var groups = new GroupsConstructor(tokenized).ConstructFromTokens();
                    var root = new TreeBuilder(groups).Build();

                    PrintInfo("Visualizing tree...");
                    TreePrinter.Print(root);
                }
            }

            Console.ReadKey();
        }

        private static void PrintError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void PrintSuccess(string success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(success);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void PrintInfo(string info)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(info);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
