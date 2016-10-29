using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tapako.ObjectMergerTests.TestClasses
{
    public class OwnListClass : List<string>
    {
    }

    public class OwnGenericListClass<T> : List<T>
    {
    }
}
