#region

using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

#endregion

namespace Algorithmix.Forensics
{
    public class EdgeDetector
    {
        public static int[] EdgePoints(Bitmap shred, Direction direction, double percentageIgnored = 0.15)
        {
            if (percentageIgnored > 0.5 || percentageIgnored < 0)
            {
                throw new ArgumentException("Must have a percentage ignored that is between 0 and 0.5");
            }

            if (direction == Direction.FromLeft)
            {
                return ScanFromLeft(shred, direction, percentageIgnored);
            }
            if (direction == Direction.FromRight)
            {
                return ScanFromRight(shred, direction, percentageIgnored);
            }

            throw new ArgumentException("Non Left-Right directions not supported for this method");
        }

        public static int[] ScanFromLeft(Bitmap shred, Direction direction, double percentageIgnored = 0.15)
        {
            int startHeight = (int) (percentageIgnored*(shred.Height));
            int stopHeight = (int) ((1 - percentageIgnored)*(shred.Height));
            int[] edgePoints = new int[shred.Height];
            using (Image<Bgra, byte> image = new Image<Bgra, byte>(shred))
            {
                for (int row = 0; row < image.Height; row++)
                {
                    if (row < startHeight || row >= stopHeight) // If we are in the ignore range
                    {
                        edgePoints[row] = IgnorePoint;
                    }
                    else // find the first none transperant point
                    {
                        for (int col = 0; col < image.Width; col++)
                        {
                            if (!(Math.Abs(image[row, col].Alpha - byte.MaxValue) < 0.0001)) continue;
                            edgePoints[row] = col;
                            break;
                        }
                    }
                }
            }
            return edgePoints;
        }

        public static int[] ScanFromRight(Bitmap shred, Direction direction, double percentageIgnored = 0.15)
        {
            int startHeight = (int) (percentageIgnored*(shred.Height));
            int stopHeight = (int) ((1 - percentageIgnored)*(shred.Height));
            int[] edgePoints = new int[shred.Height];
            using (Image<Bgra, byte> image = new Image<Bgra, byte>(shred))
            {
                for (int row = 0; row < image.Height; row++)
                {
                    if (row < startHeight || row >= stopHeight)
                    {
                        edgePoints[row] = IgnorePoint;
                    }
                    else
                    {
                        for (int col = image.Width - 1; col >= 0; col--)
                        {
                            if (!(Math.Abs(image[row, col].Alpha - byte.MaxValue) < 0.0001)) continue;
                            edgePoints[row] = col;
                            break;
                        }
                    }
                }
            }
            return edgePoints;
        }


        private static int Average(int[] sum, int middle, int size)
        {
            int start;
            int end;
            if ((middle - size / 2) >= 0 && (middle + size / 2) < sum.Length)
            {
                // If we in the middle, use full sample area
                start = middle - size/2;
                end = middle + size/2;
            }
            else if ((middle - size / 2) < 0 && (middle + size / 2) < sum.Length)
            {
                // If we are closer to the start set sample area to be smaller
                start = 0;
                end = middle + middle;
            }
            else if ( (middle-size/2)>=0 && (middle+size/2) >= sum.Length )
            {
                // If we are close to the end, set sample area to be smaller
                start = middle - (sum.Length-middle);
                end = sum.Length - 1;
            }
            else
            {
                // This case should never occur
                return IgnorePoint;
            }
            return (int)(sum[start] - sum[end])/(start - end);
        }

        public static int[] Smoothen(int[] edgePoints, int size = 10)
        {
            if (size > edgePoints.Length)
            {
                throw new ArgumentException("Smoothing size is too large for image");
            }

            Queue<int> queue = new Queue<int>(size);
            int[] smooth = new int[edgePoints.Length];
            int sum = 0;
            for (int ii = 0; ii < edgePoints.Length; ii++)
            {
                if (edgePoints[ii] == IgnorePoint) continue;
                if (queue.Count != size)
                {
                    queue.Enqueue(edgePoints[ii]);
                    sum += edgePoints[ii];
                }
                else
                {
                    int discarded = queue.Dequeue();
                    sum -= discarded;
                    sum += edgePoints[ii];
                    queue.Enqueue(edgePoints[ii]);
                }
                smooth[ii] = (int) (sum/(double) queue.Count);
            }

            return smooth;
        }

        public static readonly int IgnorePoint = -1;
    }
}