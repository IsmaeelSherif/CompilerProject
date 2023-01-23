using System;
using System.Collections.Generic;
using System.Text;

namespace CompilerProject.Scanning
{
    public enum TokenType
    {
        T_Main, T_Int, T_Float, T_String, T_Read, T_Write, T_Repeat, T_Until, T_If, T_Elseif, T_Else, T_Then, T_Return, T_End, T_End_L, //reserved words
        T_Constant, T_StringValue, T_Identifier,
        T_Sub, T_Add, T_Multiply, T_Divide, T_LessThan, T_GreaterThan, T_Equal, T_NotEqual, T_And, T_Or,
        T_Assign,
        T_Comma, T_SemiColon, T_LeftBracket, T_RightBracket, T_LeftBrace, T_RightBrace
    }
}
