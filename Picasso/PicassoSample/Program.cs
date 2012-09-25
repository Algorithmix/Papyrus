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
            string imagesrc = args[0];
            Bitmap source = new Bitmap(imagesrc);
            Bitmap mask = Utility.FloodFill(source, 0, 0, 95);
            mask.Save("mask.png");
        }
    }
}
