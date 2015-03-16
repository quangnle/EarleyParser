using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreParser
{
    public class Tokenizer
    {        
        public List<string> Keywords { get; set; }

        public Tokenizer()
        {   
        }

        private List<string> Preprocess(string input)
        {
            var lstTokens = new List<string>();

            var currentToken = "";

            for (var i = 0; i < input.Length; i++)
            {   
                if (input[i] == ' ')
                {
                    SpaceProcessing(input, i, lstTokens, ref currentToken);
                }
                else if (input[i] == '.')
                {
                    DotProcessing(input, i, lstTokens, ref currentToken);
                }
                else if (input[i] == ',')
                {
                    CommaProcessing(input, i, lstTokens, ref currentToken);
                }
                else if (input[i] == '>' || input[i] == '<' || input[i] == '=')
                {
                    ComparatorProcessing(input, i, lstTokens, ref currentToken);
                }
                else if (input[i] == '"')
                {
                    QuoteProcessing(input, i, lstTokens, ref currentToken);
                }
                else if (input[i] == '(' || input[i] == ')' || input[i] == '{' || input[i] == '}' || input[i] == '[' || input[i] == ']')
                {
                    BracketProcessing(input, i, lstTokens, ref currentToken);
                }
                else if (input[i] == '+' || input[i] == '*' || input[i] == '/' || input[i] == '-')
                {
                    OperatorProcessing(input, i, lstTokens, ref currentToken);
                }
                else
                {
                    currentToken = currentToken + input[i];
                }
            }

            if (currentToken != "")
                lstTokens.Add(currentToken);

            return lstTokens;
        }

        public List<Token> Tokenize(string input)
        {
            var tokens = Preprocess(input);
            var lstTokens = new List<Token>();
            Token newToken = null;

            var counter = 0;
            while (counter < tokens.Count)
            {
                if (tokens[counter] == "-")
                {
                    if (counter == 0)
                    {
                        if (counter + 1 < tokens.Count && IsNumber(tokens[counter + 1]))
                        {
                            newToken = new Token(tokens[counter] + tokens[counter + 1], TokenType.Number);
                            lstTokens.Add(newToken);
                            counter++;
                        }
                        else
                            throw new Exception("Wrong input format.");
                    }
                    else
                    {
                        if (counter + 1 >= tokens.Count)
                            throw new Exception("Wrong input format.");

                        if (tokens[counter - 1] == "," &&
                            (IsNumber(tokens[counter + 1]) || IsKeyword(tokens[counter + 1]) || tokens[counter + 1] == "("))
                        {
                            newToken = new Token(tokens[counter] + tokens[counter + 1], TokenType.Number);
                            lstTokens.Add(newToken);
                            counter++;
                        }
                        else if (tokens[counter - 1] == ")")
                        {
                            newToken = new Token(tokens[counter], TokenType.Operation);
                            lstTokens.Add(newToken);
                        }
                        else if (IsOperation(tokens[counter - 1]) && !IsOperation(tokens[counter + 1]))
                        {
                            if (IsNumber(tokens[counter + 1]))
                            {
                                newToken = new Token(tokens[counter] + tokens[counter + 1], TokenType.Number);
                                lstTokens.Add(newToken);
                                counter++;
                            }
                            else if (tokens[counter + 1] == "(")
                            {
                                newToken = new Token("-1", TokenType.Number);
                                lstTokens.Add(newToken);
                                newToken = new Token("*", TokenType.Operation);
                                lstTokens.Add(newToken);
                            }
                            else if (IsKeyword(tokens[counter + 1]))
                            {
                                newToken = new Token("-1", TokenType.Number);
                                lstTokens.Add(newToken);
                                newToken = new Token("*", TokenType.Operation);
                                lstTokens.Add(newToken);
                            }
                        }
                        else if (tokens[counter - 1] == ")" || IsNumber(tokens[counter - 1]) ||
                            IsKeyword(tokens[counter - 1]))
                        {
                            lstTokens.Add(new Token(tokens[counter], TokenType.Operation));
                        }
                        else if (tokens[counter - 1] == "(")
                        {
                            if (IsNumber(tokens[counter + 1]))
                            {
                                newToken = new Token(tokens[counter] + tokens[counter + 1], TokenType.Number);
                                lstTokens.Add(newToken);
                                counter++;
                            }
                            else
                            {
                                newToken = new Token("-1", TokenType.Number);
                                lstTokens.Add(newToken);
                                newToken = new Token("*", TokenType.Operation);
                                lstTokens.Add(newToken);
                            }
                        }
                    }
                }
                else
                {
                    if (IsOperation(tokens[counter]))
                        lstTokens.Add(new Token(tokens[counter], TokenType.Operation));
                    else if (IsBracket(tokens[counter]))
                        lstTokens.Add(new Token(tokens[counter], TokenType.Bracket));
                    else if (IsNumber(tokens[counter]))
                        lstTokens.Add(new Token(tokens[counter], TokenType.Number));
                    else if (IsKeyword(tokens[counter]))
                        lstTokens.Add(new Token(tokens[counter], TokenType.Keyword));
                    else if (tokens[counter] == ".")
                        lstTokens.Add(new Token(tokens[counter], TokenType.Dot));
                    else if (tokens[counter] == ",")
                        lstTokens.Add(new Token(tokens[counter], TokenType.Comma));
                    else if (tokens[counter] == "<")
                        lstTokens.Add(new Token(tokens[counter], TokenType.LessThan));
                    else if (tokens[counter] == ">")
                        lstTokens.Add(new Token(tokens[counter], TokenType.GreaterThan));
                    else if (IsString(tokens[counter]))
                        lstTokens.Add(new Token(tokens[counter], TokenType.String));
                }

                counter++;
            }

            return lstTokens;
        }

        private void SpaceProcessing(string input, int currentPosition, List<string> lstTokens, ref string currentToken)
        {
            if (currentToken != "")
            {
                lstTokens.Add(currentToken);
                currentToken = "";
            }
        }

        private void DotProcessing(string input, int currentPosition, List<string> lstTokens, ref string currentToken)
        {
            if (IsNumber(currentToken))
            {
                if (IsInteger(currentToken))
                    currentToken = currentToken + input[currentPosition];
                else
                    throw new Exception(String.Format("Wrong input format at position {0}", currentPosition));
            }
            else
            {
                if (currentToken != "")
                {
                    if (IsNumeric(currentToken[0]))
                        throw new Exception(String.Format("Wrong input format at position {0}", currentPosition));
                    else if (IsCharacter(currentToken[0]) || currentToken[0] == '_' || currentToken == ")")
                    {
                        lstTokens.Add(currentToken);
                        lstTokens.Add(".");
                        currentToken = "";
                    }
                    else
                        throw new Exception(String.Format("Wrong input format at position {0}", currentPosition));
                }
                else
                {
                    if (lstTokens[lstTokens.Count - 1] == ")")
                        lstTokens.Add(".");
                    else
                        throw new Exception(String.Format("Wrong input format at position {0}", currentPosition));
                }
            }
        }

        private void CommaProcessing(string input, int currentPosition, List<string> lstTokens, ref string currentToken)
        {
            if (currentToken != "")
            {
                if (currentToken.Contains(','))
                    throw new Exception(String.Format("Wrong input format at position {0}", currentPosition));
                else
                {
                    lstTokens.Add(currentToken);
                    lstTokens.Add(",");

                    currentToken = "";
                }
            }
            else
            {
                if (lstTokens[lstTokens.Count - 1] != ",")
                {
                    if (!String.IsNullOrWhiteSpace(currentToken))
                        lstTokens.Add(currentToken);

                    lstTokens.Add(",");

                    currentToken = "";
                }
                else
                    throw new Exception(String.Format("Wrong input format at position {0}", currentPosition));
            }
        }

        private void ComparatorProcessing(string input, int currentPosition, List<string> lstTokens, ref string currentToken)
        {
            if (currentPosition == input.Length - 1)
                throw new Exception("Input is still incompleted");
            else
            {
                if (input[currentPosition + 1] == '=')
                {
                    currentToken = currentToken + input[currentPosition];
                }
                else
                {
                    if (IsCharacter(input[currentPosition + 1]) || IsNumeric(input[currentPosition + 1])
                        || input[currentPosition + 1] == '_' || input[currentPosition + 1] == ' ' || input[currentPosition + 1] == '(')
                    {
                        if (!String.IsNullOrWhiteSpace(currentToken))
                            lstTokens.Add(currentToken);

                        lstTokens.Add(input[currentPosition].ToString());
                        currentToken = "";
                    }
                    else
                        throw new Exception(String.Format("Wrong input format at position {0}", currentPosition));
                }
            }
        }

        private void QuoteProcessing(string input, int currentPosition, List<string> lstTokens, ref string currentToken)
        {
            currentToken = currentToken + input[currentPosition];
            if (currentToken != "\"")
            {
                lstTokens.Add(currentToken);
                currentToken = "";
            }
        }

        private void BracketProcessing(string input, int currentPosition, List<string> lstTokens, ref string currentToken)
        {
            if (!String.IsNullOrWhiteSpace(currentToken))
                lstTokens.Add(currentToken);

            lstTokens.Add(input[currentPosition].ToString());
            currentToken = "";
        }

        private void OperatorProcessing(string input, int currentPosition, List<string> lstTokens, ref string currentToken)
        {
            if (currentToken != "")
                lstTokens.Add(currentToken);

            lstTokens.Add(input[currentPosition].ToString());
            currentToken = "";
        }

        private bool IsKeyword(string input)
        {
            return this.Keywords.Any(kw => kw == input);
        }
        
        private bool IsCharacter(char ch)
        {            
            return 'A' <= char.ToUpper(ch) && char.ToUpper(ch) <= 'Z';
        }

        private bool IsNumeric(char ch)
        {
            return '0' <= ch && ch <= '9';
        }

        private bool IsInteger(string st)
        {
            if (String.IsNullOrWhiteSpace(st))
                return false;

            for (var i = 0; i < st.Length; i++)
            {
                if (!IsNumeric(st[i]))
                    return false;
            }

            return true;
        }

        private bool IsFloat(string st)
        {
            if (String.IsNullOrWhiteSpace(st)) return false;

            var parts = st.Split(new char[]{'.'});
            if (parts.Length <= 2 && parts.Length >= 1)
            {
                for (int i = 0; i < parts.Length; i++)
                {
                    if (!IsInteger(parts[i])) return false;
                }

                return true;
            }

            return false;
        }

        private bool IsNumber(string input)
        {
            return IsInteger(input) || IsFloat(input);
        }

        private bool IsOperation(string input)
        {
            return input == "*" || input == "/" || input == "+" || input == "-";
        }

        private bool IsBracket(string input)
        {
            return "(){}[]".Contains(input);
        }

        private bool IsString(string input)
        {
            return input[0] == '"' && input[input.Length - 1] == '"';
        }
    }
}
