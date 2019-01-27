using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoRotate.Image.Logic
{
    public interface IImageRotateLogic
    {
        void RotateImages(string directoryName, IReadOnlyCollection<string> fileFilterOptions);

        void RotateImagesParallelAsync(string directoryName, IReadOnlyCollection<string> fileFilterOptions, CancellationToken token);
    }
}
