#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Algorithmix
{
    public class Differ
    {
        public static double DiffShredByOrder<T>(List<T> first, List<T> second)
        {
            if (first.Count != second.Count)
            {
                throw new Exception("Expect both lists to be the same length");
            }
            int length = first.Count - 1;
            int full = first.Count;

            HashSet<Tuple<T, T>> firstSet = new HashSet<Tuple<T, T>>();
            HashSet<Tuple<T, T>> secondSet = new HashSet<Tuple<T, T>>();

            // Add forward pairings
            for (int ii = 0; ii < length; ii++)
            {
                firstSet.Add(Tuple.Create(first[ii], first[ii + 1]));
                secondSet.Add(Tuple.Create(second[ii], second[ii + 1]));
            }

            // Add reverse pairings for first set only
            for (int ii=1; ii < full ; ii++)
            {
                firstSet.Add(Tuple.Create(first[ii], first[ii - 1]));
            }

            var intersect = firstSet.Intersect(secondSet);
            return intersect.Count()/(double) length;
        }
    }
}