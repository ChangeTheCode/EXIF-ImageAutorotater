using AutoRotate.Image.Logic;
using System;
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

            // TODO move to app.config 
            var fileTypes = new List<string>{"*.jpg", "*.png", "*.jpge"};
            
            _ImageLogic = new ImageRotateLogic();
            _ImageLogic.RotateImages(path, fileTypes);

            System.Console.WriteLine("Rotate of the give directory '" + path + "'was successful. "+ DateTime.Now.ToString());
            System.Console.WriteLine("Press any key to exit ..." );
            System.Console.ReadLine();
        }
    }
}
