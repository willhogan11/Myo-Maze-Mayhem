using MyoSharp.Communication;
using MyoSharp.Device;
using MyoSharp.Exceptions;
using MyoSharp.Poses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyoUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyoPlay : Page
    {

        // Myo Related variable Declarations
        IChannel _myoChannel;
        IChannel _myoChannel1;
        IHub _myoHub;
        IHub _myoHub1;
        double _currentRoll;
        double _currentYaw;
        double _currentPitch;
        DispatcherTimer _orientationTimer;


        // Game Related variable Declarations
        Canvas myCanvas;
        Rectangle debris;
        Rectangle ship;
        List<Rectangle> debrisArray = new List<Rectangle>();
        DispatcherTimer myStopwatchTimer;
        Stopwatch stopWatch;
        private long ms, ss, mm, hh, dd;
        private bool myoMove = true;
        // private bool keyCount = false;



        public MyoPlay()
        {
            this.InitializeComponent();
            setupTimers();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            readyText.Text = "Ready? Make a Fist to Start";

            // Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }


        private void Page_Loading(FrameworkElement sender, object args)
        {
            CreateShip();
            CreateAllDebris();
        }


        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // communication, device, exceptions, poses
                // create the channel

                _myoChannel = Channel.Create(ChannelDriver.Create(ChannelBridge.Create(),
                                      MyoErrorHandlerDriver.Create(MyoErrorHandlerBridge.Create())));

                if (_myoChannel == null)
                {
                    Frame.Navigate(typeof(Menu));
                    MessageDialog message = new MessageDialog("You can't connect to the Myo right now.\nTry Again......");
                    await message.ShowAsync();
                }
                else
                {
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
               
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                throw;
            }
        }




        private async void _myoHub_MyoDisconnected(object sender, MyoEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                tblUpdates.Text = tblUpdates.Text + System.Environment.NewLine + "Myo disconnected";
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



        #region timers methods
        private void setupTimers()
        {
            if (_orientationTimer == null)
            {
                _orientationTimer = new DispatcherTimer();
                _orientationTimer.Interval = TimeSpan.FromMilliseconds(10);
                _orientationTimer.Tick += _orientationTimer_Tick;
            }
        }



        private void _orientationTimer_Tick(object sender, object e)
        {
            if (_currentRoll < 0)
            {   // move to the right
                eMyo.SetValue(Canvas.LeftProperty, (double)eMyo.GetValue(Canvas.LeftProperty) + 2);
            }
            else if (_currentPitch >= 0)
            {   // Move up
                eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) - 2);
            }
            else if (_currentRoll >= 0)
            {   // move to the left
                eMyo.SetValue(Canvas.LeftProperty, (double)eMyo.GetValue(Canvas.LeftProperty) - 2);
            }
            else if (_currentPitch < 0)
            {   // Move down
                eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) + 2);
            }
            detectCollision(sender, e);
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
                // showOrientationData(e.Pitch, e.Yaw, e.Roll);
            });
        }

        //private void showOrientationData(double pitch, double yaw, double roll)
        //{

        //    var pitchDegree = (pitch * 180.0) / System.Math.PI;
        //    var yawDegree = (yaw * 180.0) / System.Math.PI;
        //    var rollDegree = (roll * 180.0) / System.Math.PI;

        //    tblXValue.Text = "Pitch: " + (pitchDegree).ToString("0.00");
        //    tblYValue.Text = "Yaw: " + (yawDegree).ToString("0.00");
        //    tblZValue.Text = "Roll: " + (rollDegree).ToString("0.00");

        //    pitchLine.X2 = pitchLine.X1 + pitchDegree;
        //    yawLine.Y2 = yawLine.Y1 - yawDegree;
        //    rollLine.X2 = rollLine.X1 - rollDegree;
        //    rollLine.Y2 = rollLine.Y1 + rollDegree;
        //}
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
                        if(myoMove)
                        {
                            _orientationTimer.Start();
                            myStopwatchTimer.Start();
                            stopWatch.Start();
                            readyText.Visibility = Visibility.Collapsed;
                        }          
                        break;
                    case Pose.WaveIn:
                        break;
                    case Pose.WaveOut:
                        break;
                    case Pose.FingersSpread:
                        eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) + 5);
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




        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (stopWatch == null)
            {
                stopWatch = new Stopwatch();
            }
            // check for the timer and then set up.
            if (myStopwatchTimer == null)
            {
                ms = ss = mm = hh = dd = 0;
                myStopwatchTimer = new DispatcherTimer();
                myStopwatchTimer.Tick += MyStopwatchTimer_Tick;
                myStopwatchTimer.Interval = new TimeSpan(0, 0, 0, 0, 1); // 1 millisecond
            }
            base.OnNavigatedTo(e);
        }



        private void MyStopwatchTimer_Tick(object sender, object e)
        {
            SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);

            // update the textblock with the time elapsed
            // figure out the elapsed time using the timer properties
            // some maths division and modulus
            ms = stopWatch.ElapsedMilliseconds;

            ss = ms / 1000;
            ms = ms % 1000;

            mm = ss / 60;
            ss = ss % 60;

            hh = mm % 60;
            mm = mm % 60;

            dd = hh / 24;
            hh = hh % 24;

            gameTimer.Text = mm.ToString("00") + ":" + ss.ToString("00");


            if(gameTimer.Text == "00:30")
            {
                gameTimer.Foreground = redBrush;
                gameTimer.FontSize = 35;
            }

            if (gameTimer.Text == "0:40")
            {
                winGame.Visibility = Visibility.Visible;
                gameText.Text = ("YOU RAN OUT OF TIME!");
                myoMove = false;

                myStopwatchTimer.Stop();
                stopWatch.Stop();
                _orientationTimer.Stop();

                debrisArray.Clear();
                eMyo.Fill = redBrush;

                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

                // Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
            }
        }



        private void CreateAllDebris()
        {
            int numberOfRectangles = 500;

            for (int i = 0; i < numberOfRectangles; i++)
            {
                CreateDebris();
            }
        }


        private void CreateCanvas()
        {
            if (myCanvas == null)
            {
                myCanvas = new Canvas();
                SolidColorBrush lightGrayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

                myCanvas.Background = lightGrayBrush;
                // myCanvas.Width = 800;

                layoutRoot.Children.Add(myCanvas);
            }
        }


        private void CreateShip()
        {
            if (ship == null)
            {
                SolidColorBrush blackBrush = new SolidColorBrush(Windows.UI.Colors.Black);
                ship = new Rectangle();
                ship.RadiusX = (float)Canvas.GetLeft(eMyo);
                ship.RadiusY = (float)Canvas.GetTop(eMyo);
                ship.Width = (float)eMyo.Width;
                ship.Height = (float)eMyo.Height;
                ship.Fill = blackBrush;

                cvsRoller.Children.Add(ship);
            }
        }

        



        #region
        private void CreateEllipse()
        {
            Random random = new Random();
            Ellipse ellipse = new Ellipse();

            ellipse.Width = random.Next(5, 100);
            ellipse.Height = random.Next(5, 100);

            CompositeTransform transform = new CompositeTransform();
            transform.TranslateX = random.Next(0, 720);
            transform.TranslateY = random.Next(0, 800);
            ellipse.RenderTransform = transform;

            Color color = Color.FromArgb(255, (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
            ellipse.Fill = new SolidColorBrush(color);

            myCanvas.Children.Add(ellipse);
        }
        #endregion



        private void CreateDebris()
        {
            Random random = new Random();
            debris = new Rectangle();
            SolidColorBrush brownBrush = new SolidColorBrush(Windows.UI.Colors.Brown);

            debris.Width = 25;
            debris.Height = 25;

            CompositeTransform transform = new CompositeTransform();
            transform.TranslateX = random.Next(0, 975);
            transform.TranslateY = random.Next(0, 525);

            debris.RenderTransform = transform;
            debris.RadiusX = transform.TranslateX;
            debris.RadiusY = transform.TranslateY;

            if (debris.RadiusX >= 900 && debris.RadiusY <= 65 || debris.RadiusX <= 65 && debris.RadiusY >= 460)
            {
                this.debris = null;
                Debug.WriteLine("Debris Removed from Escape Pod or Mars Base");
            }
            else
            {
                debris.Fill = brownBrush;
                debrisArray.Add(debris);
                cvsRoller.Children.Add(debris);
            }
        }


        #region KeyEvents
        // Get key press events working first or as a backup if myo isn't available or can't connect
        //private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        //{
        //    if (keyCount == false)
        //    {
        //        myStopwatchTimer.Start();
        //        stopWatch.Start();
        //        readyText.Visibility = Visibility.Collapsed;
        //    }

        //    keyCount = true;

        //    if (args.VirtualKey == VirtualKey.Down)
        //    {
        //        eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) + 2);
        //    }
        //    if (args.VirtualKey == VirtualKey.Up)
        //    {
        //        eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) - 2);
        //    }
        //    if (args.VirtualKey == VirtualKey.Left)
        //    {
        //        eMyo.SetValue(Canvas.LeftProperty, (double)eMyo.GetValue(Canvas.LeftProperty) - 2);
        //    }
        //    if (args.VirtualKey == VirtualKey.Right)
        //    {
        //        eMyo.SetValue(Canvas.LeftProperty, (double)eMyo.GetValue(Canvas.LeftProperty) + 2);
        //    }
        //    detectCollision(sender, args);
        //}
        #endregion


        private void detectCollision(object sender, object e)
        {
            SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
            SolidColorBrush whiteBrush = new SolidColorBrush(Windows.UI.Colors.White);

            ship.RadiusX = (float)Canvas.GetLeft(eMyo);
            ship.RadiusY = (float)Canvas.GetTop(eMyo);
            ship.Width = (float)eMyo.Width;
            ship.Height = (float)eMyo.Height;


            for (int i = 0; i < debrisArray.Count; i++)
            {
                if ((ship.RadiusX + ship.Width >= debrisArray[i].RadiusX) &&
                    (ship.RadiusX <= debrisArray[i].RadiusX + debrisArray[i].Width) &&
                    (ship.RadiusY + ship.Height >= debrisArray[i].RadiusY) &&
                    (ship.RadiusY <= debrisArray[i].RadiusY + debrisArray[i].Height))
                {
                    Debug.WriteLine("Collision Detected");
                    eMyo.Fill = redBrush;

                    // Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;

                    winGame.Visibility = Visibility.Visible;
                    gameText.Text = ("YOU CRASHED, GAME OVER");
                    debrisArray.Clear();

                    myoMove = false;

                    _orientationTimer.Stop();
                    myStopwatchTimer.Stop();
                    stopWatch.Stop();

                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                }
            }


            if (ship.RadiusX <= 0)
            {
                eMyo.SetValue(Canvas.LeftProperty, (double)eMyo.GetValue(Canvas.LeftProperty) + 2);
            }
            if (ship.RadiusY <= 0)
            {
                eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) + 2);
            }
            if (ship.RadiusX + eMyo.Width >= cvsRoller.Width)
            {
                eMyo.SetValue(Canvas.LeftProperty, (double)eMyo.GetValue(Canvas.LeftProperty) - 2);
            }
            if (ship.RadiusY + eMyo.Height >= cvsRoller.Height)
            {
                eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) - 2);
            }


            if (ship.RadiusX >= 930 && ship.RadiusY <= 34)
            {
                winGame.Visibility = Visibility.Visible;
                gameText.Text = "YOU REACHED THE ESCAPE POD!";

                // Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;

                myoMove = false;

                _orientationTimer.Stop();
                myStopwatchTimer.Stop();
                stopWatch.Stop();
                debrisArray.Clear();
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }

            rect1X.Text = ("Ship X : " + ship.RadiusX.ToString());
            rect1Y.Text = ("Ship Y : " + ship.RadiusY.ToString());
        }
    }
}
