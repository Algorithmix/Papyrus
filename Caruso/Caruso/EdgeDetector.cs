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

                double lStdDev = 0;
                double rStdDev = 0;

                double lLowBound = 0;
                double lHighBound = 0;
                double rLowBound = 0;
                double rHighBound = 0;

                int count = 0;

                Queue<int> lData = new Queue<int>();
                Queue<int> rData = new Queue<int>();

                for (int j = (int)(shred.Height * 0.25); j < (int)(shred.Height * 0.75); j++)
                {
                    if (lData.Count == 10)
                    {
                        lMean = getAverage(lData);
                        rMean = getAverage(rData);
                        lStdDev = getStdDev(lData, lMean);
                        rStdDev = getStdDev(rData, rMean);

                        lLowBound = lMean - lStdDev * 3;
                        lHighBound = lMean + lStdDev * 3;

                        rLowBound = rMean - rStdDev * 3;
                        rHighBound = rMean + rStdDev * 3;
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

                    if (xHits.Count >= 2)
                    {
                        //abstract edges from xHits
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

                        bool lfilter = (currentLowest >= lLowBound) && (currentLowest <= lHighBound);
                        bool rfilter = (currentHighest >= rLowBound) && (currentHighest <= rHighBound);

                        //add data to queue's
                        if (lData.Count < 10)
                        {
                            lData.Enqueue(currentLowest);
                            rData.Enqueue(currentHighest);
                        }
                        else
                        {
                            lData.Enqueue(currentLowest);
                            lData.Dequeue();
                            rData.Enqueue(currentHighest);
                            rData.Dequeue();

                            if (lfilter && rfilter)
                            {
                                lrunsum += (lMean - currentLowest) * (lMean - currentLowest);
                                rrunsum += (rMean - currentHighest) * (rMean - currentHighest);
                                count++;
                            }
                        }
                    }
                }



                double lVariance = lrunsum / count;
                double rVariance = rrunsum / count;

                Tuple<double, double> output = new Tuple<double, double>(lVariance, rVariance);

                return output;
            }


            static double getStdDev(Queue<int> data, double mean)
            {
                double runsum = 0;
                int counter = 0;

                foreach (int x in data)
                {
                    runsum += (mean - x) * (mean - x);
                    counter++;
                }

                double variance = runsum / counter;
                return Math.Sqrt(variance);
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