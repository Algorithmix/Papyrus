#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Algorithmix
{
    public class Differ
    {
        public static double DiffShredByOrder(List<long> first, List<long> second)
        {
            if (first.Count != second.Count)
            {
                throw new Exception("Expect both lists to be the same length");
            }
            int length = first.Count - 1;
            int power = first.Count;


            HashSet<Tuple<long, long>> firstSet = new HashSet<Tuple<long, long>>();
            HashSet<Tuple<long, long>> secondSet = new HashSet<Tuple<long, long>>();

            for (int ii = 0; ii < length; ii++)
            {
                firstSet.Add(new Tuple<long, long>(first[ii], first[ii + 1]));
                secondSet.Add(new Tuple<long, long>(second[ii], second[ii + 1]));
            }

            var intersect = firstSet.Intersect(secondSet);
            return intersect.Count()/(double) length;
        }
    }
}