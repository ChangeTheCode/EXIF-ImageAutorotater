using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Threading;

namespace AutoRotate.Image.Logic
{
    public class ImageRotateLogic : IImageRotateLogic
    {
        public void RotateImages(string directoryName, IReadOnlyCollection<string> fileFilterOptions)
        {
            var allFilesInDirectory = LoadAllFilesWithFiterOptions(directoryName, fileFilterOptions);

            foreach (var item in allFilesInDirectory)
            {
                RotateImage(item);
            }
        }

        public void RotateImagesParallel(string directoryName, IReadOnlyCollection<string> fileFilterOptions, CancellationToken token)
        {
            var allFilesInDirectory = LoadAllFilesWithFiterOptions(directoryName, fileFilterOptions);

            var taskPool = new List<Task>();

            foreach (var oneImagePath in allFilesInDirectory)
            {
                var singleTask = Task.Factory.StartNew(() => RotateImage(oneImagePath));

                taskPool.Add(singleTask);
            }

            Task.WaitAll(taskPool.ToArray(), token);
        }

        private void RotateImage(string itemFilePath)
        {
            if (File.Exists(itemFilePath))
            {
                using (MemoryStream inMemoryCopy = new MemoryStream())
                {
                    using (FileStream fs = File.OpenRead(itemFilePath))
                    {
                        fs.CopyTo(inMemoryCopy);
                    }

                    try
                    {
                        using (var bit = new Bitmap(inMemoryCopy))
                        {
                            bit.ExifRotate();

                            bit.Save(itemFilePath);
                        }
                    }
                    catch (Exception e)
                    {
                        RestoreImageByFail(itemFilePath, inMemoryCopy);
                        Console.WriteLine("Image was resotred: " + e.Message + " " + e.StackTrace);
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
