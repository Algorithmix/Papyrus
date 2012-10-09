using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;

namespace Caruso
{
    namespace Forensics
    {
        public class Chamfer
        {
            /// <summary>
            /// Calculates the chamfer from a convolution
            /// </summary>
            /// <param name="convolution">An array of doubles, where 0 indicates no feature and non zero indicates a feature</param>
            /// <returns>A chamfer value for each pixel</returns>
            public static int[] Calculate(double[] convolution)
            {
                List<int> features = new List<int> {0};
                for (int ii = 1; ii < convolution.Length - 1; ii++)
                {
                    if (convolution[ii] > 0.0)
                    {
                        features.Add(ii);
                    }
                }
                features.Add(convolution.Length - 1);

                int[] chamfers = new int[convolution.Length];
                int previous = 0;
                int next = 1;
                for (int ii = 0; ii < chamfers.Length; ii++)
                {
                    if (ii > features[next])
                    {
                        previous++;
                        next++;
                    }
                    chamfers[ii] = Math.Min(ii - features[previous], features[next] - ii);
                }

                return chamfers;
            }
        }
    }
}
