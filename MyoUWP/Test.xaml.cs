using System;
using System.Diagnostics;
using Windows.System;
using Windows.UI;
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
        Ellipse movableCircle;
        Canvas myCanvas;

        public Test()
        {
            this.InitializeComponent();

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            // CreateCanvas();
            // CreateShip();
        }


        // Example one Shape created on page load
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            

            // int numberOfRectangles = 800;

            //for(int i = 0; i < numberOfRectangles; i++)
            //{
            //    CreateRectangle();
            //}
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
            SolidColorBrush blackBrush = new SolidColorBrush(Windows.UI.Colors.Black);

            if (movableCircle == null)
            {
                movableCircle = new Ellipse();
                movableCircle.Width = 25;
                movableCircle.Height = 25;
                movableCircle.Fill = blackBrush;

                layoutRoot.Children.Add(movableCircle);
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


        private void CreateRectangle()
        {
            Random random = new Random();
            Rectangle rect;

            rect = new Rectangle();
            rect.Width = random.Next(5, 100);
            rect.Height = random.Next(5, 100);

            CompositeTransform transform = new CompositeTransform();
            transform.TranslateX = random.Next(0, 720);
            transform.TranslateY = random.Next(0, 800);
            rect.RenderTransform = transform;

            Color color = Color.FromArgb(255, (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
            rect.Fill = new SolidColorBrush(color);

            myCanvas.Children.Add(rect);
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
            detectCollision(sender, args);
        }


        private void detectCollision(object sender, object e)
        {
            SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
            SolidColorBrush whiteBrush = new SolidColorBrush(Windows.UI.Colors.White);

            Rectangle rect1 = new Rectangle();
            rect1.RadiusX = (float)Canvas.GetLeft(eMyo);
            rect1.RadiusY = (float)Canvas.GetTop(eMyo);
            rect1.Width = (float)eMyo.Width;
            rect1.Height = (float)eMyo.Height;

            Rectangle rect2 = new Rectangle();
            rect2.RadiusX = (float)Canvas.GetLeft(blockObject);
            rect2.RadiusY = (float)Canvas.GetTop(blockObject);
            rect2.Width = (float)blockObject.Width;
            rect2.Height = (float)blockObject.Height;


            if ((rect1.RadiusX + rect1.Width >= rect2.RadiusX) &&
                (rect1.RadiusX <= rect2.RadiusX + rect2.Width) &&
                (rect1.RadiusY + rect1.Height >= rect2.RadiusY) &&
                (rect1.RadiusY <= rect2.RadiusY + rect2.Height))
            {
                Debug.WriteLine("Collision Detected...");
                eMyo.Fill = redBrush;
            }
            else
            {
                eMyo.Fill = whiteBrush;
            }



            if (rect1.RadiusX >= 930 && rect1.RadiusY <= 34)
            {
                Debug.WriteLine("You Reached the Escape Pod!! Well done");
                winGame.Visibility = Visibility.Visible;
                cvsRoller.Background = whiteBrush;
            }


            // Do something when Controlled Object is at the Canvas Edge
            // Need to Redraw the Rectangle at Edge
            if (rect1.RadiusX <= 0)
            {
                Debug.WriteLine("At Left Edge of Canvas");
                eMyo.SetValue(Canvas.LeftProperty, (double)eMyo.GetValue(Canvas.LeftProperty) + 2);
            }
            if (rect1.RadiusY <= 0)
            {
                Debug.WriteLine("At Top Edge of Canvas");
                eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) + 2);
            }
            if(rect1.RadiusX + eMyo.Width >= cvsRoller.Width)
            {
                Debug.WriteLine("At Right Edge of Canvas");
                eMyo.SetValue(Canvas.LeftProperty, (double)eMyo.GetValue(Canvas.LeftProperty) - 2);
            }
            if (rect1.RadiusY + eMyo.Height >= cvsRoller.Height)
            {
                Debug.WriteLine("At Bottom Edge of Canvas");
                eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) - 2);
            }


            // Update the Display Text boxes on screen with X Y Coordinates
            rect1X.Text = ("Rect 1 X : " + rect1.RadiusX.ToString()); 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ellipseStoryBoard.Begin();
        }
    }
}
