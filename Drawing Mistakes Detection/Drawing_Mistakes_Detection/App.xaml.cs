using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
// TODO Remove after debug.
using System.Diagnostics;

using Xamarin.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Drawing_Mistakes_Detection
{
    public partial class App : Application
    {
        static Dictionary<string, byte> TagNameToTagId = InitializeTagNameToTagId();
        static Dictionary<byte, string> TagIdToTagName = InitializeTagIdToTagName();

        public App()
        {
            var dataService = new AzureDataService();

            // Orginising the layout:
            // MainPage -> ScrollView view -> StackLayout stack.
            var stack = new StackLayout();
            var view = new ScrollView{ Content = stack };
            MainPage = new ContentPage{ Content = view };

            // Create a button for picking a drawing.
            Button pickPictureButton = new Button
            {
                Text = "Pick Drawing",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            stack.Children.Add(pickPictureButton);

            //Create a button for accessing past tags for images
            Button showHistoryButton = new Button
            {
                Text = "Show History",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            stack.Children.Add(showHistoryButton);

            // Initialize showing history on button
            showHistoryButton.Clicked += async (sender, e) =>
            {
                // Clear the content for history.
                stack.Children.Clear();

                var pastTags = await dataService.GetDrawingsWithTags();
                var historyLabel = new Label { Text = "Empty", HorizontalTextAlignment = TextAlignment.Center };
                
                
                if (pastTags != null)
                {
                    historyLabel.Text = "";
                    foreach (DrawingWithTag drawingTag in pastTags)
                    {
                        if (TagIdToTagName.ContainsKey(drawingTag.TagId))
                        {
                            string tag = TagIdToTagName[drawingTag.TagId];
                            historyLabel.Text += "\n" + tag;
                        }
                    }
                }
                stack.Children.Add(historyLabel);

                // Add the button to the content stack, so can select other image.
                stack.Children.Add(pickPictureButton);
                stack.Children.Add(showHistoryButton);
            };

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
                    if(predictionJSON == null)
                    {
                        var errorLabel = new Label { Text = "Error sending a request. Please check your connection.",
                                                     HorizontalTextAlignment = TextAlignment.Center};
                        stack.Children.Add(errorLabel);
                    } else
                    {
                        PredictionResult predictionResult = JsonConvert.DeserializeObject<PredictionResult>(predictionJSON);
                        // Put the most probable tags on label and add it to the page
                        string[] bestPredictionTags = predictionResult.GetBestPredictions();
                        var predictionLabel = new Label { Text = "Unclear" , HorizontalTextAlignment = TextAlignment.Center };
                        if (bestPredictionTags.Length != 0)
                        {
                            predictionLabel.Text = "";
                            foreach (string tag in bestPredictionTags)
                            {
                                predictionLabel.Text += "\n" + tag;
                            }
                        }
                        stack.Children.Add(predictionLabel);

                        //Create a button for accessing past tags for images
                        Button addToHistoryButton = new Button
                        {
                            Text = "Add Tags To History",
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.CenterAndExpand
                        };
                        stack.Children.Add(addToHistoryButton);
                        addToHistoryButton.Clicked += async (sender_, e_) =>
                        {
                            byte[] tagIds = new byte[bestPredictionTags.Length];
                            for(int i = 0; i<tagIds.Length; i++)
                            {
                                string tag = bestPredictionTags[i];
                                if (TagNameToTagId.ContainsKey(tag))
                                {
                                    tagIds[i] = TagNameToTagId[tag];   
                                }
                            }
                            await dataService.AddDrawingWithTag(tagIds);
                        };
                    }    
                }

                // Add the button to the content stack, so can select other image.
                stack.Children.Add(pickPictureButton);
                stack.Children.Add(showHistoryButton);
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
            try
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
            catch (HttpRequestException e)
            {
                return null;
            }
        }

        private static Dictionary<string, byte> InitializeTagNameToTagId()
        {
            var dict = new Dictionary<string, byte>();
            dict.Add("Bad anatomy", 1);
            dict.Add("Bad anatomy (humans)", 2);
            dict.Add("Bad perspective", 3);
            dict.Add("Dirty colors", 4);
            dict.Add("Relatively good", 5);
            dict.Add("Shaky lines", 6);
            dict.Add("Slopy shading", 7);
            dict.Add("Unclear", 8);
            return dict;
        }

        private static Dictionary<byte, string> InitializeTagIdToTagName()
        {
            var dict = new Dictionary<byte, string>();
            dict.Add(1, "Bad anatomy");
            dict.Add(2, "Bad anatomy (humans)");
            dict.Add(3, "Bad perspective");
            dict.Add(4, "Dirty colors");
            dict.Add(5, "Relatively good");
            dict.Add(6, "Shaky lines");
            dict.Add(7, "Slopy shading");
            dict.Add(8, "Unclear");
            return dict;
        }
    }
}
