using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Tapako.View.CustomElements
{
    /// <summary>
    /// This class extends <see cref="Image"/> with additional Events and customizations.
    /// e.g. SourceUpdatedEvent
    /// </summary>
    public class ExtendedImage : Image
    {
        private string _oldSource;

        public static readonly RoutedEvent SourceChangedEvent = EventManager.RegisterRoutedEvent(
            "SourceChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ExtendedImage));

        static ExtendedImage()
        {
            Image.SourceProperty.OverrideMetadata(typeof(ExtendedImage), new FrameworkPropertyMetadata(SourcePropertyChanged));
        }

        public event RoutedEventHandler SourceChanged
        {
            add { AddHandler(SourceChangedEvent, value); }
            remove { RemoveHandler(SourceChangedEvent, value); }
        }

        private static void SourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var image = obj as ExtendedImage;

            if (image == null)
            {
                return;
            }

            string newSource = ((BitmapFrame) image.Source).Decoder.ToString();
            if (newSource != image._oldSource)
            {
                image._oldSource = newSource;
                image.RaiseEvent(new RoutedEventArgs(SourceChangedEvent));
            }
        }
    }
}
