using OperationApplicator.Attributes;
using System;
using System.Collections.Generic;

namespace OperationApplicator.Tests.Models
{
    public class SampleSource
    {
        public int IntValue { get; set; }
        public int? NullableIntValue { get; set; }
        public Guid? NullableGuidValue { get; set; }
        public string StringValue { get; set; }

        [OperationIgnore]
        public string ThisShouldNotGenerateAnOperation { get; set; }

        public List<string> StringList { get; set; } = new List<string>();

        public List<SampleSelfReferencingItem> SubItems { get; set; } = new List<SampleSelfReferencingItem>();

        [OperateRecursively]
        public SampleChildItem CantUpdateInOneOperation { get; set; }

        public SampleSelfReferencingItem MustUpdateInOneOperation { get; set; }

        public List<SampleSelfReferencingItem> ListOfMustUpdateInOneOperation { get; set; } = new List<SampleSelfReferencingItem>();
    }
}
