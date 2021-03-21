using OperationApplicator.Attributes;

namespace OperationApplicator.Tests.Models
{
    public class SampleChildItem
    {
        public string Code { get; set; }
        public string Description { get; set; }

        [OperateRecursively]
        public SampleSelfReferencingItem Child { get; set; }
    }
}
