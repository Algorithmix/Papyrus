using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Algorithmix.TestTools;
using System.Threading.Tasks;
using System.IO;
using CarusoSample;

namespace CarusoSample
{
    class Experimental
    {
        public static void promptTestDrivePath( )
        {
            Console.Write("Enter Relative Path from TestDrive: ");
            var path = Console.ReadLine();
            var fullpath = Path.Combine(Drive.GetDriveRoot(), path);
            if (!File.Exists(fullpath)) {
                Console.WriteLine("Invalid Path");
                return;
            }
            Deliverable.Run(fullpath);
        }
    }
}
