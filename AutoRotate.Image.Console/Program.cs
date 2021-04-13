using AutoRotate.Image.Logic;
using System;
using System.Configuration;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AutoRotate.Image.Console
{
    class Program
    {
        private const string ON = "hide";
        private const string AN = "ausblenden";
        private static string _CurrentConsoleHiddenState;
        private static bool _PrintConsole = true;
        private static IImageRotateLogic _ImageLogic;

        // TODO handle if the given path is a picture instead of a directory 
        // make testable 
        // Create a tool to set the run setup like, which files, how many task, and so one 
        // move defintion of the supported image types to app.config like -> <add key="ClientsFilePath" value="filepath"/>

        // imports to handle hidden or display console
        [DllImport("user32.dll")]
        private static extern int ShowWindow(int Handle, int showState);
        [DllImport("kernel32.dll")]
        public static extern int GetConsoleWindow();
        
        static void Main(string[] args)
        {
            int win = GetConsoleWindow();
            _CurrentConsoleHiddenState = ConfigurationManager.AppSettings["Console.HideState"];
            _PrintConsole = UseConsoleOutput();
            var startTime = DateTime.Now;

            ShowWindow(win, HideConsole(_CurrentConsoleHiddenState));
            if (_PrintConsole)
            {
                System.Console.WriteLine("Rotate of pictures starts .... ");
                if (args == null || args.Length == 0)
                {
                    System.Console.WriteLine("No parameters are set");
                }
            }

            var path = args.FirstOrDefault<string>();

            if (!Directory.Exists(path))
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

            if (_PrintConsole)
            {
                System.Console.WriteLine("Press any key to exit ...");
            }
            
            using(var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromMinutes(20)))
            {
                //_ImageLogic.RotateImagesParallel(path, fileTypes, cts.Token);
                
                // run each rotate sqeunzell
                _ImageLogic.RotateImages(path, fileTypes); 

                // run with defined task count 
                //_ImageLogic.RotateImagesParallel(path, countOfTaskToCreate, fileTypes, cts.Token);
            }

            if (_PrintConsole)
            {
                System.Console.WriteLine("Rotate of the give directory '" + path + "'was successful. Used time: " + (DateTime.Now - startTime).ToString());
                System.Console.WriteLine("Press any key to exit ...");
                System.Console.ReadLine();
            }
            else
            {
                MessageBox.Show("Rotate of the give directory '" + path + "'was successful. Used time: " + (DateTime.Now - startTime).ToString());
            }
        }

        private static int HideConsole(string hideValue)
        {
            if (string.Equals( ON, hideValue, StringComparison.CurrentCultureIgnoreCase) ||
                string.Equals(ON, hideValue, StringComparison.CurrentCultureIgnoreCase))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        private static bool UseConsoleOutput()
        {
            if (string.Equals(ON, _CurrentConsoleHiddenState, StringComparison.CurrentCultureIgnoreCase) ||
                string.Equals(AN, _CurrentConsoleHiddenState, StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
