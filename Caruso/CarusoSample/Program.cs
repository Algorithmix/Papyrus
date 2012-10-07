using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caruso;

namespace CarusoSample
{
    class Program
    {
        static void Main(string[] args)
        {
           // Caruso.Visualizer.Plot(new Double[]{ 0.0, 0.1, 0.2, 0.3, 0.1, 0.2, 0.3},"Hello");
            ForensicsSample.LumaFromLeft(args[0]);
            ForensicsSample.ConvolutionFromLeft(args[0]);
            ForensicsSample.ChamberFromLeft(args[0]);
        }
    }
}
