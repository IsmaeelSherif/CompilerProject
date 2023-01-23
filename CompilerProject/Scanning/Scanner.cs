
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CompilerProject.Scanning
{

    class Scanner
    {
        public readonly List<LexemeToken> Tokens = new List<LexemeToken>();
        public readonly List<string> ErrorsList = new List<string>();
        static readonly Dictionary<char, TokenType> Operators = new Dictionary<char, TokenType>()
        {
            {'+', TokenType.T_Add },
            {'-', TokenType.T_Sub },
            {'*', TokenType.T_Multiply },
            {'/', TokenType.T_Divide },
            {'=', TokenType.T_Equal },
            {'<', TokenType.T_LessThan },
            {'>', TokenType.T_GreaterThan },
            {';', TokenType.T_SemiColon },
            {',', TokenType.T_Comma },
            {'(', TokenType.T_LeftBracket },
            {')', TokenType.T_RightBracket },
            {'{', TokenType.T_LeftBrace },
            {'}', TokenType.T_RightBrace },
        };
        static readonly Dictionary<string, TokenType> TwoCharOperators = new Dictionary<string, TokenType>() {
            { ":=", TokenType.T_Assign },
            { "<>", TokenType.T_NotEqual },
            { "&&", TokenType.T_And },
            { "||", TokenType.T_Or }
        };
        static readonly Dictionary<string, TokenType> ReservedWords = new Dictionary<string, TokenType>() {
            { "main", TokenType.T_Main } ,
            { "int", TokenType.T_Int } ,
            { "float", TokenType.T_Float },
            { "string", TokenType.T_String },
            { "read", TokenType.T_Read },
            { "write", TokenType.T_Write },
            { "repeat", TokenType.T_Repeat },
            { "until", TokenType.T_Until },
            { "if", TokenType.T_If },
            { "elseif", TokenType.T_Elseif },
            { "else", TokenType.T_Else },
            { "then", TokenType.T_Then },
            { "return", TokenType.T_Return },
            { "end", TokenType.T_End },
            { "endl", TokenType.T_End_L }
        };

        public void StartScanning(string sourceCode)
        {

            Tokens.Clear();
            ErrorsList.Clear();

            for (int i = 0; i < sourceCode.Length; i++)
            {
                char currentChar = sourceCode[i];
                string currentLexeme = currentChar.ToString();

                //these are 2 local fucntions to the function, they are declared here to be in the scope of the loop variables
                bool isLexeme(string operat0r)
                {
                    if (sourceCode[i] == operat0r[0] && i + 1 < sourceCode.Length && sourceCode[i + 1] == operat0r[1])
                    {
                        return true;
                    }
                    return false;
                }

                bool isTwoCharOp()
                {
                    if(i+1 < sourceCode.Length && TwoCharOperators.ContainsKey($"{currentChar}{sourceCode[i + 1]}"))
                    {
                        currentLexeme += sourceCode[i+1];
                        return true;
                    }
                    return false;
                }



                if (currentChar == ' ' || currentChar == '\r' || currentChar == '\n')
                    continue;

                if (IsCharacter(currentChar))
                {
                    CheckReservedAndIdentifiers(sourceCode, i, out int continueFromIndex);
                    i = continueFromIndex;
                }

                else if (IsDigit(currentChar))
                {
                    CheckConstants(sourceCode, i, out int continueFromIndex);
                    i = continueFromIndex;
                }

                else if (currentChar == '"')
                {
                    bool isStringClosed = false;
                    for (i++; i < sourceCode.Length; i++)
                    {
                        currentLexeme += sourceCode[i];

                        if (sourceCode[i] == '"')
                        {
                            isStringClosed = true;
                            break;
                        }
                    }

                    if (!isStringClosed)
                    {
                        ErrorsList.Add("Scanning Error: Invalid String Closing");
                    }
                    else
                    {
                        Tokens.Add(new LexemeToken(currentLexeme, TokenType.T_StringValue));
                    }
                }

                else if (isTwoCharOp())
                {
                    i++;
                    Tokens.Add(new LexemeToken(currentLexeme, TwoCharOperators[currentLexeme]));
                }

                else if (isLexeme("/*"))
                {
                    bool isCommentClosed = false;
                    for (i += 2; i < sourceCode.Length; i++)
                    {
                        if (isLexeme("*/"))
                        {
                            i++;
                            isCommentClosed = true;
                            break;
                        }
                    }

                    if (!isCommentClosed)
                    {
                        ErrorsList.Add("Scanning Error: Invalid Comment Closing");
                    }
                }

                else if (Operators.ContainsKey(currentChar))
                {
                    Tokens.Add(new LexemeToken(currentLexeme, Operators[currentChar]));
                }

                else
                {
                    ErrorsList.Add("Scanning Error: unidentified lexeme \"" + currentLexeme + "\"");
                }
            }

        }


        private void CheckReservedAndIdentifiers(string sourceCode, int startFromIndex, out int continueFromIndex)
        {
            string currentLexeme = sourceCode[startFromIndex].ToString();
            int i;
            for (i = startFromIndex + 1; i < sourceCode.Length; i++)
            {
                char currentChar = sourceCode[i];
                if (IsCharacter(currentChar) || IsDigit(currentChar))
                {
                    currentLexeme += currentChar;
                }
                else
                {
                    i--;
                    break;
                }
            }

            TokenType token;
            if (ReservedWords.ContainsKey(currentLexeme))
            {
                token = ReservedWords[currentLexeme];
            }
            else
            {
                token = TokenType.T_Identifier;
            }
            Tokens.Add(new LexemeToken(currentLexeme, token));
            continueFromIndex = i;

        }

        private void CheckConstants(string sourceCode, int startFromIndex, out int continueFromIndex)
        {
            string currentLexeme = sourceCode[startFromIndex].ToString();
            int i;
            for (i = startFromIndex + 1; i < sourceCode.Length; i++)
            {
                char currentChar = sourceCode[i];
                if (IsCharacter(currentChar) || IsDigit(currentChar) || currentChar == '.')
                {
                    currentLexeme += currentChar;
                }
                else
                {
                    i--;
                    break;
                }
            }

            var constantRegex = new Regex("^[0-9]+([/.][0-9]+)?$");
            if (constantRegex.IsMatch(currentLexeme))
            {
                Tokens.Add(new LexemeToken(currentLexeme, TokenType.T_Constant));
            }
            else
            {
                ErrorsList.Add("Scanning Error: unidentified lexeme \"" + currentLexeme + "\"");
            }

            continueFromIndex = i;
        }




        private static bool IsCharacter(char c)
        {
            return c >= 'A' && c <= 'z';
        }

        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }


    }
}
