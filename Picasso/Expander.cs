using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picasso
{
    class Expander
    {
        public static System.Drawing.Bitmap Expand(System.Drawing.Bitmap shred)
        {
            //read all images into memory
            System.Drawing.Bitmap finalImage = null;

            try
            {

                //create a bitmap to hold the stretched image (20px on each border
                finalImage = new System.Drawing.Bitmap(shred.Width + 40, shred.Height + 40);

                //get a graphics object from the image so we can draw on it
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
                {
                    //set background color, make it clear
                    g.Clear(System.Drawing.Color.Transparent);

                    //go through each image and draw it on the final image
                    int offset = 20;

                    g.DrawImage(shred, new System.Drawing.Rectangle(offset, offset, shred.Width, shred.Height));
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
                shred.Dispose();
            }
        }
    }
}
