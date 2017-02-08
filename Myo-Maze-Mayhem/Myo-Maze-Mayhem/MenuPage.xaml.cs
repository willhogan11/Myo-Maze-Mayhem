using Myo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Myo_Maze_Mayhem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MenuPage : Page
    {
        private readonly global::Myo.Myo _myo;

        public MenuPage()
        {
            this.InitializeComponent();
            // Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown; // For adding event handlers
            _myo = new global::Myo.Myo(); //  Initialise

            _myo.Connect();
            _myo.Vibrate();

            // Add event handlers
            _myo.OnPoseDetected += _myo_OnPoseDetected;

        }



        private void _myo_OnPoseDetected(object sender, MyoPoseEventArgs e)
        {
            switch (e.Pose)
            {
                case MyoPoseEventArgs.PoseType.Rest:
                    Debug.WriteLine("Rest Gesture made...");
                    break;
                case MyoPoseEventArgs.PoseType.Fist:
                    Debug.WriteLine("Fist Gesture made...");
                    break;
                case MyoPoseEventArgs.PoseType.WaveIn:
                    Debug.WriteLine("Wave in Gesture made...");

                    // Issue here, not working...
                    myRectangle.SetValue(Canvas.LeftProperty, (double)myRectangle.GetValue(Canvas.LeftProperty) - 1);
                    break;
                case MyoPoseEventArgs.PoseType.WaveOut:
                    Debug.WriteLine("Wave out Gesture made...");
                    break;
                case MyoPoseEventArgs.PoseType.DoubleTap:
                    Debug.WriteLine("Double Tap Gesture made...");
                    break;
                case MyoPoseEventArgs.PoseType.FingersSpread:
                    Debug.WriteLine("Fingers Spread Gesture made...");
                    break;
                default:
                    break;
            }
        }



        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.Down)
            {
                System.Diagnostics.Debug.WriteLine("Key Down Pressed");
                myRectangle.SetValue(Canvas.TopProperty, (double)myRectangle.GetValue(Canvas.TopProperty) + 5);
            }
            if (args.VirtualKey == VirtualKey.Up)
            {
                System.Diagnostics.Debug.WriteLine("Key U5p Pressed");
                myRectangle.SetValue(Canvas.TopProperty, (double)myRectangle.GetValue(Canvas.TopProperty) - 5);
            }
            if (args.VirtualKey == VirtualKey.Left)
            {
                System.Diagnostics.Debug.WriteLine("Key Left Pressed");
                myRectangle.SetValue(Canvas.LeftProperty, (double)myRectangle.GetValue(Canvas.LeftProperty) - 5);
            }
            if (args.VirtualKey == VirtualKey.Right)
            {
                System.Diagnostics.Debug.WriteLine("Key Right Pressed");
                myRectangle.SetValue(Canvas.LeftProperty, (double)myRectangle.GetValue(Canvas.LeftProperty) + 5);
            }
            if ((args.VirtualKey == VirtualKey.Down) && (args.VirtualKey == VirtualKey.Left))
            {
                myRectangle.SetValue(Canvas.TopProperty, (double)myRectangle.GetValue(Canvas.TopProperty) + 1);
                myRectangle.SetValue(Canvas.LeftProperty, (double)myRectangle.GetValue(Canvas.LeftProperty) - 1);
            }
        }
    }
}
