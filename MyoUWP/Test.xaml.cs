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
        public Test()
        {
            this.InitializeComponent();
        }


        // Example one Shape created on page load
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var ellipse1 = new Ellipse();
            ellipse1.Fill = new SolidColorBrush(Windows.UI.Colors.SteelBlue);
            ellipse1.Width = 50;
            ellipse1.Height = 50;

            myCanvas.Children.Add(ellipse1);
        }



        // Get key press events working first or as a backup if myo isn't available or can't connect
        //private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        //{
        //    if (args.VirtualKey == VirtualKey.Down)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Key Down Pressed");
        //        eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) + 2);
        //    }
        //    if (args.VirtualKey == VirtualKey.Up)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Key Up Pressed");
        //        eMyo.SetValue(Canvas.TopProperty, (double)eMyo.GetValue(Canvas.TopProperty) - 2);
        //    }
        //    if (args.VirtualKey == VirtualKey.Left)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Key Left Pressed");
        //        eMyo.SetValue(Canvas.LeftProperty, (double)eMyo.GetValue(Canvas.LeftProperty) - 2);
        //    }
        //    if (args.VirtualKey == VirtualKey.Right)
        //    {
        //        System.Diagnostics.Debug.WriteLine("Key Right Pressed");
        //        eMyo.SetValue(Canvas.LeftProperty, (double)eMyo.GetValue(Canvas.LeftProperty) + 2);
        //    }
        //}




    }
}
