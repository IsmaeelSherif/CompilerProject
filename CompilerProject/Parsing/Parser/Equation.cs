using CompilerProject.Scanning;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompilerProject.Parsing
{
    public partial class Parser
    {
        void Equation(Node parent)
        {
            Node node = new Node("Equation");

            bool checker(TokenType token)
            {
                if (isEquation(token))
                {
                    Term2(node);
                    return true;
                }
                return false;
            }

            tryShouldMatch(node, checker);

            SubEquation(node);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void Brackets(Node parent)
        {
            Node node = new Node("Brackets");

            match(node, TokenType.T_LeftBracket);

            Equation(node);

            match(node, TokenType.T_RightBracket);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void SubEquation(Node parent)
        {
            Node node = new Node("SubEquation");

            void checker(TokenType token)
            {
                if (token == TokenType.T_Add || token == TokenType.T_Sub || token == TokenType.T_Multiply || token == TokenType.T_Divide)
                {
                    ArithOperator(node);
                    Equation(node);
                }
            }

            tryToMatch(checker);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void Term2(Node parent)
        {
            Node node = new Node("Term2");

            bool checker(TokenType token)
            {
                if (token == TokenType.T_LeftBracket)
                {
                    Brackets(node);
                }
                else
                {
                    Term(node);
                }
                return true;
            }

            tryShouldMatch(node, checker);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void Term(Node parent)
        {
            Node term = new Node("Term");

            bool checker(TokenType token)
            {
                if (token == TokenType.T_Identifier)
                {
                    if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1] == TokenType.T_LeftBracket)
                    {
                        FunctionCall(term);
                    }
                    else
                    {
                        match(term, TokenType.T_Identifier);
                    }

                    return true;
                }
                else if (isNumber(token))
                {
                    Number(term);
                    return true;
                }
                return false;
            }

            tryShouldMatch(term, checker);



            if (term.Children.Count > 0)
            {
                parent.Children.Add(term);
            }
        }

        void Number(Node parent)
        {
            Node node = new Node("Number");

            bool checker(TokenType token)
            {
                if (token == TokenType.T_Constant)
                {
                    match(node, TokenType.T_Constant);
                    return true;
                }
                return false;
            }

            tryShouldMatch(node, checker);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void ArithOperator(Node parent)
        {
            Node node = new Node("ArithOperator");

            bool checker(TokenType token)
            {
                if (token == TokenType.T_Add || token == TokenType.T_Sub || token == TokenType.T_Multiply || token == TokenType.T_Divide)
                {
                    match(node, token);
                    return true;
                }
                return false;
            }

            tryShouldMatch(node, checker);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

    }
}
