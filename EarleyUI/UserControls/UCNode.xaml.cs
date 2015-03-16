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

namespace EarleyUI.UserControls
{
    /// <summary>
    /// Interaction logic for UCNode.xaml
    /// </summary>
    public partial class UCNode : UserControl
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set 
            { 
                _text = value;
                lblText.Content = _text;
            }
        }        

        public UCNode()
        {
            InitializeComponent();
        }
    }
}
