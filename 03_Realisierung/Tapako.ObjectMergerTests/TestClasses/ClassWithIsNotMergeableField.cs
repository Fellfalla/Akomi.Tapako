﻿using Akomi.InformationModel.Attributes;

namespace Tapako.TestClasses
{
    public class ClassWithIsNotMergeableField
    {
        [IsMergeable(false)]
        public int[] NotMergableArray;
        
        public ClassWithIsNotMergeableField(int[] notMergeableArray)
        {
            NotMergableArray = notMergeableArray;
        }
    }
}
