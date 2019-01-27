using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoRotate.Image.Console
{
    internal interface IConfigurationManager
    {
        void SetConfig(string key, string value);

        string GetConfig(string key);
    }
}
