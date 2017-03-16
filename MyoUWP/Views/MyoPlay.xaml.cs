using MyoSharp.Communication;
using MyoSharp.Device;
using MyoSharp.Exceptions;
using MyoSharp.Poses;
using MyoUWP.Classes;
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
        GameObjects gameObjects;
        List<Rectangle> debrisArray = new List<Rectangle>();
        List<string> levelTimes;
        List<string> gameNameScores;
        DispatcherTimer myStopwatchTimer;
        Stopwatch stopWatch;

        private long ms, ss, mm, hh, dd;
        private bool myoMove = true;
        string easyLevel;
        string mediumLevel;
        string hardLevel;


        // Constructor to init game
        public MyoPlay()
        {
            this.InitializeComponent();
            setupTimers();
            PrepareGameData();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }


        /* On Page load event:
         * Create a New GameObjects class instance
         * Assign the in game ship instance to a gameObjects ship creation
         * Create the Debris objects
         */
        private void Page_Loading(FrameworkElement sender, object args)
        {
            gameObjects = new GameObjects();
            ship = gameObjects.CreateShip(ship, eMyo, cvsRoller);
            CreateAllDebris();
        }


        // Add the various Game level time limits to a List called levelTimes
        private void PrepareGameData()
        {
            levelTimes = new List<string>();
            levelTimes.Add("01:30");
            levelTimes.Add("01:00");
            levelTimes.Add("00:30");
        }


        // Function that makes a connection to the Myo Armband
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


        
        // Function that disconnects from the Myo Armband
        // When disconnected Stop all timers
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


        // Function to detail what happens when connected to the Myo Armband
        // Displays Orientation data on a TextBlock, along with Pose information
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



        // Function that sets up a Dispatch Timer associated with the Myo Armband
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


        // Function that deals with What happens when the certain gestures / poses are made using the Myo Armband
        // Detect for collision when during movement
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




        // Displays Accelerometer data to screen
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

        #endregion



        // A function that when called, displays which pose has just been completed and updates Textblock in real time
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



        // A function that deals what happens when gestures are made using the Myo Armband
        // The only ones relevant are Fist and Fingers spread gestures
        // Fist starts game and relevant timers
        // Fingers spread gesture moves ship downwards a specified number of pixels each time the gesture is made
        private async void Myo_PoseChanged(object sender, PoseEventArgs e)
        {
            Pose curr = e.Pose;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                tblUpdates.Text = curr.ToString();
                switch (curr)
                {
                    case Pose.Fist:
                        if(myoMove)
                        {
                            _orientationTimer.Start();
                            myStopwatchTimer.Start();
                            stopWatch.Start();
                            readyText.Visibility = Visibility.Collapsed;
                        }          
                        break;
                    case Pose.FingersSpread:
                        eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) + 5);
                        break;
                    default:
                        break;
                }
            });
        }
        #endregion



        // Setup a game timer......
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



        // Setup a stopwatch timer that updates the Textblock with real time data
        // Update the textblock with the time elapsed
        // figure out the elapsed time using the timer properties
        private void MyStopwatchTimer_Tick(object sender, object e)
        {
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

            GameTimeRanOut();     
        }



        private void GameTimeRanOut()
        {
            SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);

            if ((gameTimer.Text == "00:20" && (bool)hard.IsChecked) ||
                 (gameTimer.Text == "00:50" && (bool)medium.IsChecked) ||
                 (gameTimer.Text == "01:20" && (bool)easy.IsChecked))
            {
                gameTimer.Foreground = redBrush;
                gameTimer.FontSize = 35;
            }

            if ((gameTimer.Text == levelTimes[2].ToString() && (bool)hard.IsChecked) ||
                 (gameTimer.Text == levelTimes[1].ToString() && (bool)medium.IsChecked) ||
                 (gameTimer.Text == levelTimes[0].ToString() && (bool)easy.IsChecked))
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
            }
        }



        // Create all Debris objects Function
        // Create an amount of debris objects to be created
        private void CreateAllDebris()
        {
            int numberOfRectangles = 800;

            for (int i = 0; i < numberOfRectangles; i++)
            {
                gameObjects.CreateDebris(debris, debrisArray, cvsRoller);
            }
        }




        // Create an Ellipse object
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



        /* A function to keep trakc of game timer
         * Steps:
         * If the timer is 10 seconds away from a particular difficulty level limit, set timer text to red
         * If the timer reaches the level limit, end the game
         * */
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

            CanvasEdgeDetection();
            WinGame();

            rect1X.Text = ("Rover X : " + ship.RadiusX.ToString());
            rect1Y.Text = ("Rover Y : " + ship.RadiusY.ToString());
        }



        // A function that fires when a user reaches the escape pod
        // Reset the various game variables and make the back button visible
        private void WinGame()
        {
            if (ship.RadiusX >= 930 && ship.RadiusY <= 34)
            {
                winGame.Visibility = Visibility.Visible;
                gameText.Text = "YOU REACHED THE ESCAPE POD!";

                myoMove = false;

                _orientationTimer.Stop();
                myStopwatchTimer.Stop();
                stopWatch.Stop();
                debrisArray.Clear();
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

                enterName.Visibility = Visibility.Visible;
            }
        }


        // A function that checks if the ship has reached the canvas edge
        // Repaint the ship at the edge should it collide with the canvas edge
        private void CanvasEdgeDetection()
        {
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
        }


        // For debug purposes
        private void dbg(string str)
        {
            Debug.WriteLine(str);
        }



        /* A function that deals with which game difficulty is chosen
         * When a difficulty has been picked, start the game when a key event is triggered */
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)easy.IsChecked)
            {
                easyLevel = levelTimes[0];
                difficultyInfo.Text = "Easy";
                Debug.WriteLine("Easy Was checked " + easyLevel);
            }
            else if ((bool)medium.IsChecked)
            {
                mediumLevel = levelTimes[1];
                difficultyInfo.Text = "Med";
                Debug.WriteLine("Medium Was checked " + mediumLevel);
            }
            else if ((bool)hard.IsChecked)
            {
                hardLevel = levelTimes[2];
                difficultyInfo.Text = "Hard";
                Debug.WriteLine("Hard Was checked " + hardLevel);
            }
            difficultyStPanel.Visibility = Visibility.Collapsed;
            readyText.Text = "Ready? Make a Fist to Start";
        }



        /* A function that allows a user to enter their name for the scores page, should they complete the game
         * Steps:
         * Create a new instance of the ScoresStorage class
         * Get a hold of the finished state of the game, ie time completed in, game difficulty etc
         * Write or appends the score to the Write method of the ScoredStorage class instance */
        private void EnterName_Click(object sender, RoutedEventArgs e)
        {
            ScoresStorage scoreStorage = new ScoresStorage();

            gameNameScores = new List<string>();
            string gameType = "Myo Play";
            string finishedState = name.Text + "\t\t" + gameTimer.Text + "\t\t" + difficultyInfo.Text + "\t\t" + gameType;

            gameNameScores.Add(finishedState);

            scoreStorage.WriteScoreToFile(finishedState);

            enterName.Visibility = Visibility.Collapsed;
        }
    }
}
