using Akomi.InformationModel.Attributes;

namespace Tapako.TestClasses
{
    public class ClassWithMergeableField
    {
        [IsMergeable(true)]
        public int[] MergableArray;
        
        public ClassWithMergeableField(int[] mergeableArray)
        {
            MergableArray = mergeableArray;
        }
    }
}
