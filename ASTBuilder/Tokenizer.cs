using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utility;

namespace ASTBuilder
{
    public enum TokenType
    {
        Plus,
        Minus,
        Multiply,
        Division,
        Operand,
        RBrace,
        LBrace,
        Group,
        EOF
    }

    public class Token
    {
        public Token(string value, TokenType type, int priority)
        {
            this.value = value;
            this.type = type;
            this.priority = priority;
        }

        public readonly string value;
        public readonly TokenType type;
        public readonly int priority;
    }

    public class Tokenizer
    {
        public Tokenizer(string expression)
        {
            var formated = StringFormater.RemoveSpaces(expression);
            iterator = new Iterator<char>(formated.ToList(), IterationOptions.ToCollectionEnd);
            tokens = new List<Token>();
        }

        private Dictionary<TokenType, char> OperationTypes { get; } =
            new Dictionary<TokenType, char>
            {
                { TokenType.Minus, '-' },
                { TokenType.Plus, '+' },
                { TokenType.Division, '/' },
                { TokenType.Multiply, '*' }
            };

        private const string operationChars = "[-+/*]";
        private const string operandChars = "[.a-zA-Z\\d]";

        private readonly Iterator<char> iterator;
        private readonly List<Token> tokens;

        public List<Token> TokenizeExpression()
        {
            while (!EndOfExpression())
            {
                tokens.Add(ToToken(iterator.CurrentElement));
                iterator.MoveForward();
            }

            return tokens;
        }

        private Token ToToken(char currentChar) => currentChar.ToString() switch
        {
            var operation when Regex.IsMatch(operation, operationChars) => GenerateOperationToken(),
            var operand when Regex.IsMatch(operand, operandChars)  => GenerateOperandToken(),
            "(" => new Token(currentChar.ToString(), TokenType.LBrace, 3),
            ")" => new Token(currentChar.ToString(), TokenType.RBrace, 3),
            _ => throw new Exception("Unknown expression character")
        };

        private Token GenerateOperationToken()
        {
            var type = OperationTypes.First(operationType => operationType.Value == iterator.CurrentElement);
            var priority = (type.Value == '+') || (type.Value == '-') ? 1 : 2;

            return new Token(iterator.CurrentElement.ToString(), type.Key, priority);
        }

        private Token GenerateOperandToken()
        {
            var operand = new StringBuilder();

            while (Regex.IsMatch(iterator.CurrentElement.ToString(), operandChars))
            {
                operand.Append(iterator.CurrentElement);
                iterator.MoveForward();
            }

            if(iterator.NextElement is not default(char) || EndsWithBrackets()) 
                iterator.MoveBackward();

            return new Token(operand.ToString(), TokenType.Operand, 0);
        }

        private bool EndsWithBrackets() => iterator.CurrentElement is ')' &&
            iterator.NextElement is default(char);

        private bool EndOfExpression() => iterator.CurrentElement is default(char);
    }
}
