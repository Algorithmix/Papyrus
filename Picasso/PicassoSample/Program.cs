using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Picasso;
using System.Drawing;

namespace PicassoSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap source = new Bitmap(args[0]);
            Bitmap mask = Utility.FloodFill(source, 0, 0, 150);
            mask.Save("mask.png");
        }
    }
}
