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
            List<MatchData> list = BuildSortedList(input);
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
                if ((node = MatchData.SmartClusterNodes(list[pointer])) != null)
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
            PriorityQueue<MatchData, double> queue = BuildQueue(input);
            List<INode> nodes = new List<INode>(input.Count);
            int expected = input.Count;
            while (queue.Count > 0)
            {
                MatchData match = queue.Dequeue();
                INode cluster = MatchData.ClusterNodes(match);
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

        public static List<MatchData> BuildSortedList(List<Shred> shreds)
        {
            PriorityQueue<MatchData, double> queue = BuildQueue(shreds);
            List<MatchData> sorted = new List<MatchData>(queue.Count);
            foreach (MatchData data in queue)
            {
                sorted.Add(data);
            }
            return sorted;
        }

        public static PriorityQueue<MatchData, double> BuildQueue(List<Shred> shreds)
        {
            PriorityQueue<MatchData, double> queue = new PriorityQueue<MatchData, double>(PriorityQueueType.Maximum);
            foreach (Shred shred in shreds)
            {
                foreach (Shred other in shreds)
                {
                    if (shred.Id == other.Id)
                    {
                        continue;
                    }

                    MatchData matchDataForwardsRegular = MatchData.CompareShred(shred, other,
                                                                 Direction.FromRight,
                                                                 Orientation.Regular,
                                                                 Direction.FromLeft,
                                                                 Orientation.Regular);
                    MatchData matchDataBackwardsRegular = MatchData.CompareShred(shred, other,
                                                                  Direction.FromLeft,
                                                                  Orientation.Regular,
                                                                  Direction.FromRight,
                                                                  Orientation.Regular);
                    MatchData matchDataForwardsReverse = MatchData.CompareShred(shred, other,
                                                                 Direction.FromRight,
                                                                 Orientation.Reversed,
                                                                 Direction.FromLeft,
                                                                 Orientation.Regular);
                    MatchData matchDataBackwardsReverse = MatchData.CompareShred(shred, other,
                                                                  Direction.FromLeft,
                                                                  Orientation.Reversed,
                                                                  Direction.FromRight,
                                                                  Orientation.Regular);

                    queue.Enqueue(matchDataForwardsRegular, matchDataForwardsRegular.ChamferSimilarity);
                    queue.Enqueue(matchDataBackwardsRegular, matchDataBackwardsRegular.ChamferSimilarity);
                    queue.Enqueue(matchDataForwardsReverse, matchDataForwardsReverse.ChamferSimilarity);
                    queue.Enqueue(matchDataBackwardsReverse, matchDataBackwardsReverse.ChamferSimilarity);
                }
            }
            return queue;
        }

        #endregion
    }
}