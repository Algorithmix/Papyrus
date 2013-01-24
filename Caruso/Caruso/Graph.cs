using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace Algorithmix
{
    public partial class Graph : Form
    {
        public Graph(double[] xx, double[] yy, string title)
        {
            InitializeComponent();
            var pane = this.ZGraph.GraphPane;
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
