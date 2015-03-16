using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreParser
{
    public enum TokenType
    {
        Keyword,
        Function,
        Number,
        String,
        Operation,
        GreaterThan,
        LessThan,
        Comparison,
        Bracket,
        Dot,
        Comma,
        Unknown
    }
}
