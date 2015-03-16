using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using CoreParser;

namespace EarleyUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Parser _parser = new Parser();
        private Tokenizer _tokenizer = new Tokenizer();
        private List<Token> _tokens = null;
        private int _tokenCounter = 0;
        private TreeViewItem _treeNode = null;
        
        public MainWindow()
        {
            InitializeComponent();
            BuildGrammar();
        }

        private void BuildGrammar()
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
            
            _tokenizer.Keywords = new List<string>() { "Sum", "Convert", "ToInt32", "ToDouble" };
            _parser.LoadGrammar(grammar);

            txtInput.Text = "Sum(-7*-2) + Sum(7 + -2, -4.5 + 11.66, Sum(Convert.ToInt32(\"7\").ToDouble(), -15.1))";
            //txtInput.Text = "Sum(7 + -2, -4.5 + 11.66, Sum(Convert.ToInt32(\"7\").ToDouble(), -15.1))";
            //txtInput.Text = "Sum(7 + -2)";
            
        }

        private List<string> MapToTerminal(List<Token> tokens)
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

        private void btnParse_Click(object sender, RoutedEventArgs e)
        {
            var input = txtInput.Text;
            _tokens = _tokenizer.Tokenize(input);

            var lstInput = MapToTerminal(_tokens);
            var tree = _parser.Parse(lstInput);

            _treeNode = new TreeViewItem();
            _tokenCounter = 0;

            BuildTreeView(tree, _treeNode);

            trvParse.Items.Clear();
            trvParse.Items.Add(_treeNode);
        }

        
        
        private void BuildTreeView(TNode<State> node, TreeViewItem treeNode)
        {
            if (node.Content.Rule.RHS[node.Content.DotPosition - 1].Type == SymbolType.Terminal)
            {
                treeNode.Header = node.Content.Rule.RHS[node.Content.DotPosition - 1].Name; // _tokens[_tokenCounter].Content;
                treeNode.Items.Add(new TreeViewItem() 
                {
                    Header = _tokens[_tokenCounter].Content
                });
                
                _tokenCounter++;
                
                return;
            }
            else
            {
                treeNode.Header = node.Content.Rule.RHS[node.Content.DotPosition - 1].Name;
            }
            if (node.Children != null && node.Children.Count > 0)
            {
                foreach (var child in node.Children)
                {
                    var newTreeNode = new TreeViewItem();
                    treeNode.Items.Add(newTreeNode);
                    BuildTreeView(child, newTreeNode);
                }
            }
        }

        private void btnDrawTree_Click(object sender, RoutedEventArgs e)
        {
            var formChartViewer = new ChartViewer(_treeNode, 1280);
            formChartViewer.Width = 1280;
            formChartViewer.Show();
        }
    }
}
