using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Picasso;
namespace PicassoTest
{
    [TestClass]
    public class UtilityTest
    {

        [TestMethod]
        public void TestDistance()
        {
           
            Console.WriteLine("Generating Cartesian Differences between Colors");
            double red_blue = Picasso.Utility.Distance(Color.Red, Color.Blue);
            double red_black = Picasso.Utility.Distance(Color.Red, Color.Black);
            double black_white = Picasso.Utility.Distance(Color.Black, Color.White);

            Assert.AreEqual(red_blue, Math.Sqrt((double)(255*255+255*255)));
            Assert.AreEqual(red_black, Math.Sqrt((double)(255*255)));
            Assert.AreEqual(black_white, Math.Sqrt((double)(255*255+255*255+255*255)));
            Console.WriteLine("Picasso.Utility.Distance Test Successful");
        }

        [TestMethod]
        public void TestBound()
        {
            Bitmap image = new Bitmap(3000, 3000);

            Assert.IsTrue(Picasso.Utility.Bound(image,2000,2000));
            Assert.IsFalse(Picasso.Utility.Bound(image,3010,2000));
            Assert.IsFalse(Picasso.Utility.Bound(image, 2000, 3010));
            Assert.IsFalse(Picasso.Utility.Bound(image, -200, 2000));
            Assert.IsFalse(Picasso.Utility.Bound(image, 2000, -200));
            Console.WriteLine("Picasso.Utility.Bound Test Successful");
        }
    }
}
