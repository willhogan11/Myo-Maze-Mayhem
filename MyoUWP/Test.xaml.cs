using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyoUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Test : Page
    {
        Canvas myCanvas;
        Rectangle debris;
        Rectangle ship;
        List<Rectangle> debrisArray = new List<Rectangle>(); 

        public Test()
        {
            this.InitializeComponent();

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }


        // Example one Shape created on page load
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateShip();
            CreateAllDebris();
        }


        private void CreateAllDebris()
        {
            int numberOfRectangles = 800;

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


        private void CreateDebris()
        {
            Random random = new Random();
            debris = new Rectangle();
            SolidColorBrush blueBrush = new SolidColorBrush(Windows.UI.Colors.Blue);
            SolidColorBrush whiteBrush = new SolidColorBrush(Windows.UI.Colors.White);

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
                this.debris.Fill = whiteBrush;
                this.debris = null;
                Debug.WriteLine("Debris Removed from Escape Pod or Mars Base");
            }
            else
            {
                debris.Fill = blueBrush;
                debrisArray.Add(debris);
                cvsRoller.Children.Add(debris);
            }
        }


        // Working Version of Rectangle Collision
        #region
        //private void CreateRectangle()
        //{
        //    Random random = new Random();
        //    Rectangle rect;

        //    rect = new Rectangle();
        //    rect.Width = random.Next(5, 100);
        //    rect.Height = random.Next(5, 100);

        //    CompositeTransform transform = new CompositeTransform();
        //    transform.TranslateX = random.Next(0, 920);
        //    transform.TranslateY = random.Next(0, 800);
        //    rect.RenderTransform = transform;

        //    Color color = Color.FromArgb(255, (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
        //    rect.Fill = new SolidColorBrush(color);

        //    myCanvas.Children.Add(rect);
        //}
        #endregion





        // Get key press events working first or as a backup if myo isn't available or can't connect
        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
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


        private void detectCollision(object sender, object e)
        {

            SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
            SolidColorBrush whiteBrush = new SolidColorBrush(Windows.UI.Colors.White);

            debris.RadiusX = (float)Canvas.GetLeft(randomBlock);
            debris.RadiusY = (float)Canvas.GetTop(randomBlock);
            debris.Width = (float)randomBlock.Width;
            debris.Height = (float)randomBlock.Height;

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
                    ship.Fill = redBrush;
                    eMyo.Fill = redBrush;

                    debris.Visibility = Visibility.Collapsed;

                    Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;

                    winGame.Visibility = Visibility.Visible;
                    cvsRoller.Background = whiteBrush;
                    gameText.Text = ("YOU CRASHED, GAME OVER");

                    debrisArray.Clear();

                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                }
                else
                {
                    eMyo.Fill = whiteBrush;
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
                cvsRoller.Background = whiteBrush;
                debrisArray.Clear();
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }

            rect1X.Text = ("Ship X : " + ship.RadiusX.ToString());
            rect1Y.Text = ("Ship Y : " + ship.RadiusY.ToString());
        }
    }
}
