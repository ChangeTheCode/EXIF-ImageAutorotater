﻿using System;
using System.Linq;
using System.Drawing;

namespace AutoRotate.Image.Logic
{
    public static class ImageExtensions
    {
            
        private const int exifOrientationID = 0x112; //274

        public static RotateFlipType GetRotation(Bitmap img)
        {
            // if this image contains exif information then figure out which case it is
            if (img.PropertyIdList.Contains(exifOrientationID))
            {
                var prop = img.GetPropertyItem(exifOrientationID);
                int val = BitConverter.ToUInt16(prop.Value, 0);

                if (val == 3 || val == 4)
                {
                    return RotateFlipType.Rotate180FlipNone;
                }
                else if (val == 5 || val == 6)
                {
                    return RotateFlipType.Rotate90FlipNone;
                }
                else if (val == 7 || val == 8)
                {
                    return RotateFlipType.Rotate270FlipNone;
                }
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
