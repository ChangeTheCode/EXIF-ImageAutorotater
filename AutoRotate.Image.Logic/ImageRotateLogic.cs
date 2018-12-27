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
