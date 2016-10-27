using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ConsoleTester
{
    /// <summary>
    /// Dieses Projekt wird dazu benutzt, schnelle Consolentests durchzuführen
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new class3().Value);
            Console.ReadKey();
        }
    }

    public class class3
    {
        [DefaultValue(5)]
        public int Value { get; set; }
    }

    class class2 : class3
    {
        public override string ToString()
        {
            return "class2";

        }
    }

    class class1 : class3
    {
        public override string ToString()
        {
            return "class3";

        }
    }

    class FieldSetter
    {
        Foo a = new Foo();

        private void SetFieldValues()
        {
            foreach (var field in
                    typeof(Foo).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                           BindingFlags.Static).Where(field => !field.IsLiteral))
            {
                if (!EvalAttribute(Attribute.GetCustomAttributes(field)))
                {
                    // Do something if no special Information is set
                    field.SetValue(a, "default Value");
                }

                else
                {
                    // Do special things
                    field.SetValue(a, "special Value");
                }
            }

        }


        internal static bool EvalAttribute(Attribute[] attributes)
        {

            foreach (Attribute attr in attributes)
            {
                var myAttr = attr as MyAttribute;
                if (myAttr != null)
                {
                    if (myAttr.SomeAttributeValues == "Specific Attribute Value")
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }


    class Foo
    {
        [MyAttribute("Example Information")] // This Attribute won't be accessed via prop-Field
        int prop { get; set; }
    }


    [AttributeUsage(AttributeTargets.All)]
    class MyAttribute : Attribute
    {
        public MyAttribute(string someInformation)
        {
            SomeAttributeValues = someInformation;
        }

        public string SomeAttributeValues;
    }
}
