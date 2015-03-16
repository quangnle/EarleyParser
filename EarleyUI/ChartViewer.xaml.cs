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
using System.Windows.Shapes;

using CoreParser;
using EarleyUI.UserControls;

namespace EarleyUI
{
    /// <summary>
    /// Interaction logic for ChartViewer.xaml
    /// </summary>
    public partial class ChartViewer : Window
    {
        public ChartViewer(TreeViewItem source, int screenWidth)
        {
            var root = new NodeUI();
            root.Text = (string)source.Header;
            root.X = screenWidth / 2;
            root.Y = 40;

            CopyTree(source, root);
            CountLeaves(root);
            SetPositions(root, 0, screenWidth);

            InitializeComponent();

            DrawTree(root);
        }

        private void CopyTree(TreeViewItem node, NodeUI nodeUI)
        {
            if (node.Items != null)
            {   
                for (int i = 0; i < node.Items.Count; i++)
                {
                    var newNodeUI = new NodeUI();
                    newNodeUI.Parent = nodeUI;
                    newNodeUI.Text = (string)((TreeViewItem)node.Items[i]).Header;

                    if (nodeUI.Children == null)
                        nodeUI.Children = new List<NodeUI>();

                    nodeUI.Children.Add(newNodeUI);

                    CopyTree((TreeViewItem)node.Items[i], newNodeUI);
                }                
            }
        }

        private int CountLeaves(NodeUI node)
        {
            if (node.Children == null)
            {
                node.NumOfLeaves = 1;
                return 1;
            }

            int result = 0;

            for (int i = 0; i < node.Children.Count; i++)
            {
                result = result + CountLeaves(node.Children[i]);
            }

            node.NumOfLeaves = result;

            return result;
        }

        private void SetPositions(NodeUI node, double startX, double endX)
        {
            
            var totalLeaves = node.NumOfLeaves;
            var width = endX - startX;
            var aPieceWidth = width / totalLeaves;

            if (node.Children != null)
            {
                var currentStartX = startX;

                for (int i = 0; i < node.Children.Count; i++)
                {
                    node.Children[i].X = currentStartX + aPieceWidth * node.Children[i].NumOfLeaves * 0.5;
                    node.Children[i].Y = node.Y + 60;

                    SetPositions(node.Children[i], currentStartX, currentStartX + aPieceWidth * node.Children[i].NumOfLeaves);

                    currentStartX = currentStartX + aPieceWidth * node.Children[i].NumOfLeaves;
                }
            }
        }

        private void DrawLines(NodeUI node)
        {
            if (node.Parent != null)
            {
                canvas.Children.Add(new Line()
                {
                    X1 = node.Parent.X,
                    Y1 = node.Parent.Y,
                    X2 = node.X,
                    Y2 = node.Y,
                    Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    StrokeThickness = 1
                });
            }

            if (node.Children != null)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    DrawLines(node.Children[i]);
                }
            }
        }

        private void DrawNodes(NodeUI node)
        {
            if (node.Parent == null)
            {
                var ucNode = new UCNode();
                ucNode.Text = node.Text;

                Canvas.SetTop(ucNode, node.Y - 12);
                Canvas.SetLeft(ucNode, node.X - 12);

                canvas.Children.Add(ucNode);
            }
            else
            {
                var ucNode = new UCNode();
                ucNode.Text = node.Text;

                Canvas.SetTop(ucNode, node.Y - 12);
                Canvas.SetLeft(ucNode, node.X - 12);

                canvas.Children.Add(ucNode);
            }

            if (node.Children != null)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    DrawNodes(node.Children[i]);
                }
            }
        }

        private void DrawTree(NodeUI node)
        {
            DrawLines(node);
            DrawNodes(node);
        }
    }
}
