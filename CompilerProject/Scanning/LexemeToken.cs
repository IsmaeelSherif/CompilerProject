using System;
using System.Collections.Generic;
using System.Text;

namespace CompilerProject.Scanning
{
    class LexemeToken
    {
        public string lex;
        public TokenType token_type;

        public LexemeToken(string lex)
        {
            this.lex = lex;
        }
        public LexemeToken(string lex, TokenType token_type)
        {
            this.lex = lex;
            this.token_type = token_type;
        }
    }
}
