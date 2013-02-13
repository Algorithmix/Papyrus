#region

using System.Collections.Generic;
using System.IO;
using NGenerics.DataStructures.Queues;

#endregion

namespace Algorithmix
{
    public class Reconstructor
    {
        public static List<Shred> NaiveKruskalAlgorithm(List<Shred> input)
        {
            PriorityQueue<Data, double> queue = BuildQueue(input);
            List<INode> nodes = new List<INode>(input.Count);
            int expected = input.Count;
            while (queue.Count > 0)
            {
                Data match = queue.Dequeue();
                INode cluster = Data.ClusterNodes(match);
                if (cluster != null)
                {
                    nodes.Add(cluster);
                    if (cluster.Size() == expected)
                    {
                        List<Shred> ordered = new List<Shred>(input.Count);
                        cluster.Flatten(ordered);
                        return ordered;
                    }
                }
            }

            throw new InvalidDataException("Unable to generate full match");
        }

        public static PriorityQueue<Data, double> BuildQueue(List<Shred> shreds)
        {
            PriorityQueue<Data, double> queue = new PriorityQueue<Data, double>(PriorityQueueType.Maximum);
            foreach (Shred shred in shreds)
            {
                foreach (Shred other in shreds)
                {
                    if (shred.Id == other.Id)
                    {
                        continue;
                    }

                    Data dataForwardsRegular = Data.CompareShred(shred, other,
                                                                 Direction.FromRight,
                                                                 Orientation.Regular,
                                                                 Direction.FromLeft,
                                                                 Orientation.Regular);
                    Data dataBackwardsRegular = Data.CompareShred(shred, other,
                                                                  Direction.FromLeft,
                                                                  Orientation.Regular,
                                                                  Direction.FromRight,
                                                                  Orientation.Regular);
                    Data dataForwardsReverse = Data.CompareShred(shred, other,
                                                                 Direction.FromRight,
                                                                 Orientation.Reversed,
                                                                 Direction.FromLeft,
                                                                 Orientation.Regular);
                    Data dataBackwardsReverse = Data.CompareShred(shred, other,
                                                                  Direction.FromLeft,
                                                                  Orientation.Reversed,
                                                                  Direction.FromRight,
                                                                  Orientation.Regular);

                    queue.Enqueue(dataForwardsRegular, dataForwardsRegular.ChamferSimilarity);
                    queue.Enqueue(dataBackwardsRegular, dataBackwardsRegular.ChamferSimilarity);
                    queue.Enqueue(dataForwardsReverse, dataForwardsReverse.ChamferSimilarity);
                    queue.Enqueue(dataBackwardsReverse, dataBackwardsReverse.ChamferSimilarity);
                }
            }
            return queue;
        }
    }
}