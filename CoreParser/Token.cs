using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreParser
{
    public class Token
    {
        public string Content { get; set; }
        public TokenType Type { get; set; }

        public Token(string content, TokenType type)
        {
            this.Content = content;
            this.Type = type;
        }
    }
}
