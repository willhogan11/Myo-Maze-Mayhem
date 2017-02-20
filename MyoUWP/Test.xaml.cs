using System;
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
        }


        // Example one Shape created on page load
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateCanvas();
            CreateShip();

            // int numberOfRectangles = 800;

            //for(int i = 0; i < numberOfRectangles; i++)
            //{
            //    CreateRectangle();
            //}
        }

        private void CreateCanvas()
        {
            if(myCanvas == null)
            {
                myCanvas = new Canvas();
                SolidColorBrush lightGrayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

                myCanvas.Background = lightGrayBrush;
                myCanvas.Width = 800;

                layoutRoot.Children.Add(myCanvas);
            }
        }


        private void CreateShip()
        {
            SolidColorBrush blackBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            
            if(movableCircle == null)
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
        //private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        //{
        //    if (args.VirtualKey == VirtualKey.Down)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Key Down Pressed");
        //        movableCircle.SetValue(Canvas.TopProperty, (double)movableCircle.GetValue(Canvas.TopProperty) + 2);
        //    }
        //    if (args.VirtualKey == VirtualKey.Up)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Key Up Pressed");
        //        movableCircle.SetValue(Canvas.TopProperty, (double)movableCircle.GetValue(Canvas.TopProperty) - 2);
        //    }
        //    if (args.VirtualKey == VirtualKey.Left)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Key Left Pressed");
        //        movableCircle.SetValue(Canvas.LeftProperty, (double)movableCircle.GetValue(Canvas.LeftProperty) - 2);
        //    }
        //    if (args.VirtualKey == VirtualKey.Right)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Key Right Pressed");
        //        movableCircle.SetValue(Canvas.LeftProperty, (double)movableCircle.GetValue(Canvas.LeftProperty) + 2);
        //    }
        //}


        private void layoutRoot_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs args)
        {
            if (args.Key == VirtualKey.Down)
            {
                System.Diagnostics.Debug.WriteLine("Key Down Pressed");
                movableCircle.SetValue(Canvas.TopProperty, (double)movableCircle.GetValue(Canvas.TopProperty) + 2);
            }
            if (args.Key == VirtualKey.Up)
            {
                System.Diagnostics.Debug.WriteLine("Key Up Pressed");
                movableCircle.SetValue(Canvas.TopProperty, (double)movableCircle.GetValue(Canvas.TopProperty) - 2);
            }
            if (args.Key == VirtualKey.Left)
            {
                System.Diagnostics.Debug.WriteLine("Key Left Pressed");
                movableCircle.SetValue(Canvas.LeftProperty, (double)movableCircle.GetValue(Canvas.LeftProperty) - 2);
            }
            if (args.Key == VirtualKey.Right)
            {
                System.Diagnostics.Debug.WriteLine("Key Right Pressed");
                movableCircle.SetValue(Canvas.LeftProperty, (double)movableCircle.GetValue(Canvas.LeftProperty) + 2);
            }
        }
    }
}
