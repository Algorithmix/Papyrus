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
            int power = first.Count;

            HashSet<Tuple<T, T>> firstSet = new HashSet<Tuple<T, T>>();
            HashSet<Tuple<T, T>> secondSet = new HashSet<Tuple<T, T>>();

            for (int ii = 0; ii < length; ii++)
            {
                firstSet.Add(new Tuple<T, T>(first[ii], first[ii + 1]));
                secondSet.Add(new Tuple<T, T>(second[ii], second[ii + 1]));
            }

            var intersect = firstSet.Intersect(secondSet);
            return intersect.Count()/(double) length;
        }
    }
}