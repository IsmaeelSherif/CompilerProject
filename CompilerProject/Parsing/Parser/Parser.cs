using CompilerProject.Scanning;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CompilerProject.Parsing
{
    public partial class Parser
    {
        int InputPointer = 0;
        List<TokenType> TokenStream;
        public List<string> Errors = new List<string>();
        public Node root;

        public Node StartParsing(List<TokenType> tokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = tokenStream;
            return Begin();
        }


        static bool isDataType(TokenType token)
        {
            return token == TokenType.T_Int || token == TokenType.T_String || token == TokenType.T_Float;
        }

        static bool isNumber(TokenType token)
        {
            return token == TokenType.T_Constant || token == TokenType.T_Float || token == TokenType.T_Int;
        }

        static bool isEquation(TokenType token)
        {
            return token == TokenType.T_LeftBracket || token == TokenType.T_Identifier || isNumber(token);
        }

        static bool isStatement(TokenType token)
        {
            TokenType[] validTokensArr = {
                    TokenType.T_Int, TokenType.T_String, TokenType.T_Float,
                    TokenType.T_Read, TokenType.T_Write, TokenType.T_Repeat,
                    TokenType.T_If,
                    TokenType.T_Identifier,
                };
            var validTokens = new List<TokenType>(validTokensArr);

            return validTokens.Contains(token);
        }


        void tryShouldMatch(Node node, Func<TokenType, bool> checker)
        {
            if (InputPointer < TokenStream.Count)
            {
                var token = TokenStream[InputPointer];
                bool matched = checker(token);
                if (!matched)
                {
                    Errors.Add("Parsing Error: Expected a " + node.Name + " and found: " + TokenStream[InputPointer].ToString() + "\r\n");
                    InputPointer++;
                }
            }
            else
            {
                Errors.Add("Parsing Error: Expected a " + node.Name + "\r\n");
            }
        }

        void tryToMatch(Action<TokenType> checker)
        {
            if (InputPointer < TokenStream.Count)
            {
                var token = TokenStream[InputPointer];
                checker(token);
            }
        }


        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }

        public void match(Node parent, TokenType ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer])
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());
                    parent.Children.Add(newNode);

                }

                else
                {
                    Errors.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + ", but " +
                        TokenStream[InputPointer].ToString() +
                        "  found\r\n");
                    InputPointer++;
                }
            }
            else
            {
                Errors.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
            }
        }
    }
}
