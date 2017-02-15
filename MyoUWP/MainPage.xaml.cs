using System;
using System.Diagnostics;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Myo_Maze_Mayhem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // private readonly global::Myo.Myo _myo;

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

    }
}
