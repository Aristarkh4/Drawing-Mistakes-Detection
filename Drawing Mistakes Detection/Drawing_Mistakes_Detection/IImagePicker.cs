using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drawing_Mistakes_Detection
{
    public interface IImagePicker
    {
        Task<Stream> GetImageStreamAsync();
    }
}
