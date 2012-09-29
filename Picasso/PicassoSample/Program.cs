using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicassoSample
{
    class Program
    {
        public static void Main(String[] args)
        {
            PreprocessingSample.GetFloodFillMask(args[0]);
            HeuristicsSample.DetectBackground(args[0]);
        }
    }
}
