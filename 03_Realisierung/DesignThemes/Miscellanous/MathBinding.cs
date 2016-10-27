using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Tapako.Design.Miscellanous
{
    /// <summary>
    /// This markup extension allows the declaration of bindings that
    /// support math formulas, like: [CenterX]-[Width]/2
    /// Where the names inside square brackets are the property paths.
    /// Source: http://www.codeproject.com/Articles/817856/MathBinding
    /// </summary>
    [ContentProperty("Bindings")]
	public sealed class MathBinding:
		MarkupExtension
	{
		// Usually I would put this class in another file, making it internal.
		// Yet, as this project is too small I believe people will prefer to copy this
		// file to their application. In this case, I prefer to keep a private nested 
		// class, so we avoid an extra file and the nested class will not be visible
		// to other classes in the project, as it is private instead of internal.
		private sealed class _Converter:
			IMultiValueConverter
		{
			private readonly MathVariable[] _orderedVariables;
			private readonly Func<double> _compiledExpression;

			internal _Converter(MultiBinding multiBinding, string expression, string elementName, BindingBase[] manuallyCreatedBindings)
			{
				MathParser mathParser = new MathParser();
				var bindings = multiBinding.Bindings;
				int bindingIndex = -1;
				var orderedVariables = new List<MathVariable>();
				if (manuallyCreatedBindings != null)
				{
					foreach(var binding in manuallyCreatedBindings)
					{
						bindingIndex++;
						var variable = mathParser.DeclareVariable("b" + bindingIndex);
						orderedVariables.Add(variable);
						bindings.Add(binding);
					}
				}

				var variableNames = new Dictionary<string, string>();
				StringBuilder mathExpression = new StringBuilder();
				int count = expression.Length;
				int position = 0;
				while(position < count)
				{
					int openingPosition = expression.IndexOf('[', position);
					if (openingPosition == -1)
						break;

					if (openingPosition > position)
					{
						mathExpression.Append(expression.Substring(position, openingPosition-position));
						mathExpression.Append(' ');
					}

					// This while is here to deal with property paths that use something like: Indexer[index]
					// Instead of creating a escape character or using double [[ as a single [ in the string, we
					// know that each [ must be matched by a ]. So, we count the [ and ]. When the count reaches
					// zero, everything is OK.
					int openingCount = 1;
					int closePosition = openingPosition + 1;
					while(closePosition < count)
					{
						char c = expression[closePosition];
						if (c == '[')
							openingCount++;
						else
						if(c == ']')
						{
							openingCount--;
							if (openingCount == 0)
								break;
						}

						closePosition++;
					}

					if (openingCount > 0)
						throw new ArgumentException("A ] is missing in the expression.", "expression");

					position = closePosition+1;
					string bindingPath = expression.Substring(openingPosition+1, closePosition-openingPosition-1);
					if (bindingPath == "")
						bindingPath = ".";

					string variableName;
					if (!variableNames.TryGetValue(bindingPath, out variableName))
					{
						var binding = new Binding(bindingPath);
						binding.ElementName = elementName;
						bindings.Add(binding);

						bindingIndex++;
						variableName = "b" + bindingIndex;
						variableNames.Add(bindingPath, variableName);

						var variable = mathParser.DeclareVariable(variableName);
						orderedVariables.Add(variable);
					}

					mathExpression.Append(variableName);
					mathExpression.Append(' ');
				}

				if (position < count)
					mathExpression.Append(expression.Substring(position, count-position));

				try
				{
					_compiledExpression = mathParser.Compile(mathExpression.ToString());
				}
				catch(ArgumentException argumentException)
				{
					throw new ArgumentException(argumentException.Message + "\nThe expression processed was preparsed and became: " + mathExpression.ToString());
				}

				_orderedVariables = orderedVariables.ToArray();
			}

			public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
			{
				int count = _orderedVariables.Length;
				if (count != values.Length)
					throw new InvalidOperationException("Something is pretty wrong. The number of received values is not the same as the number of math variables.");

				for(int i=0; i<count; i++)
				{
					var mathVariable = _orderedVariables[i];

					var value = values[i];
					if (value == DependencyProperty.UnsetValue)
						return DependencyProperty.UnsetValue;

					mathVariable.Value = System.Convert.ToDouble(value);
				}

				var result = _compiledExpression();
				return System.Convert.ChangeType(result, targetType);
			}

			public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Creates a new MathBinding using the given expression and possibly using the manuallyCreatedBindings.
		/// To reference a property/property path you must encode such a path with []. So, [Width] references
		/// the Width property while [Foreground.Color.A] references a full path.
		/// The only operators supported by the Math library at this moment are the * (multiplication),
		/// / (division), % (mudulo), + (addition) and - (subtraction).
		/// Math functions may be registered to the entire application by using MathParser.StaticRegisterFunction.
		/// Any delegate where all parameters are of type double and the result is also of type double
		/// are accepted, yet there are some overloads to make it easy to provide some methods directly.
		/// The manuallyCreatedBindings will become the variables b0, b1, b2 etc in the expression. 
		/// They don't need any placeholder, simply use them directly, like: (b0+b1)/b2.
		/// </summary>
		public static BindingBase Create(string expression, params BindingBase[] manuallyCreatedBindings)
		{
			return Create(expression, null, manuallyCreatedBindings);
		}

		/// <summary>
		/// Creates a new MathBinding using the given expression and possibly using the manuallyCreatedBindings.
		/// To reference a property/property path you must encode such a path with []. So, [Width] references
		/// the Width property while [Foreground.Color.A] references a full path.
		/// The only operators supported by the Math library at this moment are the * (multiplication),
		/// / (division), % (mudulo), + (addition) and - (subtraction).
		/// Math functions may be registered to the entire application by using MathParser.StaticRegisterFunction.
		/// Any delegate where all parameters are of type double and the result is also of type double
		/// are accepted, yet there are some overloads to make it easy to provide some methods directly.
		/// The manuallyCreatedBindings will become the variables b0, b1, b2 etc in the expression. 
		/// They don't need any placeholder, simply use them directly, like: (b0+b1)/b2.
		/// </summary>
		public static BindingBase Create(string expression, string elementName, params BindingBase[] manuallyCreatedBindings)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");

			var multiBinding = new MultiBinding();
			var converter = new _Converter(multiBinding, expression, elementName, manuallyCreatedBindings);
			multiBinding.Converter = converter;
			return multiBinding;
		}

		/// <summary>
		/// You probably don't need to call this constructor. This constructor is used by the XamlLoader.
		/// If you want to create a MathBinding in code, use the MathBinding.Create() method.
		/// You will have the possibility to provide inner-bindings that use a different source
		/// and you will avoid creating a strange MarkupExtension instance, as this class
		/// can't inherit from the Binding class as such class doesn't have a valid (protected) 
		/// method to override.
		/// When writing the expression in XAML, put the entire expression between single quotes
		/// if you need to call a function that receives more than one parameter, so the XAML
		/// will not interpret the comma as a second parameter to call this constructor, understanding
		/// that it is part of a single string.
		/// Example: {MathParsing:MathBinding 'Pow([Something], 3)'}
		/// </summary>
		public MathBinding()
		{
			Bindings = new Collection<BindingBase>();
		}

		/// <summary>
		/// You probably don't need to call this constructor. This constructor is used by the XamlLoader.
		/// If you want to create a MathBinding in code, use the MathBinding.Create() method.
		/// You will have the possibility to provide inner-bindings that use a different source
		/// and you will avoid creating a strange MarkupExtension instance, as this class
		/// can't inherit from the Binding class as such class doesn't have a valid (protected) 
		/// method to override.
		/// When writing the expression in XAML, put the entire expression between single quotes
		/// if you need to call a function that receives more than one parameter, so the XAML
		/// will not interpret the comma as a second parameter to call this constructor, understanding
		/// that it is part of a single string.
		/// Example: {MathParsing:MathBinding 'Pow([Something], 3)'}
		/// </summary>
		public MathBinding(string expression)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");

			Bindings = new Collection<BindingBase>();
			Expression = expression;
		}

		/// <summary>
		/// Gets or sets the name of the element to use as the binding source object.
		/// </summary>
		public string ElementName { get; set; }

		/// <summary>
		/// Gets or sets the expression used to generate the binding.
		/// </summary>
		public string Expression { get; set; }

		/// <summary>
		/// Gets a collection of sub-bindings that will provide the values to the
		/// variables b0, b1, b2 etc.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Collection<BindingBase> Bindings { get; private set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var binding = Create(Expression, ElementName, Bindings.ToArray());
			var result = binding.ProvideValue(serviceProvider);
			return result;
		}
	}
}
