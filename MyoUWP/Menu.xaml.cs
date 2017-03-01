using System;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyoUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Menu : Page
    {
        public Menu()
        {
            this.InitializeComponent();
        }


        // Displays the System back button's Visibility to Visible
        // If there is a page to go back to, then navigate back through the stack
        // If not then the user is at the main first page. 
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
                SystemNavigationManager.GetForCurrentView().BackRequested += Menu_BackRequested;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }
        }



        private void Menu_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;

            // Navigate back if possible, and if the event has not already been handled...
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }


        // Start an animation that fades in and out on the "Tap to Begin" textBlock when the object loads
        // Adapted from[ref] https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.media.animation.doubleanimation.aspx
        private void Start_Animation(object sender, RoutedEventArgs e)
        {
            myStoryboard.Begin();
            myStoryboard1.Begin();
            myStoryboard2.Begin();
        }


        private void connectMyoButton_Click(object sender, RoutedEventArgs e)
        {
            
        }


        private async void startButton_Click(object sender, RoutedEventArgs e)
        {
            loadingText.Text += "LOADING GAME\nPLEASE WAIT.....";
            MessageDialog message = new MessageDialog("LOADING GAME\nPLEASE WAIT.....");
            await message.ShowAsync();

            // Frame.Navigate(typeof(Test));
            Frame.Navigate(typeof(GamePlay));
        }


        private void howToPlayButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HowToPlay));
        }
    }
}
