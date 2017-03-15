using Windows.UI.Xaml.Media;

namespace MyoUWP
{
    public class RectangleClass
    {
        public double RadiusX { get; set; }
        public double RadiusY { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public CompositeTransform RenderTransform { get; set; }
        public SolidColorBrush Fill { get; set; }
    }
}
