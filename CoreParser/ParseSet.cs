using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreParser
{
    public class ParseSet
    {
        public int Level { get; set; }
        public List<State> States { get; set; }

        public void AddState(State state)
        {
            if (this.States == null)
                this.States = new List<State>();

            foreach (var aState in States)
            {
                if (aState.Rule == state.Rule && aState.DotPosition == state.DotPosition && aState.OriginPosition == state.OriginPosition)
                {
                    aState.BackPointers.AddRange(state.BackPointers);
                    return;
                }
            }

            this.States.Add(state);
        }

        public override string ToString()
        {
            var result = "Level " + this.Level.ToString() + Environment.NewLine;
            
            foreach (var state in this.States)
            {
                result += state.ToString() + Environment.NewLine;
            }

            return result;
        }
    }
}
