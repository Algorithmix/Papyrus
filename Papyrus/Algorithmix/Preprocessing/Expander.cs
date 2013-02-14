using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithmix.Preprocessing
{
    class Expander
    {
        //the border in pixels for expanding for edge detecting
        private static int border = 20;
        /// <summary>
        /// Expands the given image to with a transparent boarder of size determined in this class
        /// </summary>
        /// <param name="shred">the image to expand</param>
        /// <returns>an expanded image</returns>
        public static System.Drawing.Bitmap Expand(System.Drawing.Bitmap shred)
        {
            //read all images into memory
            System.Drawing.Bitmap finalImage = null;

            try
            {

                //create a bitmap to hold the stretched image (20px on each border
                finalImage = new System.Drawing.Bitmap(shred.Width + 2 * border, shred.Height + 2* border);

                //get a graphics object from the image so we can draw on it
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
                {
                    //set background color, make it clear
                    g.Clear(System.Drawing.Color.Transparent);

                    g.DrawImage(shred, new System.Drawing.Rectangle(border, border, shred.Width, shred.Height));
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
