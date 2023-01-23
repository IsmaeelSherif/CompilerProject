using CompilerProject.Scanning;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CompilerProject.Parsing
{
    public partial class Parser
    {

        Node Begin()
        {
            Node root = null;
            if (TokenStream.Count > 0)
            {
                bool checker(TokenType token)
                {
                    if (isDataType(token))
                    {
                        root = Program();
                        return true;
                    }
                    return false;
                }

                tryShouldMatch(new Node("DataType"), checker);
            }

            return root;
        }

        void Function(Node parent)
        {
            if (InputPointer + 1 < TokenStream.Count)
            {
                var token = TokenStream[InputPointer + 1];
                if (token == TokenType.T_Identifier)
                {
                    FunctionsStatements(parent);
                }
            }
        }

        Node Program()
        {
            Node program = new Node("Program");


            void printStartError(TokenType? token)
            {
                string error = "Parsing Error: Expected an Identifier or " + TokenType.T_Main.ToString();
                if(token != null)
                {
                    error += " but found: " + token.ToString();
                }
                error += "\r\n";
                Errors.Add(error);
            }


            if(InputPointer + 1 < TokenStream.Count)
            {
                var token = TokenStream[InputPointer+1];
                if(token == TokenType.T_Main)
                {
                    MainFunction(program);
                }
                else if(token == TokenType.T_Identifier)
                {
                    FunctionsStatements(program);
                    MainFunction(program);
                }
                else
                {
                    printStartError(token);
                }
            }
            else
            {
                printStartError(null);
            }


            MessageBox.Show("Success");

            return program;
        }


        

        void FunctionsStatements(Node parent) //todo recursive
        {
            //Node node = new Node("FunctionsStatements");
            Node node = parent.Name == "Program" ? new Node("FunctionsStatements") : parent;

            void checker(TokenType token)
            {
                if (isDataType(token))
                {
                    FunctionStatement(node);
                    Function(node);
                }
            }

            tryToMatch(checker);

            if(node != parent && node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        

        void FunctionStatement(Node parent)
        {
            Node node = new Node("FunctionStatement");
            FunctionDeclaration(node);
            FunctionBody(node);
            
            if(node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }

            //todo make recursion here
        }

        void FunctionDeclaration(Node parent)
        {
            Node node = new Node("FunctionDeclaration");
            DataType(node);
            match(node, TokenType.T_Identifier);
            match(node, TokenType.T_LeftBracket);
            Parameters(node);
            match(node, TokenType.T_RightBracket);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void FunctionBody(Node parent)
        {
            Node node = new Node("FunctionBody");
            match(node, TokenType.T_LeftBrace);

            void checker(TokenType token)
            {
                if (isStatement(token))
                {
                    Statements(node);
                }
            }

            tryToMatch(checker);
            //Statements(node);


            ReturnStatement(node);
            match(node, TokenType.T_RightBrace);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void MainFunction(Node parent)
        {
            Node node = new Node("MainFunction");

            bool checker(TokenType token)
            {
                DataType(node);
                match(node, TokenType.T_Main);
                match(node, TokenType.T_LeftBracket);
                match(node, TokenType.T_RightBracket);
                FunctionBody(node);
                return true;
            }

            tryShouldMatch(node, checker);


            

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }


        void DataType(Node parent)
        {
            Node node = new Node("DataType");

            bool checker(TokenType token)
            {
                if (isDataType(token))
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

        void ReturnStatement(Node parent)
        {
            Node node = new Node("ReturnStatment");
            match(node, TokenType.T_Return);
            Expression(node);
            match(node, TokenType.T_SemiColon);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void AssignmentStatement(Node parent)
        {
            Node node = new Node("AssignmentStatement");
            match(node, TokenType.T_Identifier);
            match(node, TokenType.T_Assign);
            Expression(node);
            match(node, TokenType.T_SemiColon);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }


        void Expression(Node parent)
        {
            Node node = new Node("Expression");

            bool checker(TokenType token)
            {
                if (token == TokenType.T_StringValue)
                {
                    match(node, TokenType.T_StringValue);
                    return true;
                }
                else if (isEquation(token))
                {
                    Equation(node);
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

        


        void FunctionCall(Node parent)
        {
            Node node = new Node("FunctionCall");
            match(node, TokenType.T_Identifier);
            match(node, TokenType.T_LeftBracket);
            Identifiers(node);
            match(node, TokenType.T_RightBracket);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }


        void Parameters(Node parent)
        {
            Node node = new Node("Parameters");

            void checker(TokenType token)
            {
                if (isDataType(token))
                {
                    Parameter(node);
                    Parameters2(node);
                }
            }

            tryToMatch(checker);


            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void Parameters2(Node parent) //tODO recursive
        {
            Node node = new Node("Parameters2");

            void checker(TokenType token)
            {
                if (token == TokenType.T_Comma)
                {
                    Node nextNode = parent.Name == "Parameters" ? node : parent;
                    match(nextNode, TokenType.T_Comma);
                    Parameter(nextNode);
                    Parameters2(nextNode);
                }
            }

            tryToMatch(checker);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void Parameter(Node parent)
        {
            Node node = new Node("Parameter");
            DataType(node);
            match(node, TokenType.T_Identifier);
            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void Identifiers(Node parent)
        {
            Node node = new Node("Identifiers");

            match(node, TokenType.T_Identifier);

            void checker(TokenType token)
            {
                if (token == TokenType.T_Comma)
                {
                    Identifiers2(node);
                }
            }

            tryToMatch(checker);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void Identifiers2(Node parent)
        {
            Node node = parent.Name == "Identifiers" ? parent : new Node("Identifiers");

            void checker(TokenType token)
            {
                if (token == TokenType.T_Comma)
                {
                    match(node, TokenType.T_Comma);
                    match(node, TokenType.T_Identifier);
                    Identifiers2(node);
                }
            }

            tryToMatch(checker);

        }



        void DeclarationStatement(Node parent)
        {
            Node node = new Node("DeclarationStatement");
            DataType(node);
            match(node, TokenType.T_Identifier);
            Decl2(node);
            match(node, TokenType.T_SemiColon);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void Decl2(Node parent)
        {
            Node node = parent.Name == "Decl2" ? parent : new Node("Decl2");

            void checker(TokenType token)
            {
                if (token == TokenType.T_Comma)
                {
                    match(node, TokenType.T_Comma);
                    match(node, TokenType.T_Identifier);
                    Decl2(node);
                }
                else if (token == TokenType.T_Assign)
                {
                    AssignStatement2(node);
                    Decl2(node);
                }
            }

            tryToMatch(checker);

            if (parent.Name != "Decl2" && node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void AssignStatement2(Node parent) //TODO
        {
            Node node = new Node("AssignStatement2");

            bool checker(TokenType token)
            {
                if (token == TokenType.T_Assign)
                {
                    match(node, TokenType.T_Assign);
                    Expression(node);
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

        void ReadStatement(Node parent)
        {
            Node node = new Node("ReadStatement");
            match(node, TokenType.T_Read);
            match(node, TokenType.T_Identifier);
            match(node, TokenType.T_SemiColon);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void WriteStatement(Node parent)
        {
            Node node = new Node("WriteStatement");
            match(node, TokenType.T_Write);

            bool checker(TokenType token)
            {
                if (token == TokenType.T_End_L)
                {
                    match(node, TokenType.T_End_L);
                }
                else
                {
                    Expression(node);
                }
                return true;
            }

            tryShouldMatch(node, checker);

            match(node, TokenType.T_SemiColon);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void RepeatStatement(Node parent)
        {
            Node node = new Node("RepeatStatement");
            match(node, TokenType.T_Repeat);
            Statements(node);
            match(node, TokenType.T_Until);
            ConditionStatement(node);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void ConditionOperator(Node parent)
        {
            Node node = new Node("ConditionOperator");

            bool checker(TokenType token)
            {
                if (token == TokenType.T_LessThan || token == TokenType.T_GreaterThan || token == TokenType.T_Equal || token == TokenType.T_NotEqual)
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

        void BooleanOperator(Node parent)
        {
            Node node = new Node("BooleanOperator");

            bool checker(TokenType token)
            {
                if (token == TokenType.T_Or || token == TokenType.T_And)
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

        void Condition(Node parent)
        {
            Node node = new Node("Condition");
            match(node, TokenType.T_Identifier);
            ConditionOperator(node);
            Term(node);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void ConditionStatement(Node parent)
        {
            Node node = new Node("ConditionStatement");
            Condition(node);

            void checker(TokenType token)
            {
                if (token == TokenType.T_Or || token == TokenType.T_And)
                {
                    BooleanOperator(node);
                    ConditionStatement(node);
                }
            }

            tryToMatch(checker);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }


        void IfStatement(Node parent)
        {
            Node node = new Node("IfStatement");
            match(node, TokenType.T_If);
            ConditionStatement(node);
            match(node, TokenType.T_Then);
            Statements(node);
            ETC(node);

            bool checker(TokenType token)
            {
                if(token == TokenType.T_End)
                {
                    match(node, TokenType.T_End);
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

        void ETC(Node parent)
        {
            Node node = new Node("ETC");

            void checker(TokenType token)
            {
                if (token == TokenType.T_Elseif)
                {
                    ElseIfStatement(node);
                }
                else if (token == TokenType.T_Else)
                {
                    ElseStatement(node);
                }
            }

            tryToMatch(checker);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }


        void ElseIfStatement(Node parent)
        {
            Node node = new Node("ElseIfStatement");
            match(node, TokenType.T_Elseif);
            ConditionStatement(node);
            match(node, TokenType.T_Then);
            Statements(node);
            ETC(node);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void ElseStatement(Node parent)
        {
            Node node = new Node("ElseStatement");
            match(node, TokenType.T_Else);
            Statements(node);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }


        void Statements(Node parent)
        {
            Node node = new Node("Statements");
            Statement(node);
            Statements2(node);

            if (node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }

        void Statements2(Node parent)
        {
            Node node = parent.Name == "Statements2" ? parent : new Node("Statements2");

            void checker(TokenType token)
            {
                if (isStatement(token))
                {
                    Statement(node);
                    Statements2(node);
                }
            }

            tryToMatch(checker);

            if (parent.Name != "Statements2" && node.Children.Count > 0)
            {
                parent.Children.Add(node);
            }
        }


        void Statement(Node parent)
        {
            Node node = new Node("Statement");

            bool checker(TokenType token)
            {
                if (isDataType(token))
                {
                    DeclarationStatement(node);
                    return true;
                }
                else if (token == TokenType.T_Read)
                {
                    ReadStatement(node);
                    return true;
                }
                else if (token == TokenType.T_Write)
                {
                    WriteStatement(node);
                    return true;
                }
                else if (token == TokenType.T_If)
                {
                    IfStatement(node);
                    return true;
                }
                else if (token == TokenType.T_Repeat)
                {
                    RepeatStatement(node);
                    return true;
                }
                else if (token == TokenType.T_Identifier)
                {
                    AssignmentStatement(node);
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
