using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyoUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scores : Page
    {
        public Scores()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var scoresFolder = await folder.CreateFolderAsync("ScoresFolder", CreationCollisionOption.OpenIfExists);

            var files = await scoresFolder.GetFilesAsync();
            var desiredFile = files.FirstOrDefault(x => x.Name == "scores.txt");
            var textContent = await FileIO.ReadTextAsync(desiredFile);

            Debug.WriteLine(textContent);

            scoresText.Text = textContent;
        }
    }
}
