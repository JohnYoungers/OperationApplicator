using System.Collections.Generic;

namespace OperationApplicator.Tests.Models
{
    public class SampleSelfReferencingItem
    {
        public string Code { get; set; }
        public string Description { get; set; }

        public List<SampleSelfReferencingItem> Children { get; set; } = new List<SampleSelfReferencingItem>();
    }
}
