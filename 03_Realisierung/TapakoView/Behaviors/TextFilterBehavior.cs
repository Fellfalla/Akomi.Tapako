using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Tapako.View.Behaviors
{
    /// <summary>
    /// Eine Filterklasse, um Texteingaben in z.b. TextBoxes nach bestimmten Zeichen zu Filtern.
    /// Die Zeichen werden mit SetFilterText gesetzt.
    /// Um diese Klasse zu nutzen muss nur im .xaml der FilterText gesetzt werden.
    /// 
    /// Diese Klasse wurde nicht weiter kommentiert, da der Code nicht verstanden wurde
    /// </summary>
    public class TextFilterBehavior : Behavior<TextBox>
    {

        //public static void SetFilterText(DependencyObject dependencyObject, string value)
        //{
        //    dependencyObject.SetValue(FilterTextProperty, value);
        //}        

        //public static string GetFilterText(DependencyObject dependencyObject)
        //{
        //    return (string) dependencyObject.GetValue(FilterTextProperty);
        //}

        //public static void SetValidTextContent(DependencyObject dependencyObject, string value)
        //{
        //    dependencyObject.SetValue(ValidTextContentProperty, value);
        //}

        //public static string GetValidTextContent(DependencyObject dependencyObject)
        //{
        //    return (string)dependencyObject.GetValue(ValidTextContentProperty);
        //}

        //public static readonly DependencyProperty FilterTextProperty = DependencyProperty.RegisterAttached(
        //    "FilterText", typeof (string), typeof (TextFilterBehavior), new UIPropertyMetadata(default(string), OnAttached));

        //public static readonly DependencyProperty ValidTextContentProperty = DependencyProperty.RegisterAttached(
        //    "ValidTextContent", typeof(string), typeof(TextFilterBehavior), new UIPropertyMetadata(default(string), OnAttached));

        public static readonly DependencyProperty FilterTextProperty = DependencyProperty.Register(
            "FilterText", typeof(string), typeof(TextFilterBehavior), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ValidTextContentProperty = DependencyProperty.Register(
            "ValidTextContent", typeof(string), typeof(TextFilterBehavior), new PropertyMetadata(default(string)));

        public string FilterText
        {
            get { return (string)GetValue(FilterTextProperty); }
            set{SetValue(FilterTextProperty, value);}
        }

        public string ValidTextContent
        {
            get { return (string) GetValue(ValidTextContentProperty); }
            set { SetValue(FilterTextProperty, value); }
        }


        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += TextBoxOnPreviewTextInput;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        private void TextBoxOnPreviewTextInput(object sender, TextCompositionEventArgs textCompositionEventArgs)
        {

            if (ValidTextContent != null)
                textCompositionEventArgs.Handled = !ValidTextContent.Contains(textCompositionEventArgs.Text);

            if (FilterText != null)
                textCompositionEventArgs.Handled = FilterText.Contains(textCompositionEventArgs.Text);
        }

    }
}
