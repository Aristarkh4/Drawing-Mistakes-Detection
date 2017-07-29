using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
// TODO Remove after debug.
using System.Diagnostics;

using Xamarin.Forms;
using Newtonsoft.Json;

namespace Drawing_Mistakes_Detection
{
    public partial class App : Application
    {
        public App()
        {   
            // Orginising the layout:
            // MainPage -> ScrollView view -> StackLayout stack.
            var stack = new StackLayout();
            var view = new ScrollView{ Content = stack };
            MainPage = new ContentPage{ Content = view };

            // Create a button for picking a drawing.
            Button pickPictureButton = new Button
            {
                Text = "Pick Drawing",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            stack.Children.Add(pickPictureButton);
            // Initiate an image-picking on click.
            pickPictureButton.Clicked += async (sender, e) =>
            {
                // Clear the content for new image.
                stack.Children.Clear();

                Stream imageStream = await DependencyService.Get<IImagePicker>().GetImageStreamAsync();

                if (imageStream != null)
                {
                    // Show the selected image.
                    Image image = new Image{
                        Source = ImageSource.FromStream(() => imageStream),
                        BackgroundColor = Color.Gray };
                    stack.Children.Add(image);
                    // Request the image analysis. 
                    String predictionJSON = await MakePredictionRequest(imageStream);
                    PredictionResult predictionResult = JsonConvert.DeserializeObject<PredictionResult>(predictionJSON);
                    // Put the most probable tags on label and add it to the page
                    string[] bestPredictionTags = predictionResult.GetBestPredictions();
                    var predictionLabel = new Label { Text = "Unclear" };
                    if(bestPredictionTags.Length != 0)
                    {
                        predictionLabel.Text = "";
                        foreach(string tag in bestPredictionTags)
                        {
                            predictionLabel.Text += "\n" + tag;
                        }
                    }
                    stack.Children.Add(predictionLabel);
                }

                // Add the button to the content stack, so can select other image.
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
        private static byte[] StreamToArray(Stream s)
        {
            MemoryStream ms = new MemoryStream();
            s.CopyTo(ms);
            // Reset stream.
            s.Position = 0;
            return ms.ToArray();
        }

        /// <summary>
        /// Makes a request to the Custom Vision Service API for predicting tags of an image.
        /// </summary>
        /// <param name="imageStream">The source Stream of the image.</param>
        /// <returns>The JSON map of predictions, if successful, or null otherwise.</returns>
        private static async Task<string> MakePredictionRequest(Stream imageStream)
        {   
            byte[] imageByteArray = StreamToArray(imageStream);

            var client = new HttpClient();
            // APIKeys is a static class static string field for prediction key
            // and static string field for prediction url.
            client.DefaultRequestHeaders.Add("Prediction-Key", APIKeys.PredictionKey); 
            string url = APIKeys.PredictionURL;                                        

            HttpResponseMessage response;
            using (var content = new ByteArrayContent(imageByteArray))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
