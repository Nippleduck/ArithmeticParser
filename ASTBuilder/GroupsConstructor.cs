using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace ASTBuilder
{
    public class Group
    {
        public Group(string name, int beginIndex, List<Token> tokens)
        {
            this.name = name;
            this.beginIndex = beginIndex;
            this.tokens = tokens;
            Initiate(tokens);
        }

        public readonly string name;
        public readonly int beginIndex;
        public readonly List<Token> tokens;
        public Token FirstOperand { get; private set; }
        public Token Operation { get; private set; }
        public Token SecondOperand { get; private set; }

        private void Initiate(List<Token> tokens)
        {
            var shift = tokens[0].type is TokenType.LBrace ? 1 : 0;

            FirstOperand = tokens[0 + shift];
            Operation = tokens[1 + shift];
            SecondOperand = tokens[2 + shift];
        }
    }

    public class GroupsConstructor
    {
        public GroupsConstructor(List<Token> tokens)
        {
            tokenIterator = new Iterator<Token>(tokens, IterationOptions.KeepIterating);
            groups = new List<Group>();
        }

        private readonly Iterator<Token> tokenIterator;
        private readonly List<Group> groups;

        public List<Group> ConstructFromTokens()
        {
            while (!IsLastGroup())
            {
                if (CanGroup())
                {
                    var captured = CaptureGroup();
                    if (!captured) tokenIterator.MoveForward();
                }
                else tokenIterator.MoveForward();
            }

            return groups;
        }

        private bool CanGroup() =>
                tokenIterator.CurrentElement.type is TokenType.Operand ||
                tokenIterator.CurrentElement.type is TokenType.LBrace ||
                tokenIterator.CurrentElement.type is TokenType.Group;

        private bool CaptureGroup()
        {
            if (NextTokenInvalid()) return false;

            if (HasOpeningBracket() && !HasClosingBracket()) return false;

            var groupTokens = new List<Token>();
            var startIndex = tokenIterator.CurrentIndex;
            var groupLength = IsBracketGroup() ? 5 : 3;

            if (HasOpeningBracket() && groupLength == 3) tokenIterator.MoveForward();

            for (int i = 0; i < groupLength; i++)
            {
                groupTokens.Add(tokenIterator.CurrentElement);
                tokenIterator.MoveForward();
            }

            return CreateGroup(startIndex, groupTokens);
        }

        private bool CreateGroup(int startIndex, List<Token> groupTokens)
        {
            var group = new Group($"[{groups.Count}]", startIndex, groupTokens);

            if (!IsCorrectGroup(group))
            {
                tokenIterator.MoveTo(startIndex);
                return false;
            }

            groups.Add(group);
            ReplaceWithToken(group);

            return true;
        }

        private bool NextTokenInvalid() => tokenIterator.NextElement is default(Token) ||
            tokenIterator.NextElement.type is TokenType.RBrace;

        private bool IsLastGroup() => tokenIterator.CollectionLength is 0;

        private bool IsBracketGroup() => HasOpeningBracket() && HasClosingBracket();

        private bool HasOpeningBracket() => tokenIterator.CurrentElement.type is TokenType.LBrace;

        private bool HasClosingBracket()
        {
            var last = tokenIterator.LookFromCurrentBy(4, Direction.Forward);
            return last is not default(Token) && last.type is TokenType.RBrace;
        }

        private bool IsCorrectGroup(Group group)
        {
            var higherPriority = group.Operation.priority >= tokenIterator.CurrentElement.priority ||
                group.tokens.Count is 5 || tokenIterator.CurrentElement.type is TokenType.RBrace;
            var correctOperrads = group.FirstOperand.type is not TokenType.RBrace &&
                group.SecondOperand.type is not TokenType.LBrace;

            return higherPriority && correctOperrads;
        }

        private void ReplaceWithToken(Group group)
        {
            var token = new Token(group.name, TokenType.Group, 0);

            tokenIterator.ChangeCollection(tokens => tokens.RemoveRange(group.beginIndex, group.tokens.Count));
            tokenIterator.ChangeCollection(tokens => tokens.Insert(group.beginIndex, token));
        }
    }
}

