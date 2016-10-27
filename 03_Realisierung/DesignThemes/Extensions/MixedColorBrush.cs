using System;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Media;

namespace Tapako.Design.Extensions
{
    /// <summary>
    /// Markup extension to mix two SolidColorBrushes together to produce a new SolidColorBrush.
    /// Source: http://stackoverflow.com/questions/906045/combined-solidcolor-brush
    /// <example>
    /// Background="{local:MixedColorBrush Foreground=Blue, Background=Red}"
    /// </example>    
    /// <example>
    /// local:MixedColorBrush Foreground="Blue" Background="Red"
    /// </example>
    /// </summary>
    [MarkupExtensionReturnType(typeof(SolidColorBrush))]
    public class MixedColorBrush : MarkupExtension, INotifyPropertyChanged
    {
        /// <summary>
        /// The foreground mix color; defaults to white.  
        /// If not changed, the result will always be white.
        /// </summary>
        private SolidColorBrush _foreground = Brushes.White;

        /// <summary>
        /// The background mix color; defaults to black.  
        /// If not set, the result will be the foreground color.
        /// </summary>
        private SolidColorBrush _background = Brushes.Black;

        /// <summary>
        /// PropertyChanged event for WPF binding.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Gets or sets the foreground mix color.
        /// </summary>
        public SolidColorBrush Foreground
        {
            get
            {
                return this._foreground;
            }
            set
            {
                this._foreground = value;
                this.NotifyPropertyChanged("Foreground");
            }
        }

        /// <summary>
        /// Gets or sets the background mix color.
        /// </summary>
        public SolidColorBrush Background
        {
            get
            {
                return this._background;
            }
            set
            {
                this._background = value;
                this.NotifyPropertyChanged("Background");
            }
        }

        /// <summary>
        /// Returns a SolidColorBrush that is set as the value of the 
        /// target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (this._foreground != null && this._background != null)
            {
                // Create a new brush as a composite of the old ones
                // This does simple non-perceptual additive color, e.g 
                // blue + red = magenta, but you can swap in a different
                // algorithm to do subtractive color (red + yellow = orange)
                //return new SolidColorBrush(this._foreground.Color + this._background.Color);

                Color first = SolidColorBrushToColor(Foreground);
                Color second = SolidColorBrushToColor(Background);
                SolidColorBrush mixedBrush = new SolidColorBrush(MixColors(first, second));
                return mixedBrush;
            }

            if (this._foreground != null)
            {
                return _foreground;
            }

            if (this._background != null)
            {
                return _background;
            }

            // If either of the brushes was set to null, return an empty (white) brush.
            return new SolidColorBrush();
        }

        private Color SolidColorBrushToColor(SolidColorBrush brush)
        {
            var a = brush.Opacity*(brush.Color.A/(double) byte.MaxValue);
            return Color.FromArgb(Convert.ToByte(byte.MaxValue * a), brush.Color.R, brush.Color.G,
    brush.Color.B);
        }

        private Color MixColors(Color foreground, Color background)
        {
            double fa = TransformByteToDouble(foreground.A);
            double fr = TransformByteToDouble(foreground.R);
            double fg = TransformByteToDouble(foreground.G);
            double fb = TransformByteToDouble(foreground.B);

            double ba = TransformByteToDouble(background.A);
            double br = TransformByteToDouble(background.R);
            double bg = TransformByteToDouble(background.G);
            double bb = TransformByteToDouble(background.B);

            double ra = 0;
            double rr = 0;
            double rg = 0;
            double rb = 0;

            ra =  (1 - (1 - fa) * (1 - ba));
            if (ra < 1.0e-6) return CreateColor(ra, rr, rg, rb); // Fully transparent -- R,G,B not important
            rr = fr * fa / ra + br * ba * (1 - fa) / ra;
            rg = fg * fa / ra + bg * ba * (1 - fa) / ra;
            rb = fb * fa / ra + bb * ba * (1 - fa) / ra;

            return CreateColor(ra, rr, rg, rb);

            //return Coloradd(first, second);
        }

        private double TransformByteToDouble(byte n)
        {
            return (double) n / (double) byte.MaxValue;
        }

        private Color CreateColor(double a, double r, double g, double b)
        {
            return Color.FromArgb(Convert.ToByte(a*byte.MaxValue), Convert.ToByte(r * byte.MaxValue), Convert.ToByte(g * byte.MaxValue), Convert.ToByte(b * byte.MaxValue));
        }

        /// <summary>
        /// Raise the property changed event.
        /// </summary>
        /// <param name="propertyName">Name of the property which has changed.</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
