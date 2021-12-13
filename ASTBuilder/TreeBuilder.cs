using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace ASTBuilder
{
    public class Node
    {
        public Node(Token token)
        {
            Token = token;
        }

        public readonly Token Token;
        public Node LeftChild { get; set; }
        public Node RightChild { get; set; }
    }

    public class TreeBuilder
    {
        public TreeBuilder(List<Group> groups)
        {
            this.groups = groups;
        }

        private readonly List<Group> groups;

        public Node Build()
        {
            var head = groups.Last();

            var root = new Node(head.Operation)
            {
                LeftChild = CreateNodeFrom(head.FirstOperand),
                RightChild = CreateNodeFrom(head.SecondOperand)
            };

            return root;
        }

        private Node CreateNodeFrom(Token token) => token.type is TokenType.Group ?
            CreateNodeFromGroupBy(token) : new Node(token);

        private Node CreateNodeFromGroupBy(Token token)
        {
            var tokenGroup = groups.Find(group => group.name == token.value);

            var node = new Node(tokenGroup.Operation)
            {
                LeftChild = CreateNodeFrom(tokenGroup.FirstOperand),
                RightChild = CreateNodeFrom(tokenGroup.SecondOperand)
            };

            return node;
        }
    }
}
