using System.Reflection;

namespace OperationApplicator
{
    public class OperationPropertyPath
    {
        public PropertyInfo Property { get; init; }
        public string CollectionKey { get; init; }
        public OperationPropertyPath Next { get; init; }

        public override string ToString() =>
            Property.Name
            + (!string.IsNullOrEmpty(CollectionKey) ? $"[{CollectionKey}]" : "")
            + ((Next is object) ? "/" + Next : "");
    }
}