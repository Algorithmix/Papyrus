using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;


namespace Caruso
{
    public class Forensics
    {
        public static double Luma(Bgr color)
        {
            return (0.3*color.Red + 0.59*color.Green + 0.11*color.Blue);
        }
    }
}
