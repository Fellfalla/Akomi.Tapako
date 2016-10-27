using System.Windows;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Tapako.Utilities.WiringTool.View
{
    public class VisualWiring : Wiring<FrameworkElement>{
        public VisualWiring(FrameworkElement item1, FrameworkElement item2) : base(item1, item2)
        {
        }

        public new static VisualWiring Create(FrameworkElement item1, FrameworkElement item2)
        {
            return new VisualWiring(item1, item2);
        }
    }

    public class LogicalWiring : Wiring<object>
    {
        public LogicalWiring(object item1, object item2) : base(item1, item2)
        {
        }

        public new static LogicalWiring Create(object item1, object item2)
        {
            return new LogicalWiring(item1, item2);
        }
    }

    public class Wiring
    {
        public static Wiring Create(FrameworkElement visual1, FrameworkElement visual2, object logical1, object logical2)
        {
            var wiring = new Wiring();
            wiring.Visual = VisualWiring.Create(visual1, visual2);
            wiring.Logical = LogicalWiring.Create(logical1, logical2);
            return wiring;
        }

        /// <summary>
        /// Creates a <see cref="Wiring"/> instance without <see cref="LogicalWiring"/>
        /// </summary>
        /// <param name="visual1"></param>
        /// <param name="visual2"></param>
        /// <returns></returns>
        public static Wiring Create(FrameworkElement visual1, FrameworkElement visual2)
        {
            var wiring = new Wiring();
            wiring.Visual = VisualWiring.Create(visual1, visual2);
            return wiring;
        }

        /// <summary>
        /// Creates a <see cref="Wiring"/> instance without <see cref="VisualWiring"/>
        /// </summary>
        /// <param name="logical1"></param>
        /// <param name="logical2"></param>
        /// <returns></returns>
        public static Wiring Create(object logical1, object logical2)
        {
            var wiring = new Wiring();
            wiring.Logical = LogicalWiring.Create(logical1, logical2);
            return wiring;
        }

        public VisualWiring Visual { get; set; }
        public LogicalWiring Logical { get; set; }
    }
}