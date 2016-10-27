using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace BezierSegmentDemo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private readonly Random random;
        private bool isAnimated;
        private double speed;

        // note: no INotifyPropertyChanged for this very basic example...
        public double Speed
        {
            get
            {
                return this.speed;
            }
            set
            {
                this.speed = value;
            }
        }

        public bool IsAnimated
        {
            get
            {
                return this.isAnimated;
            }
            set
            {
                this.isAnimated = value;
                if(value)
                {
                    this.AnimateAll();
                }
            }
        }

        public Window1()
        {
            InitializeComponent();

            this.DataContext = this;
            this.Speed = 1;

            this.random = new Random((int)DateTime.Now.Ticks);
        }

        private PointAnimation CreateRandomPointAnimation()
        {
            // create a PointAnimation that can animate a point in the window
            return new PointAnimation(new Point(
                this.random.NextDouble() * this.ActualWidth,
                this.random.NextDouble() * this.ActualHeight),
                TimeSpan.FromSeconds(this.Speed));
        }

        private void AnimateAll()
        {
            // the first time the animations start, they will reset the binding set in
            // the BezierFigure control and drag will no longer work
            this.Animate(BezierFigure.StartPointProperty);
            this.Animate(BezierFigure.StartBezierPointProperty);
            this.Animate(BezierFigure.EndPointProperty);
            this.Animate(BezierFigure.EndBezierPointProperty);
        }

        private void Animate(DependencyProperty property)
        {
            var animation = this.CreateRandomPointAnimation();

            if(this.IsAnimated)
            {
                animation.Completed += (s, e) => this.Animate(property);
            }

            this.figure.BeginAnimation(property, animation);
        }
    }
}
