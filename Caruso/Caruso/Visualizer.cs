using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using NLog;
using System.Windows.Forms;
using ZedGraph;

namespace Caruso
{
    public class Visualizer
    {

        public static Logger log = LogManager.GetCurrentClassLogger();

        public static void Plot(int[] yy, String title)
        {
            double[] doubles = new double[yy.Length];

            for (int ii = 0; ii < yy.Length; ii++ )
            {
                doubles[ii] = (double) yy[ii];
            }
            Plot(doubles, title);
        }

        public static void Plot(double [] yy, String title)
        {
            int x_min = 0;
            int x_max = yy.Length;
            int y_min = (int)Math.Floor(yy.Min());
            int y_max = (int)Math.Ceiling(yy.Max()); 
            
            double [] xx= new double[yy.Length]; 
            for (int ii=x_min; ii<x_max;ii++)
            {
                xx[ii]=ii;
            }

            Plot(xx, yy, x_min , x_max , y_min , y_max, title);
        }

        public static void Plot( double[] xx, double[] yy , String title)
        {
            int x_min = (int) Math.Floor(xx.Min());
            int x_max = (int) Math.Ceiling(xx.Max());
            int y_min = (int) Math.Floor(yy.Min());
            int y_max = (int) Math.Ceiling(yy.Max());

            Plot(xx, yy, x_min, x_max, y_min, y_max, title);
        }
        
        public static void Plot(double[] xx, double[] yy, int xMin, int xMax, int yMin, int yMax, String title)
        {
            if ( xx.Length != yy.Length )
            {
                log.Error("X and Y Sizes don't Match");
                log.Error("X Data.size = "+xx.Length);
                log.Error("Y Data.size = "+yy.Length);
                throw new ArgumentException("X and Y Sizes don't match, can't plot");
            }

            Application.Run(new Graph(xx,yy,title));
        }
    }
}
