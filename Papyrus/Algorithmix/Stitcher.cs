using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;

namespace Algorithmix
{
    public class Stitcher
    {
        public static void ExportImage(Cluster cluster, string path = "cluster.png")
        {
            Merge(cluster).Save(path, ImageFormat.Png);
        }

        public static Bitmap Merge(Cluster cluster)
        {
            return Merge(cluster.Flattened);
        }

        public static Bitmap Merge(IEnumerable<Shred> shreds)
        {
            Bitmap[] images = new Bitmap[shreds.Count()];
            int index = 0;
            foreach (Shred shred in shreds)
            {
                images[index] = shred.Bitmap;
                index += 1;
            }
            return Combine(images);
        }

        public static Bitmap Combine(Bitmap[] images)
        {
            Bitmap finalImage = null;
            try
            {
                int width = 0;
                int height = 0;

                foreach (Bitmap bitmap in images)
                {
                    //update the size of the final bitmap
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;
                }

                //create a bitmap to hold the combined image
                finalImage = new Bitmap(width, height);

                //get a graphics object from the image so we can draw on it
                using (Graphics g = Graphics.FromImage(finalImage))
                {
                    //set background color
                    g.Clear(Color.Transparent);

                    //go through each image and draw it on the final image
                    int offset = 0;
                    foreach (Bitmap image in images)
                    {
                        g.DrawImage(image,
                                    new Rectangle(offset, 0, image.Width, image.Height));
                        offset += image.Width;
                    }
                }

                return finalImage;
            }
            catch (Exception ex)
            {
                if (finalImage != null)
                    finalImage.Dispose();

                throw ex;
            }
            finally
            {
                //clean up memory
                foreach (Bitmap image in images)
                {
                    image.Dispose();
                }
            }
        }
    }
}