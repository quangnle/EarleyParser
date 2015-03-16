using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreParser
{
    public class Production
    {
        public Symbol LHS { get; set; }

        private List<Symbol> _rhs;
        public List<Symbol> RHS 
        {
            get 
            {
                if (_rhs == null)
                    _rhs = new List<Symbol>();
                return _rhs; 
            }
            set { _rhs = value; }
        }

        public override bool Equals(object obj)
        {
            try
            {
                var castObj = (Production)obj;
                if (this.LHS == castObj.LHS)
                {
                    if (this.RHS.Count == castObj.RHS.Count)
                    {
                        for (var i = 0; i < this.RHS.Count; i++)
                        {
                            if (this.RHS[i] != castObj.RHS[i])
                                return false;
                        }

                        return true;
                    }
                    else return false;
                }
                else return false;

            }
            catch { throw new InvalidCastException(); }

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
