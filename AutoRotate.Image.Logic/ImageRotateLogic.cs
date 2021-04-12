using System;
using System.Collections.Generic;
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
                try
                {
                    RotateImage(item);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong. No changes at this image: " + e.Message + " " + e.StackTrace);
                }
            }
        }

        public void RotateImagesParallelAsync(string directoryName, IReadOnlyCollection<string> fileFilterOptions, CancellationToken token)
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

        private void RotateMulitpleImages(List<string> images)
        {
            foreach (var item in images)
            {
                RotateImage(item);
            }
        }

        private void RotateImage(string itemFilePath)
        {
            RotateFlipType rotationType;
            using(var bmp = new Bitmap(itemFilePath))
            {
                rotationType = ImageExtensions.GetRotation(bmp);
            }

            RotateJpeg(itemFilePath, rotationType);
        }

        private void RotateJpeg(string filePath, RotateFlipType rotationType,  int quality = 100)
        {         
            var image = Bitmap.FromFile(filePath);

            image.RotateFlip(rotationType);
            image.Save(filePath, ImageFormat.Jpeg); // TODO make it the file extension type dynamic 

            
        }

        private IReadOnlyCollection<string> LoadAllFilesWithFiterOptions(string directory, IReadOnlyCollection<string> fileFilterOptions)
        {
            var listOfAllFiles = new List<string>();
            foreach (var filesTypes in fileFilterOptions)
            {
                listOfAllFiles.AddRange(Directory.GetFiles(directory, filesTypes));
            }

            return listOfAllFiles;
        }
    }
}
