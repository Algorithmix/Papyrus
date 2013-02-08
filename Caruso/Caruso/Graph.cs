#region

using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

#endregion

namespace Algorithmix
{
    public partial class Graph : Form
    {
        public Graph(double[] xx, double[] yy, string title)
        {
            InitializeComponent();
            var pane = ZGraph.GraphPane;
            pane.Title.Text = title;
            var data = new PointPairList(xx, yy);
            pane.AddStick("Data", data, Color.Red);
            ZGraph.AxisChange();
        }

        private void zedGraphControl1_Load(object sender, EventArgs e)
        {
        }
    }
}