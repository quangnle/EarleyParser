using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreParser
{
    public class State
    {
        public int Id { get; set; }
        public Production Rule { get; set; }
        public List<State> BackPointers { get; set; }
        public int DotPosition { get; set; }
        public int OriginPosition { get; set; }
        public ParseStatus Status { get; set; }

        public State(int id, Production rule, int dotPosition, int originPosition)
        {
            this.Id = id;
            this.Rule = rule;
            this.DotPosition = dotPosition;
            this.OriginPosition = originPosition;
        }

        public void AddBackPointer(State state)
        {
            if (this.BackPointers == null)
                this.BackPointers = new List<State>();

            this.BackPointers.Add(state);
        }

        public bool IsComplete()
        {
            return (this.DotPosition >= this.Rule.RHS.Count);
        }

        public Symbol NextSymbol()
        {
            if (this.DotPosition >= this.Rule.RHS.Count)
                return null;
            return this.Rule.RHS[this.DotPosition];
        }

        public override string ToString()
        {
            var result = this.Rule.LHS.Name + " -> ";

            for (var i = 0; i < this.Rule.RHS.Count; i++ )
            {
                if (this.DotPosition == i)
                {
                    result = result + " .";
                }

                result = result + " " + this.Rule.RHS[i].Name;
            }

            var backPointers = "[ ";
            if (this.BackPointers != null && this.BackPointers.Count > 0)
            { 
                foreach (var item in this.BackPointers)
	            {
                    backPointers += item.Id.ToString() + " ";
	            }                
            }

            backPointers += "]";

            result = this.Id.ToString() + "> " + result + ", " + this.OriginPosition.ToString() + " (" + this.Status.ToString() + ") " + backPointers;
            return result;
        }
        
    }
}
