using System;
using System.Diagnostics;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Myo_Maze_Mayhem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly global::Myo.Myo _myo;

        public MainPage()
        {
            this.InitializeComponent();
            _myo = new global::Myo.Myo(); //  Initialise

            // Add event handlers
            //_myo.OnPoseDetected += _myo_OnPoseDetected;
            //_myo.OnEMGAvailable += _myo_OnEMGAvailable;
            //_myo.DataAvailable += _myo_DataAvailable;
        }

        // Start an animation that fades in and out on the "Tap to Begin" textBlock when the object loads
        // Adapted from[ref] https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.media.animation.doubleanimation.aspx
        private void Start_Animation(object sender, RoutedEventArgs e)
        {
            myStoryboard.Begin();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _myo.Connect();
                _myo.Vibrate();

                Debug.WriteLine("Myo Successfully Connected...");

                MessageDialog message = new MessageDialog("Myo Successfully Connected...");
                await message.ShowAsync();

                startButton.Visibility = Visibility.Visible;
            }
            catch (Exception)
            {
                Debug.WriteLine("Issue connecting your Myo,\nTry Again...");
                MessageDialog message = new MessageDialog("Issue connecting your Myo,\nTry Again...");
                await message.ShowAsync();
                throw;
            }
        }
    }
}
