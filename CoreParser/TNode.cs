using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreParser
{
    public class TNode<T>
    {
        public T Content { get; set; }
        public TNode<T> Parent { get; set; }
        public List<TNode<T>> Children { get; set; }        
    }
}
