using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
                pickPictureButton.IsEnabled = false;
                Stream stream = await DependencyService.Get<IImagePicker>().GetImageStreamAsync();

                if (stream != null)
                {
                    Image image = new Image
                    {
                        Source = ImageSource.FromStream(() => stream),
                        BackgroundColor = Color.Gray
                    };

                    TapGestureRecognizer recognizer = new TapGestureRecognizer();
                    recognizer.Tapped += (sender2, args) =>
                    {
                        (MainPage as ContentPage).Content = stack;
                        pickPictureButton.IsEnabled = true;
                    };
                    image.GestureRecognizers.Add(recognizer);

                    (MainPage as ContentPage).Content = image;
                }
                else
                {
                    pickPictureButton.IsEnabled = true;
                }
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
    }
}
