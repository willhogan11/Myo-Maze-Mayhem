using MyoUWP.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
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
    public sealed partial class KeyPlay : Page
    {
        // Instance variable declaration
        Rectangle debris;
        Rectangle ship;
        List<Rectangle> debrisArray = new List<Rectangle>();
        List<string> levelTimes;
        List<string> gameNameScores;
        DispatcherTimer myStopwatchTimer;
        Stopwatch stopWatch;
        GameObjects gameObjects;

        private long ms, ss, mm, hh, dd;
        private Boolean keyCount = false;
        string easyLevel;
        string mediumLevel;
        string hardLevel;


        // Constructor to init game
        public KeyPlay()
        {
            this.InitializeComponent();            
            PrepareGameData();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }


        /* On Page load event:
         * Create a New GameObjects class instance
         * Assign the in game ship instance to a gameObjects ship creation
         * Create the Debris objects
         *  */
        private void Page_Loaded(object sender, RoutedEventArgs e)
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


        // Create all Debris objects Function
        // Create an amount of debris objects to be created
        private void CreateAllDebris()
        {
            int numberOfRectangles = 900;

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
            // myCanvas.Children.Add(ellipse);
        }




        /* This function uses key events to control the ship:
         * Check if there has been any key pressed, if not start game timer to begin game
         * Set keyCount to true
         * Register Up, Down, Left and Right key events when  pressed
         * Call the DetectCollision function to check if each or any movement collides with an object
         *  */
        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            if (keyCount == false)
            {
                myStopwatchTimer.Start();
                stopWatch.Start();
                readyText.Visibility = Visibility.Collapsed;
            }

            keyCount = true;

            if (args.VirtualKey == VirtualKey.Down)
            {
                eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) + 2);
            }
            if (args.VirtualKey == VirtualKey.Up)
            {
                eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) - 2);
            }
            if (args.VirtualKey == VirtualKey.Left)
            {
                eMyo.SetValue(Canvas.LeftProperty, (double)eMyo.GetValue(Canvas.LeftProperty) - 2);
            }
            if (args.VirtualKey == VirtualKey.Right)
            {
                eMyo.SetValue(Canvas.LeftProperty, (double)eMyo.GetValue(Canvas.LeftProperty) + 2);
            }
            detectCollision(sender, args);
        }



        // A function for Debug
        private void dbg(string str, Object strOpt = null)
        {
            Debug.WriteLine(str);
        }


        #region HowGameCanFinish
        /* This function detects collisions between the ship and debris objects. 
         * Steps:
         * Get the ships X and Y coordinates from the Canvas
         * In a loop, Check that the ships coordinates don't touch any of the Debris objects
         * If it does end the game */
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
                    dbg("Collision Detected");
                    eMyo.Fill = redBrush;

                    Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;

                    debrisArray.Clear();

                    winGame.Visibility = Visibility.Visible;
                    gameText.Text = ("YOU CRASHED, GAME OVER");
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



        /* A function to keep trakc of game timer
         * Steps:
         * If the timer is 10 seconds away from a particular difficulty level limit, set timer text to red
         * If the timer reaches the level limit, end the game
         * */
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

                Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;

                myStopwatchTimer.Stop();
                stopWatch.Stop();

                debrisArray.Clear();
                eMyo.Fill = redBrush;

                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
        }
        


        // A function that fires when a user reaches the escape pod
        // Reset the various game variables and make the back button visible
        private void WinGame()
        {
            if (ship.RadiusX >= 930 && ship.RadiusY <= 34)
            {
                winGame.Visibility = Visibility.Visible;
                gameText.Text = "YOU REACHED THE ESCAPE POD!";
                Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
                myStopwatchTimer.Stop();
                stopWatch.Stop();
                debrisArray.Clear();
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

                enterName.Visibility = Visibility.Visible;
            }
        }
        #endregion



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



        /* A function that deals with which game difficulty is chosen
         * When a difficulty has been picked, start the game when a key event is triggered */
        private void Radiobutton_Checked(object sender, RoutedEventArgs e)
        {   
            if ((bool)easy.IsChecked)
            {
                easyLevel = levelTimes[0];
                difficultyInfo.Text = "Easy";
                dbg("Easy Was checked " + easyLevel);
            }
            else if ((bool)medium.IsChecked)
            {
                mediumLevel = levelTimes[1];
                difficultyInfo.Text = "Med";
                dbg("Medium Was checked " + mediumLevel);
            }
            else if ((bool)hard.IsChecked)
            {
                hardLevel = levelTimes[2];
                difficultyInfo.Text = "Hard";
                dbg("Hard Was checked " + hardLevel);
            }
            difficultyStPanel.Visibility = Visibility.Collapsed;
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            readyText.Text = "Ready? Use move keys to begin";
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
            string gameType = "Key Play";
            string finishedState = name.Text + "\t\t" + gameTimer.Text + "\t\t" + difficultyInfo.Text + "\t\t" + gameType;

            gameNameScores.Add(finishedState);

            scoreStorage.WriteScoreToFile(finishedState);

            enterName.Visibility = Visibility.Collapsed;
        }
    }
}
