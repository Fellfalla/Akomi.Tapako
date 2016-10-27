using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tapako.Framework.ExtensionMethods
{
    public static class TaskExtensionMethod
    {
        public static async Task WhenAllTasks<T>(this IEnumerable<Task<T>> enumerable)
        {
            foreach (var task in enumerable)
            {
                await task;
            }
        }
    }
}
