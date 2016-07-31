using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Generic
{
    public class ImageUtil
    {
        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            return ImageToByteArray(imageIn, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn,ImageFormat ImageType)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, ImageType);
            return ms.ToArray();
        }
        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public static bool byteArrayToImage(byte[] byteArrayIn, string FileName)
        {
            if (byteArrayIn.Length <= 0 || FileName.Trim().Length <= 0)
                return false;
            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                using (Image returnImage = Image.FromStream(ms))
                {
                    returnImage.Save(FileName);
                }
            }
            return true;
        }
    }
}
