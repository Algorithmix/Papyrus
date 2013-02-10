#region

using System;
using System.Linq;
using System.Windows.Forms;
using NLog;

#endregion

namespace Algorithmix
{
    public class Visualizer
    {
        public static Logger Log = LogManager.GetCurrentClassLogger();

        public static void Plot(int[] yy, String title)
        {
            double[] doubles = new double[yy.Length];

            for (int ii = 0; ii < yy.Length; ii++)
            {
                doubles[ii] = yy[ii];
            }
            Plot(doubles, title);
        }

        public static void Plot(double[] yy, String title)
        {
            const int xMin = 0;
            int xMax = yy.Length;
            int yMin = (int) Math.Floor(yy.Min());
            int yMax = (int) Math.Ceiling(yy.Max());

            double[] xx = new double[yy.Length];
            for (int ii = xMin; ii < xMax; ii++)
            {
                xx[ii] = ii;
            }

            Plot(xx, yy, xMin, xMax, yMin, yMax, title);
        }

        public static void Plot(double[] xx, double[] yy, String title)
        {
            int xMin = (int) Math.Floor(xx.Min());
            int xMax = (int) Math.Ceiling(xx.Max());
            int yMin = (int) Math.Floor(yy.Min());
            int yMax = (int) Math.Ceiling(yy.Max());

            Plot(xx, yy, xMin, xMax, yMin, yMax, title);
        }

        public static void Plot(double[] xx, double[] yy, int xMin, int xMax, int yMin, int yMax, String title)
        {
            if (xx.Length != yy.Length)
            {
                Log.Error("X and Y Sizes don't Match");
                Log.Error("X Data.size = " + xx.Length);
                Log.Error("Y Data.size = " + yy.Length);
                throw new ArgumentException("X and Y Sizes don't match, can't plot");
            }

            Log.Info("Plotting Graph " + title);
            Application.Run(new Graph(xx, yy, title));
        }
    }
}