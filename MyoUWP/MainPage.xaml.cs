using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyoUWP;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Myo_Maze_Mayhem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
        }
      

        // Start an animation that fades in and out on the "Tap to Begin" textBlock when the object loads
        // Adapted from[ref] https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.media.animation.doubleanimation.aspx
        private void Start_Animation(object sender, RoutedEventArgs e)
        {
            myStoryboard.Begin();
        }


        // Navigates to the Menu page, when button is clicked
        private void begin_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Menu));
        }
    }
}
