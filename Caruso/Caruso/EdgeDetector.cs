using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using AForge.Imaging.Filters;

namespace Algorithmix
{
    namespace Forensics
    {
        public class EdgeDetector
        {
            public static Tuple<double, double> AnalyzeShred(Bitmap inShred)
            {
                Bitmap gsShred = Grayscale.CommonAlgorithms.BT709.Apply(inShred);
                CannyEdgeDetector filter = new CannyEdgeDetector();
                Bitmap shred = filter.Apply(gsShred);

                double lMean = 0;
                double rMean = 0;

                double lrunsum = 0;
                double rrunsum = 0;

                int count = 0;

                Queue<int> lData = new Queue<int>();
                Queue<int> rData = new Queue<int>();

                for (int j = (int)(shred.Height * 0.25); j < (int)(shred.Height * 0.75); j++)
                {
                    if (lData.Count == 10)
                    {
                        lMean = getAverage(lData);
                        rMean = getAverage(rData);
                    }
                    ArrayList xHits = new ArrayList();

                    //traverse each row to record edge location
                    for (int i = 0; i < shred.Width; i++)
                    {
                        if (shred.GetPixel(i, j).B >= 50)
                        {
                            xHits.Add(i);
                        }
                    }

                    int currentLowest = 99999;
                    int currentHighest = 0;

                    //abstract relevent edges from xHits
                    if (xHits.Count >= 2)
                    {
                        foreach (int x in xHits)
                        {
                            if (x < currentLowest)
                            {
                                currentLowest = x;
                            }
                            if (x > currentHighest)
                            {
                                currentHighest = x;
                            }
                        }


                        //add data to queue's
                        if (lData.Count < 10)
                        {
                            lData.Enqueue(currentLowest);
                            rData.Enqueue(currentHighest);
                        }
                        else
                        {
                            lData.Enqueue(currentLowest);
                            rData.Enqueue(currentHighest);

                            lData.Dequeue();
                            rData.Dequeue();
                        }

                        //record distance from mean squared (variance)
                        if (lMean > 0)
                        {
                            lrunsum += (lMean - currentLowest) * (lMean - currentLowest);
                            rrunsum += (rMean - currentHighest) * (rMean - currentHighest);
                            count++;
                        }
                    }
                }

                double lVariance = lrunsum / count;
                double rVariance = rrunsum / count;

                Tuple<double, double> output = new Tuple<double, double>(lVariance, rVariance);

                return output;
            }


            public static double getAverage(Queue<int> input)
            {
                int runsum = 0;

                foreach (int x in input)
                {
                    runsum += x;
                }

                return runsum / (double)input.Count;
            }
        }
    }
}