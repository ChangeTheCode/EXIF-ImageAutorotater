using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace AutoRotate.Image.Logic
{
    public class ImageRotateLogic : IImageRotateLogic
    {
        public void RotateImages(string directoryName, IReadOnlyCollection<string> fileFilterOptions)
        {
            var allFilesInDirectory = LoadAllFilesWithFiterOptions(directoryName, fileFilterOptions);

            foreach (var item in allFilesInDirectory)
            {
                if (File.Exists(item))
                {
                    using (MemoryStream inMemoryCopy = new MemoryStream())
                    {
                        using (FileStream fs = File.OpenRead(item))
                        {
                            fs.CopyTo(inMemoryCopy);
                        }

                        try
                        {
                            using (var bit = new Bitmap(inMemoryCopy))
                            {
                                bit.ExifRotate();

                                bit.Save(item);
                            }
                        }
                        catch (Exception e)
                        {
                            RestoreImageByFail(item, inMemoryCopy);
                            Console.WriteLine("Image was resotred: " + e.Message + " " + e.StackTrace);
                        }
                    }
                }
            }
        }

        private void RestoreImageByFail(string fullFileName, MemoryStream fileStream)
        {
            using (var image = System.Drawing.Bitmap.FromStream(fileStream))
            {
                image.Save(fullFileName, ImageFormat.Jpeg);
            }
        }

        private IReadOnlyCollection<string> LoadAllFilesWithFiterOptions(string directory, IReadOnlyCollection<string> fileFilterOptions)
        {
            var listOfAllFiles = new List<string>();
            foreach (var filesTypes in fileFilterOptions)
            {
                listOfAllFiles.AddRange(System.IO.Directory.GetFiles(directory, filesTypes));
            }

            return listOfAllFiles;
        }
    }
}
