using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drawing_Mistakes_Detection
{
    /// <summary>
    /// A tool to select an image from the device file system.
    /// </summary>
    public interface IImagePicker
    {
        /// <summary>
        /// Gets a Stream of a selected image.
        /// </summary>
        /// <returns>A Stream of a selected image.</returns>
        Task<Stream> GetImageStreamAsync();
    }
}
