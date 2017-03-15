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
        // Create an instance of the ScoresStorage class
        ScoresStorage scoresStorage;

        public Scores()
        {
            this.InitializeComponent();
        }

        // On page load, initialise the scoresStorage class and call the function that passes in the Textblock text and reads the scores out to the page
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            scoresStorage = new ScoresStorage();
            scoresStorage.ReadScoresFromFile(scoresText);
        }
    }
}
