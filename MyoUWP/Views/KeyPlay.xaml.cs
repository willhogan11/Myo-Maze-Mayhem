using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Storage;
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
        Canvas myCanvas;
        Rectangle debris;
        Rectangle ship;
        List<Rectangle> debrisArray = new List<Rectangle>();
        List<string> levelTimes;
        List<string> gameNameScores;

        DispatcherTimer myStopwatchTimer;
        Stopwatch stopWatch;

        private long ms, ss, mm, hh, dd;
        private Boolean keyCount = false;
        string easyLevel;
        string mediumLevel;
        string hardLevel;



        public KeyPlay()
        {
            this.InitializeComponent();            
            PrepareGameData();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }


        // Example one Shape created on page load
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateShip();
            CreateAllDebris();
        }


        private void PrepareGameData()
        {
            levelTimes = new List<string>();
            levelTimes.Add("01:30");
            levelTimes.Add("01:00");
            levelTimes.Add("00:30");
        }


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



        private void CreateAllDebris()
        {
            int numberOfRectangles = 900;

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
                dbg("Debris Removed from Escape Pod or Mars Base");
            }
            else
            {
                debris.Fill = brownBrush;
                debrisArray.Add(debris);
                cvsRoller.Children.Add(debris);
            }
        }



        // Get key press events working first or as a backup if myo isn't available or can't connect
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



        private void dbg(string str, Object strOpt = null)
        {
            Debug.WriteLine(str);
        }



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
                Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
                myStopwatchTimer.Stop();
                stopWatch.Stop();
                debrisArray.Clear();
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

                enterName.Visibility = Visibility.Visible;
            }

            rect1X.Text = ("Rover X : " + ship.RadiusX.ToString());
            rect1Y.Text = ("Rover Y : " + ship.RadiusY.ToString());
        }


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


        private void EnterName_Click(object sender, RoutedEventArgs e)
        {
            ScoresStorage scoreStorage = new ScoresStorage();

            gameNameScores = new List<string>();
            string gameType = "Key Play";
            string finishedState = name.Text + "\t\t" + gameTimer.Text + "\t\t" + difficultyInfo.Text + "\t\t" + gameType;

            gameNameScores.Add(finishedState);

            scoreStorage.WriteScoreToFile(finishedState);

            // ScoreStorage(finishedState);

            enterName.Visibility = Visibility.Collapsed;
        }

    }
}
