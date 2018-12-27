using AutoRotate.Image.Logic;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace AutoRotate.Image.Console
{
    class Program
    {
        private static IImageRotateLogic _ImageLogic;

        // TODO handle if the given path is a picture instead of a directory 
        // make testable 
        // move defintion of the supported image types to app.config like -> <add key="ClientsFilePath" value="filepath"/>

        static void Main(string[] args)
        {
            System.Console.WriteLine("Rotate of pictures starts .... " + DateTime.Now.ToString());
            if (args == null || args.Length == 0)
            {
                System.Console.WriteLine("No parameters are set");
            }

            var path = args.FirstOrDefault<string>();

            if (! System.IO.Directory.Exists(path))
            {
                System.Console.WriteLine("No give directory does not exist " + path);
                System.Console.ReadLine();
                return;
            }

            // TODO add check 
            var fileTypes = ConfigurationManager.AppSettings["File.Types"].Split(';').ToList();
            int countOfTaskToCreate = 1;
            int.TryParse(ConfigurationManager.AppSettings["Task.CountOfParallelTasks"], out countOfTaskToCreate);

            
            _ImageLogic = new ImageRotateLogic();
            //_ImageLogic.RotateImages(path, fileTypes);

            using(var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromMinutes(20)))
            {
                //_ImageLogic.RotateImagesParallel(path, fileTypes, cts.Token);

                // run with defined task count 
                _ImageLogic.RotateImagesParallel(path, countOfTaskToCreate, fileTypes, cts.Token);
            }

            System.Console.WriteLine("Rotate of the give directory '" + path + "'was successful. "+ DateTime.Now.ToString());
            System.Console.WriteLine("Press any key to exit ..." );
            System.Console.ReadLine();
        }
    }
}
