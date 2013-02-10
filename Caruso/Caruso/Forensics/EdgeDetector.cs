#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using AForge.Imaging.Filters;

#endregion

namespace Algorithmix
{
    namespace Forensics
    {
        public class EdgeDetector
        {
            public static Tuple<double, double> AnalyzeShred(Bitmap inShred)
            {
                //apply edge detection
                Bitmap gsShred = Grayscale.CommonAlgorithms.BT709.Apply(inShred);
                CannyEdgeDetector filter = new CannyEdgeDetector();
                Bitmap shred = filter.Apply(gsShred);

                //parameters
                int queueLength = 100;
                double maxDistanceFactor = 0.025;

                double lExpected = 0;
                double rExpected = 0;

                double lrunsum = 0;
                double rrunsum = 0;

                double lStdDev = 0;
                double rStdDev = 0;

                double lLowBound = 0;
                double lHighBound = 0;
                double rLowBound = 0;
                double rHighBound = 0;

                int count = 0;
                int lastJ = 0;

                Queue<Tuple<Point, double>> lData = new Queue<Tuple<Point, double>>();
                Queue<Tuple<Point, double>> rData = new Queue<Tuple<Point, double>>();

                for (int j = (int) (shred.Height*0.25); j < (int) (shred.Height*0.75); j++)
                {
                    if (lData.Count == queueLength)
                    {
                        lExpected = GetPrediction(lData, j);
                        rExpected = GetPrediction(rData, j);
                        lStdDev = GetStdDev(lData);
                        rStdDev = GetStdDev(rData);

                        lLowBound = lExpected - lStdDev*3;
                        lHighBound = lExpected + lStdDev*3;

                        rLowBound = rExpected - rStdDev*3;
                        rHighBound = rExpected + rStdDev*3;
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
                        if (lData.Count < queueLength)
                        {
                            lData.Enqueue(new Tuple<Point, double>(new Point(currentLowest, j), currentLowest));
                            rData.Enqueue(new Tuple<Point, double>(new Point(currentHighest, j), currentHighest));
                            lastJ = j;
                        }
                        else
                        {
                            if (lfilter && rfilter)
                            {
                                lData.Enqueue(new Tuple<Point, double>(new Point(currentLowest, j), lExpected));
                                lData.Dequeue();
                                rData.Enqueue(new Tuple<Point, double>(new Point(currentHighest, j), rExpected));
                                rData.Dequeue();

                                lrunsum += (currentLowest - lExpected)*(currentLowest - lExpected);
                                rrunsum += (currentHighest - rExpected)*(currentHighest - rExpected);
                                count++;
                                lastJ = j;
                            }
                                //if we've gone too far without finding a match, clear the queue
                            else if (j - lastJ > shred.Height*maxDistanceFactor)
                            {
                                for (int i = 0; i < queueLength; i++)
                                {
                                    rData.Dequeue();
                                    lData.Dequeue();
                                }
                                lData.Enqueue(new Tuple<Point, double>(new Point(currentLowest, j), lExpected));
                                rData.Enqueue(new Tuple<Point, double>(new Point(currentHighest, j), rExpected));
                            }
                        }
                    }
                }


                double lVariance = lrunsum/count;
                double rVariance = rrunsum/count;

                Tuple<double, double> output = new Tuple<double, double>(lVariance, rVariance);

                return output;
            }


            private static double GetStdDev(Queue<Tuple<Point, double>> data)
            {
                double runsum = 0;
                int counter = 0;

                foreach (Tuple<Point, double> x in data)
                {
                    runsum += (x.Item2 - x.Item1.X)*(x.Item2 - x.Item1.X);
                    counter++;
                }

                if (Math.Abs(runsum - 0) < 0.001)
                    return 100;
                else
                {
                    double variance = runsum/counter;
                    return Math.Sqrt(variance);
                }
            }


            private static double GetPrediction(Queue<Tuple<Point, double>> input, int j)
            {
                int xrunsum = 0;
                int yrunsum = 0;
                int xSquaredRunsum = 0;
                int productRunsum = 0;
                int counter = 0;

                foreach (Tuple<Point, double> x in input)
                {
                    Point p = x.Item1;
                    int X = p.Y;
                    int Y = p.X;
                    xrunsum += X;
                    yrunsum += Y;
                    xSquaredRunsum += X*X;
                    productRunsum += X*Y;
                    counter++;
                }

                double xAve = xrunsum/(double) counter;
                double yAve = yrunsum/(double) counter;


                double m = (counter*productRunsum - xrunsum*yrunsum)/
                           (double) (counter*xSquaredRunsum - xrunsum*xrunsum);
                double b = yAve - m*xAve;
                double output = m*j + b;
                return output;
            }
        }
    }
}