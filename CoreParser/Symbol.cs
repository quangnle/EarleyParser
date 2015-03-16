using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreParser
{
    public class Symbol
    {
        public SymbolType Type { get; set; }
        public string Name { get; set; }

        public Symbol(string name, SymbolType type)
        {
            this.Name = name;
            this.Type = type;
        }
        
        public override bool Equals(object obj)
        {
            try
            {
                var castObj = (Symbol)obj;

                return (this.Name == castObj.Name && this.Type == castObj.Type);
            }
            catch { throw new InvalidCastException(); }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }   
    }
}
