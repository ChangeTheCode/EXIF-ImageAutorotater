using System;
using System.Linq;
using System.Drawing;

namespace AutoRotate.Image.Logic
{
    public static class ImageExtensions
    {

        private const int exifOrientationID = 0x112; //274

        public static RotateFlipType GetRotation(Bitmap img)
        {
            if (!img.PropertyIdList.Contains(exifOrientationID))
            {
                throw new NotSupportedException("No exifOrientationID available ");
            }

            var prop = img.GetPropertyItem(exifOrientationID);

            int val = BitConverter.ToUInt16(prop.Value, 0);

            switch (val)
            {
                case 1:
                    // No rotation required.
                    return RotateFlipType.RotateNoneFlipNone;
                case 2:
                    return RotateFlipType.RotateNoneFlipX;
                case 3:
                    return RotateFlipType.Rotate180FlipY;
                case 4:
                    return RotateFlipType.Rotate180FlipX;
                case 5:
                    return RotateFlipType.Rotate90FlipX;
                case 6:
                    return RotateFlipType.Rotate90FlipNone;
                case 7:
                    return RotateFlipType.Rotate270FlipX;
                case 8:
                    return RotateFlipType.Rotate270FlipNone;   
            }
            return RotateFlipType.RotateNoneFlipNone;
        }

        public static void ExifRotate(this System.Drawing.Image img)
        {
            if (!img.PropertyIdList.Contains(exifOrientationID))
                return;

            var prop = img.GetPropertyItem(exifOrientationID);
            int val = BitConverter.ToUInt16(prop.Value, 0);
            var rot = RotateFlipType.RotateNoneFlipNone;

            if (val == 3 || val == 4)
                rot = RotateFlipType.Rotate180FlipNone;
            else if (val == 5 || val == 6)
                rot = RotateFlipType.Rotate90FlipNone;
            else if (val == 7 || val == 8)
                rot = RotateFlipType.Rotate270FlipNone;

            if (val == 2 || val == 4 || val == 5 || val == 7)
                rot |= RotateFlipType.RotateNoneFlipX;

            if (rot != RotateFlipType.RotateNoneFlipNone)
                img.RotateFlip(rot);
        }
    }
}
