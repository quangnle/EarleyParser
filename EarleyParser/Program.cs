using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreParser;

namespace EarleyParser
{
    class Program
    {
        static List<Symbol> _terminalList = new List<Symbol>();
        static List<Symbol> _nonterminalList = new List<Symbol>();
        static List<Symbol> _startList = new List<Symbol>();
        static Parser _parser = new Parser();

        static void Main(string[] args)
        {
            string grammar = "n kw op st ob cb gt lt dot comma brkt" + Environment.NewLine +
                            "S EXP EXPS CMD KW OB CB OP DOT COMMA" + Environment.NewLine +
                            "S EXP" + Environment.NewLine +
                            "S CMD" + Environment.NewLine +
                            "EXP CMD" + Environment.NewLine +
                            "EXP EXP OP EXP" + Environment.NewLine +
                            "EXP OB EXP CB" + Environment.NewLine +
                            "CMD KW OB CB" + Environment.NewLine +
                            "CMD KW DOT CMD" + Environment.NewLine +
                            "CMD CMD DOT CMD" + Environment.NewLine +
                            "CMD KW OB EXP CB" + Environment.NewLine +
                            "CMD KW OB EXPS CB" + Environment.NewLine +
                            "EXPS EXP" + Environment.NewLine +
                            "EXPS EXPS COMMA EXP" + Environment.NewLine +
                            "EXP n" + Environment.NewLine +
                            "EXP st" + Environment.NewLine +
                            "KW kw" + Environment.NewLine +
                            "OB ob" + Environment.NewLine +
                            "CB cb" + Environment.NewLine +
                            "DOT dot" + Environment.NewLine +
                            "COMMA comma" + Environment.NewLine +
                            "OP op";

            Tokenizer tokenizer = new Tokenizer();            
            tokenizer.Keywords = new List<string>() { "Sum", "Convert", "ToInt32", "ToDouble" };


            var input = "Sum(7 + -2)";
            var tokens = tokenizer.Tokenize(input);

            List<string> lstInput = MapToTerminal(tokens);

            var i = 0;
            foreach (var token in tokens)
            {
                Console.WriteLine(token.Content + "\t\t(" + token.Type.ToString() + ")" + "\t" + lstInput[i]);
                i++;
            }

            _parser.LoadGrammar(grammar);
            var tree = _parser.Parse(lstInput);

            Console.ReadLine();
        }

        private static List<string> MapToTerminal(List<Token> tokens)
        {
            var lstInput = new List<string>();
            foreach (var item in tokens)
            {
                if (item.Type == TokenType.Dot)
                    lstInput.Add("dot");
                else if (item.Type == TokenType.Comma)
                    lstInput.Add("comma");
                else if (item.Type == TokenType.Bracket)
                {
                    if (item.Content == "(")
                        lstInput.Add("ob");
                    else if (item.Content == ")")
                        lstInput.Add("cb");
                }
                else if (item.Type == TokenType.Number)
                {
                    lstInput.Add("n");
                }
                else if (item.Type == TokenType.Keyword)
                {
                    lstInput.Add("kw");
                }
                else if (item.Type == TokenType.String)
                {
                    lstInput.Add("st");
                }
                else if (item.Type == TokenType.Operation)
                {
                    lstInput.Add("op");
                }
                else if (item.Type == TokenType.GreaterThan)
                {
                    lstInput.Add("gt");
                }
                else if (item.Type == TokenType.LessThan)
                {
                    lstInput.Add("lt");
                }
            }

            return lstInput;
        }

    }
}
