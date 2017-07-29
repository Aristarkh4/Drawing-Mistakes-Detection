using System;
using System.IO;
using System.Threading.Tasks;

using Android.Content;

using Xamarin.Forms;

using Drawing_Mistakes_Detection.Droid;

[assembly: Dependency(typeof(ImagePicker))]

namespace Drawing_Mistakes_Detection.Droid
{
    class ImagePicker : IImagePicker
    {
        public Task<Stream> GetImageStreamAsync()
        {
            // Create an intent for getting images
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);

            // Get the MainActivity instance
            MainActivity activity = Forms.Context as MainActivity;

            // Start actvity with image-picking intent (continues in MainActivity.cs)
            activity.StartActivityForResult(
                Intent.CreateChooser(intent, "Select Drawing"),
                MainActivity.PickImageId);

            // Return Task object
            activity.PickImageTaskCompletionSource = new TaskCompletionSource<Stream>();
            return activity.PickImageTaskCompletionSource.Task;
        }
    }
}