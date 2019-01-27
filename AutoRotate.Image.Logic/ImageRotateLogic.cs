using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
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

        /// <summary>
        /// Will rotate the images in parallel, you can decide how many task should run in parallel 
        /// </summary>
        /// <param name="directoryName">path of the directory</param>
        /// <param name="taskCount">count of the task which should run in prallel</param>
        /// <param name="fileFilterOptions">which file type (ending of file e.g. *.jpg) should rotated</param>
        /// <param name="token">Cancellation token</param>
        public void RotateImagesParallel(string directoryName, int taskCount, IReadOnlyCollection<string> fileFilterOptions, CancellationToken token)
        {
            var allFilesInDirectory = LoadAllFilesWithFiterOptions(directoryName, fileFilterOptions).ToList();

            var filesPerTask = SplitFilesForEachTask(taskCount, allFilesInDirectory);

            var taskPool = new List<Task>();

            foreach (var files in filesPerTask)
            {
                var singleTask = Task.Factory.StartNew(() => RotateMulitpleImages(files));
                taskPool.Add(singleTask);
            }

            Task.WaitAll(taskPool.ToArray(), token);
        }

        //TODO move this function to a class e.g. TaskManager -> inject via interface 
        private List<List<string>> SplitFilesForEachTask(int taskCount, List<string> allFilesInDirectory)
        {
            int sizeOfEachSplitList = allFilesInDirectory.Count / taskCount;

            var listOfLists = new List<List<string>>();

            for(int i = 0; i < taskCount; i++)
            {
                var newList = new List<string>();

                newList.AddRange(allFilesInDirectory.GetRange(i * sizeOfEachSplitList, sizeOfEachSplitList +1 ));
                listOfLists.Add(newList);    
            }
            
            return listOfLists;
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
            Rotation rotation;
            using(var bmp = new Bitmap(itemFilePath))
            {
                rotation = ImageExtensions.GetRotation(bmp);
            }

            RotateJpeg(itemFilePath, rotation);
        }

        private void RotateJpeg(string filePath, Rotation rotation,  int quality = 100)
        {
            var original = new FileInfo(filePath);
            var temp = new FileInfo(original.FullName.Replace(".", "_temp."));

            const BitmapCreateOptions createOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile;

            try
            {
                using (Stream originalFileStream = File.Open(original.FullName, FileMode.Open, FileAccess.Read))
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder { QualityLevel = quality, Rotation = rotation };

                    //BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile and BitmapCacheOption.None
                    //is a KEY to lossless jpeg edit if the QualityLevel is the same
                    encoder.Frames.Add(BitmapFrame.Create(originalFileStream, createOptions, BitmapCacheOption.None));

                    using (Stream newFileStream = File.Open(temp.FullName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        encoder.Save(newFileStream);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            try
            {
                temp.CreationTime = original.CreationTime;

                original.Delete();
                temp.MoveTo(original.FullName);
            }
            catch (Exception)
            {
                throw;
            }

            return;
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
