using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Drawing_Mistakes_Detection
{
    public partial class App : Application
    {
        public App()
        {
            var stack = new StackLayout();
            MainPage = new ContentPage();
            (MainPage as ContentPage).Content = stack;

            // Create a button for picking a drawing
            Button pickPictureButton = new Button
            {
                Text = "Pick Drawing",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            stack.Children.Add(pickPictureButton);
            // Initiate an image-picking on click
            pickPictureButton.Clicked += async (sender, e) =>
            {
                // Clear the content for new image
                stack.Children.Clear();

                Stream stream = await DependencyService.Get<IImagePicker>().GetImageStreamAsync();
                if (stream != null)
                {
                    // Show the selected image
                    Image image = new Image
                    {
                        Source = ImageSource.FromStream(() => stream),
                        BackgroundColor = Color.Gray
                    };
                    stack.Children.Add(image);
                    // Request the image analysis 
                    
                }

                // Add the button to the content stack, so can select other image
                stack.Children.Add(pickPictureButton);
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        /// <summary>
        /// Returns a byte array gotten from a Stream.
        /// </summary>
        /// <param name="s"> The Stream to be turned into a byte array.</param>
        /// <returns>The byte array gotten from a Stream</returns>
        static byte[] StreamToArray(Stream s)
        {
            MemoryStream ms = new MemoryStream();
            s.CopyTo(ms);
            return ms.ToArray();
        }

        /// <summary>
        /// Makes a request to the Custom Vision Service API for predicting tags of an image.
        /// </summary>
        /// <param name="imageStream">The source Stream of the image.</param>
        /// <returns>The map of predictions, if successful, or null otherwise.</returns>
        static async Task<string> MakePredictionRequest(Stream imageStream)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-Key", );

            string url = ;

            HttpResponseMessage response;

            // Request body
            byte[] imageByteArray = StreamToArray(imageStream);

            using (var content = new ByteArrayContent(imageByteArray))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
