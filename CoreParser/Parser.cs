using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreParser
{
    public class Parser
    {
        private List<Symbol> _lstSymbols = new List<Symbol>();
        private int _stateIndex = 0;

        private List<Production> _grammar;
        public List<Production> Grammar
        {
            get 
            {
                if (_grammar == null)
                    _grammar = new List<Production>();

                return _grammar; 
            }
            set { _grammar = value; }
        }
        
        private List<ParseSet> _setTable;

        /// <summary>
        /// Parsing using the Earley Algorithm
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public TNode<State> Parse(List<string> tokens)
        {
            _setTable = new List<ParseSet>();

            // look for all start states, push into the table set level 0
            var firstSet = new ParseSet();
            firstSet.Level = 0;

            var startRules = _grammar.Where(p => p.LHS.Type == SymbolType.Start);
            foreach (var rule in startRules)
            {
                var state = new State(GetRulesCount(), rule, 0, 0);

                firstSet.AddState(state);
            }

            _setTable.Add(firstSet);

            // run the Earley Algorithm with Predict, Scan and Complete repeatedly
            for (var i = 0; i <= tokens.Count; i++)
            {
                var counter = 0;

                // get the ith level set
                var chosenSet = _setTable.FirstOrDefault(s => s.Level == i);

                // look through the states set of the chosen Set for each every state
                while (counter < chosenSet.States.Count())
                {
                    // get the next state of the set
                    var state = chosenSet.States[counter];
                    
                    if (state.IsComplete())
                    {
                        Complete(state, i);
                    }
                    else
                    {
                        if (state.NextSymbol() != null)
                        {
                            if (state.NextSymbol().Type == SymbolType.Terminal)
                            {
                                string token = null;
                                if (i < tokens.Count)
                                    token = tokens[i];
                                Scan(state, i, token);
                            }
                            else
                            {
                                Predict(state, i);
                            }
                        }
                    }

                    counter++;
                }

                Console.WriteLine(chosenSet.ToString());
            }

            if (CheckGrammar())
            {
                return BuildTree();
            }

            return null;
        }

        
    

        // Prediction: For every state in S(k) of the form (X → α • Y β, j) (where j is the origin position),
        // add (Y → • γ, k) to S(k) for every production in the grammar with Y on the left-hand side (Y → γ).
        private void Predict(State state, int originPosition)
        {
            var nextSymbol = state.NextSymbol();
            var matchedRules = _grammar.Where(r => r.LHS == nextSymbol);
            
            foreach (var rule in matchedRules)
            {
                var newState = new State(GetRulesCount(), rule, 0, originPosition);
                newState.Status = ParseStatus.Predict;
                
                newState.AddBackPointer(state);

                AddToTableSet(newState, originPosition);
            }
        }

        // Scanning: If a is the next symbol in the input stream, 
        // for every state in S(k) of the form (X → α • a β, j), add (X → α a • β, j) to S(k+1).
        private void Scan(State state, int originPosition, string token)
        {
            var chosenSet = _setTable.FirstOrDefault(s => s.Level == originPosition);
            if (chosenSet != null)
            {
                var matchedStates = chosenSet.States.Where(s => (s.NextSymbol() != null) && (s.NextSymbol().Name == token));
                foreach (var foundState in matchedStates)
                {
                    var newState = new State(GetRulesCount(), foundState.Rule, foundState.DotPosition + 1, foundState.OriginPosition);
                    newState.Status = ParseStatus.Scan;
                    
                    newState.AddBackPointer(state);

                    AddToTableSet(newState, originPosition + 1);
                }
            }
        }

        // For every state in S(k) of the form (X → γ •, j), 
        // find states in S(j) of the form (Y → α • X β, i) and add (Y → α X • β, i) to S(k).
        private void Complete(State state, int originPosition)
        {
            var chosenSet = _setTable.FirstOrDefault(s => s.Level == state.OriginPosition);
            if (chosenSet != null)
            {
                var matchedStates = chosenSet.States.Where(s => s.NextSymbol() == state.Rule.LHS);
                foreach (var foundState in matchedStates)
                {
                    var newState = new State(GetRulesCount(), foundState.Rule, foundState.DotPosition + 1, foundState.OriginPosition);
                    newState.Status = ParseStatus.Complete;
                    
                    newState.AddBackPointer(state);

                    AddToTableSet(newState, originPosition);
                }
            }
        }

        // add a state to the table set at a given level
        // make sure that there will be no duplicated rule added
        private void AddToTableSet(State state, int originPosition)
        {
            var set = _setTable.FirstOrDefault(s => s.Level == originPosition);
            if (set == null)
            {
                set = new ParseSet();
                set.Level = originPosition;
                _setTable.Add(set);
            }

            set.AddState(state);            
        }

        /// <summary>
        /// validate the input string whether it matches with the grammar
        /// </summary>
        /// <returns></returns>
        private bool CheckGrammar()
        {
            var lastSet = _setTable.FirstOrDefault(s => s.Level == _setTable.Count - 1);
            if (lastSet != null)
            {
                foreach (var state in lastSet.States)
                {
                    if (state.Rule.LHS.Type == SymbolType.Start && state.IsComplete())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// refines all Ids of the states in _setTable
        /// </summary>
        private void RefineIds()
        {
            var counter = 0;
            for (var i = 0; i < _setTable.Count; i++)
            {
                var currentTable = _setTable.FirstOrDefault(s => s.Level == i);
                for (int j = 0; j < currentTable.States.Count; j++)
                {
                    currentTable.States[j].Id = counter;
                    counter++;
                }
            }
        }

        /// <summary>
        /// build a grammar tree using the result table set
        /// </summary>
        /// <returns></returns>
        private TNode<State> BuildTree()
        {
            RefineIds();

            var rootRule = _setTable.FirstOrDefault(s => s.Level == _setTable.Count - 1).States
                                    .FirstOrDefault(state => state.Rule.LHS.Type == SymbolType.Start 
                                                            && state.IsComplete());            

            var rootNode = new TNode<State>();
            rootNode.Content = rootRule;

            _stateIndex = rootNode.Content.Id;
            
            AddNode(rootNode);

            return rootNode;
        }



        /// <summary>
        /// recursively adding node from a root node  to build the grammar tree
        /// </summary>
        /// <param name="node"></param>
        /// <param name="setLevel"></param>
        /// <param name="stateIndex"></param>
        private void AddNode(TNode<State> node)
        {
            if (node.Content.Status == ParseStatus.Scan) 
                return;

            if (node.Content.Status == ParseStatus.Complete)
            {
                var newNode = new TNode<State>();
                newNode.Content = node.Content.BackPointers[0];

                if (node.Children == null) node.Children = new List<TNode<State>>();

                newNode.Parent = node;
                node.Children.Insert(0, newNode);
                _stateIndex = node.Content.Id;
                AddNode(newNode);
            }

            if (node.Content.DotPosition > 0)
            {
                State selectedState = null;
                for (var i = _setTable.Count - 1; i >= 0; i--)
                {
                    for (var j = _setTable[i].States.Count - 1; j >= 0 ; j--)
                    {
                        var aState = _setTable[i].States[j];
                        if (aState.Id < _stateIndex &&
                            aState.Rule == node.Content.Rule &&
                            aState.DotPosition == node.Content.DotPosition - 1)
                        {
                            selectedState = aState;
                            break;
                        }

                        if (selectedState != null)
                            break;
                    }
                }

                if (selectedState != null && selectedState.DotPosition != 0)
                {
                    var newNode = new TNode<State>();
                    newNode.Content = selectedState;
                    newNode.Parent = node.Parent;

                    node.Parent.Children.Insert(0, newNode);

                    _stateIndex = selectedState.Id;

                    AddNode(newNode);
                }
            }
        }

        /// <summary>
        /// load grammar from a text file
        /// first line is terminal symbols
        /// second line is nonterminal symbols
        /// from the third to the rest are rules
        /// </summary>
        /// <param name="input"></param>
        public void LoadGrammar(string input)
        {
            var lines = input.Split(new string[]{Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            var terminals = lines[0];
            var nonterminals = lines[1];

            foreach (var terminal in terminals.Split(new char[] { ' ' }))
            {
                var item = new Symbol(terminal, SymbolType.Terminal);
                _lstSymbols.Add(item);
            }

            var isStart = true;
            foreach (var nonterminal in nonterminals.Split(new char[] {' '}))
            {
                if (isStart)
                {
                    var item = new Symbol(nonterminal, SymbolType.Start);
                    _lstSymbols.Add(item);
                    isStart = false;
                }
                else
                {
                    var item = new Symbol(nonterminal, SymbolType.NonTerminal);
                    _lstSymbols.Add(item);
                }
            }

            for (var i = 2; i < lines.Length; i++)
            {
                Production production = new Production();
                production.LHS = GetSymbol(lines[i].Split(new char[]{' '})[0]);

                for (var j = 1; j < lines[i].Split(new char[] { ' ' }).Length; j++)
                {
                    production.RHS.Add(GetSymbol(lines[i].Split(new char[] { ' ' })[j]));                    
                }

                this.Grammar.Add(production);
            }
        }

        private Symbol GetSymbol(string symbolName)
        {
            return _lstSymbols.FirstOrDefault(item => item.Name == symbolName);
        }

        private int GetRulesCount()
        {
            return _setTable.Sum(s => s.States.Count);
        }
    }
}
