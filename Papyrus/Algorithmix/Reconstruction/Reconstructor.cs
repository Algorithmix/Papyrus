#region

using System.Collections.Generic;
using System.IO;
using NGenerics.DataStructures.Queues;
using NLog;

#endregion

namespace Algorithmix.Reconstruction
{
    public class Reconstructor
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #region Reconstruction Algorithms

        public static List<Shred> Backtrack(List<Shred> input)
        {
            // Build sorted List from the input shreds, and output nodes
            List<Data> list = BuildSortedList(input);
            int expected = input.Count;
            List<INode> clusters = new List<INode>(expected);

            // Create a stack and push the first position onto the stack
            Stack<int> stack = new Stack<int>(expected);

            // Place current pointer to list on the stack
            stack.Push(0);

            // Break when completed
            bool completed = false;
            INode root = null ;

            while (!completed)
            {
                int pointer = stack.Peek();
                INode node;
                if ((node = Data.SmartClusterNodes(list[pointer])) != null)
                {
                    clusters.Add(node);

                    // If we are complete, then return the flattened root
                    if (node.Size() == expected)
                    {
                        completed = true;
                        root = node;
                        //return ((Cluster) node).Flattened;
                    }

                    // Not complete, lets try to combine the next best shred
                    stack.Push(pointer + 1);
                }
                else
                {
                    // No luck for given match, try next best
                    if (pointer + 1 < expected)
                    {
                        stack.Push(stack.Pop() + 1);
                    }
                    else
                    {
                        // If we can backtrack
                        if (stack.Count > 1)
                        {
                            // we have exhausted all options
                            // discard the top pointer
                            stack.Pop();

                            // Deconstruct the current shred
                            // Implement INode.Orphan
                            INode parent = clusters[clusters.Count - 1];
                            clusters.RemoveAt(clusters.Count - 1);
                            parent.OrphanChildren();

                            // Try the next best match and continue
                            stack.Push(stack.Pop() + 1);                            
                        }
                        else
                        {
                            // Can't back track give up
                            completed = true;
                        }
                    }
                }
            }

            return root == null ? NaiveKruskalAlgorithm(input) : ((Cluster)root).Flattened;
        }

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

        #endregion

        #region Shred Pairing Helpers

        public static List<Data> BuildSortedList(List<Shred> shreds)
        {
            PriorityQueue<Data, double> queue = BuildQueue(shreds);
            List<Data> sorted = new List<Data>(queue.Count);
            foreach (Data data in queue)
            {
                sorted.Add(data);
            }
            return sorted;
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

        #endregion
    }
}