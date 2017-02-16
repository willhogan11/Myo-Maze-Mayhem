using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using MyoUWP.Classes;

using MyoSharp.Communication;
using MyoSharp.Device;
using MyoSharp.Exceptions;
using MyoSharp.Poses;
using Windows.UI;
using System.Diagnostics;
using Windows.System;





// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MyoUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePlay : Page
    {

        IChannel _myoChannel;
        IChannel _myoChannel1;
        IHub _myoHub;
        IHub _myoHub1;

        Pose _currentPose;
        double _currentRoll;
        double _currentYaw;
        double _currentPitch;

        DispatcherTimer _orientationTimer;

        public GamePlay()
        {
            this.InitializeComponent();
            setupTimers();
            _orientationTimer.Start();

        }


        // Get key press events working first or as a backup if myo isn't available or can't connect
        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.Down)
            {
                System.Diagnostics.Debug.WriteLine("Key Down Pressed");
                eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) + 2);
            }
            if (args.VirtualKey == VirtualKey.Up)
            {
                System.Diagnostics.Debug.WriteLine("Key Up Pressed");
                eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) - 2);
            }
            if (args.VirtualKey == VirtualKey.Left)
            {
                System.Diagnostics.Debug.WriteLine("Key Left Pressed");
                eMyo.SetValue(Canvas.LeftProperty, (double)eMyo.GetValue(Canvas.LeftProperty) - 2);
            }
            if (args.VirtualKey == VirtualKey.Right)
            {
                System.Diagnostics.Debug.WriteLine("Key Right Pressed");
                eMyo.SetValue(Canvas.LeftProperty, (double)eMyo.GetValue(Canvas.LeftProperty) + 2);
            }
        }



        #region timers methods
        private void setupTimers()
        {
            if( _orientationTimer == null)
            {
                _orientationTimer = new DispatcherTimer();
                _orientationTimer.Interval = TimeSpan.FromMilliseconds(20);
                _orientationTimer.Tick += _orientationTimer_Tick;
            }
        }



        private void _orientationTimer_Tick(object sender, object e)
        {
            //if (_currentRoll < 0)
            //{   // move to the right
            //    eMyo.SetValue(Canvas.LeftProperty, (double)eMyo.GetValue(Canvas.LeftProperty) + 5);
            //}
            //else if (_currentPitch >= 0)
            //{   // Move up
            //    eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) - 5);
            //}
            //else if (_currentRoll >= 0)
            //{   // move to the left
            //    eMyo.SetValue(Canvas.LeftProperty, (double)eMyo.GetValue(Canvas.LeftProperty) - 5);
            //}
            //else if (_currentPitch < 0)
            //{   // Move down
            //    eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) + 5);
            //}


            //double x = eMyo.ActualHeight / 2;
            //double y = eMyo.ActualWidth / 2;

            //double canvasCeiling = cvsRoller.ActualHeight;

            //Debug.WriteLine("x : " + x);
            //Debug.WriteLine("y : " + y);

            //if(x >= canvasCeiling)
            //{
            //    Debug.WriteLine("Collision on Ceiling?");
            //}

        }
        #endregion




        #region Myo Setup Methods
        private void btnMyo_Click(object sender, RoutedEventArgs e)
        { // communication, device, exceptions, poses
            // create the channel
            _myoChannel = Channel.Create(ChannelDriver.Create(ChannelBridge.Create(),
                                    MyoErrorHandlerDriver.Create(MyoErrorHandlerBridge.Create())));

            // create the hub with the channel
            _myoHub = MyoSharp.Device.Hub.Create(_myoChannel);
            // create the event handlers for connect and disconnect
            _myoHub.MyoConnected += _myoHub_MyoConnected;
            _myoHub.MyoDisconnected += _myoHub_MyoDisconnected;

            // start listening 
            _myoChannel.StartListening();


            // create the channel
            _myoChannel1 = Channel.Create(ChannelDriver.Create(ChannelBridge.Create(),
                                    MyoErrorHandlerDriver.Create(MyoErrorHandlerBridge.Create())));

            // create the hub with the channel
            _myoHub1 = MyoSharp.Device.Hub.Create(_myoChannel1);
            // create the event handlers for connect and disconnect
            _myoHub1.MyoConnected += _myoHub_MyoConnected;
            _myoHub1.MyoDisconnected += _myoHub_MyoDisconnected;

            // start listening 
            _myoChannel1.StartListening();

        }

        private async void _myoHub_MyoDisconnected(object sender, MyoEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                tblUpdates.Text = tblUpdates.Text + System.Environment.NewLine +
                                    "Myo disconnected";
            });
            _myoHub.MyoConnected -= _myoHub_MyoConnected;
            _myoHub.MyoDisconnected -= _myoHub_MyoDisconnected;
            _orientationTimer.Stop();
        }

        private async void _myoHub_MyoConnected(object sender, MyoEventArgs e)
        {
            e.Myo.Vibrate(VibrationType.Long);
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                tblUpdates.Text = "Myo Connected: " + e.Myo.Handle;
            });
            // add the pose changed event here
            e.Myo.PoseChanged += Myo_PoseChanged;
            e.Myo.OrientationDataAcquired += Myo_OrientationDataAcquired;
            e.Myo.GyroscopeDataAcquired += Myo_GyroscopeDataAcquired;
            
           
            // unlock the Myo so that it doesn't keep locking between our poses
            e.Myo.Unlock(UnlockType.Hold);

            try
            {
                var sequence = PoseSequence.Create(e.Myo, Pose.FingersSpread, Pose.WaveIn);
                sequence.PoseSequenceCompleted += Sequence_PoseSequenceCompleted;

            }
            catch (Exception myoErr)
            {
                string strMsg = myoErr.Message;
            }

        }
        #endregion

        #region Gryoscope data
        private async void Myo_GyroscopeDataAcquired(object sender, GyroscopeDataEventArgs e)
        {
            
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                showGryoscopeData(e.Gyroscope.X, e.Gyroscope.Y, e.Gyroscope.Z);
            });

        }

        private void showGryoscopeData(float x, float y, float z)
        {
            var pitchDegree = (x * 180.0) / System.Math.PI;
            var yawDegree = (y * 180.0) / System.Math.PI;
            var rollDegree = (z * 180.0) / System.Math.PI;

            tblXGyro.Text = "Gyro X: " + (pitchDegree).ToString("0.00");
            tblYGyro.Text = "Gyro Y: " + (yawDegree).ToString("0.00");
            tblZGyro.Text = "Gyro R: " + (rollDegree).ToString("0.00");

        }
        #endregion

        #region Accelerometer Orientation Data
        private async void Myo_OrientationDataAcquired(object sender, OrientationDataEventArgs e)
        {
            _currentRoll = e.Roll;
            _currentYaw = e.Yaw;
            _currentPitch = e.Pitch;
            
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                showOrientationData(e.Pitch, e.Yaw, e.Roll);
            });
        }

        private void showOrientationData(double pitch, double yaw, double roll)
        {

            var pitchDegree = (pitch * 180.0) / System.Math.PI;
            var yawDegree = (yaw * 180.0) / System.Math.PI;
            var rollDegree = (roll * 180.0) / System.Math.PI;

            tblXValue.Text = "Pitch: " + (pitchDegree).ToString("0.00");
            tblYValue.Text = "Yaw: " + (yawDegree).ToString("0.00");
            tblZValue.Text = "Roll: " + (rollDegree).ToString("0.00");

            pitchLine.X2 = pitchLine.X1 + pitchDegree;
            yawLine.Y2 = yawLine.Y1 - yawDegree;
            rollLine.X2 = rollLine.X1 - rollDegree;
            rollLine.Y2 = rollLine.Y1 + rollDegree;
        }
        #endregion

        #region Pose related methods

        private async void Sequence_PoseSequenceCompleted(object sender, PoseSequenceEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                tblUpdates.Text = "Pose Sequence completed";
            });
        }

        private async void Pose_Triggered(object sender, PoseEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                tblUpdates.Text = "Pose Held: " + e.Pose.ToString();
            });

        }


        private async void Myo_PoseChanged(object sender, PoseEventArgs e)
        {
            Pose curr = e.Pose;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                tblUpdates.Text = curr.ToString();
                switch (curr)
                {
                    case Pose.Rest:
                        break;
                    case Pose.Fist:
                        // eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) + 10);            
                        break;
                    case Pose.WaveIn:
                        // eMyo.SetValue(Canvas.LeftProperty, (double)eMyo.GetValue(Canvas.LeftProperty) - 10);
                        break;
                    case Pose.WaveOut:
                        // eMyo.SetValue(Canvas.LeftProperty, (double)eMyo.GetValue(Canvas.LeftProperty) + 10);
                        break;
                    case Pose.FingersSpread:
                        // eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) - 10);
                        break;
                    case Pose.DoubleTap:
                        break;
                    case Pose.Unknown:
                        break;
                    default:
                        break;
                }
            });
        }
        #endregion


        private void startTimer()
        {
            

        }
    }
}
