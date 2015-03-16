using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarleyUI.UserControls
{
    public class NodeUI
    {
        public string Text { get; set; }
        public NodeUI Parent { get; set; }
        public List<NodeUI> Children { get; set; }
        public int NumOfLeaves { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}
