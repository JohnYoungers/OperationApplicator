using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OperationApplicator
{
    public static class OperationExtensions
    {
        public static IEnumerable<Operation> ToOperations(this object model)
            => model is null
            ? Enumerable.Empty<Operation>()
            : Dismantle(model, new Stack<PropertyInfo>());

        private static IEnumerable<Operation> Dismantle(object model, Stack<PropertyInfo> stack)
        {
            foreach (var property in model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                    .Where(p => p.CanRead
                                                                && p.GetCustomAttribute<System.Runtime.Serialization.IgnoreDataMemberAttribute>() is null
                                                                && p.GetCustomAttribute<Attributes.OperationIgnoreAttribute>() is null))
            {
                stack.Push(property);

                var parseEntireObject = (property.GetCustomAttribute<Attributes.OperateRecursivelyAttribute>() is null);
                var isCollection = typeof(IEnumerable).IsAssignableFrom(property.PropertyType);
                var value = property.GetValue(model);
                if (!property.PropertyType.IsValueType && !isCollection && !parseEntireObject)
                {
                    foreach (var op in Dismantle(value ?? Activator.CreateInstance(property.PropertyType), stack))
                    {
                        yield return op;
                    }
                }
                else
                {
                    static OperationPropertyPath getPath(IEnumerator<PropertyInfo> pathParts)
                        => pathParts.MoveNext()
                            ? new OperationPropertyPath
                            {
                                Property = pathParts.Current,
                                Next = getPath(pathParts)
                            }
                            : null;

                    yield return new Operation
                    {
                        OperationType = OperationTypes.replace,
                        PropertyPath = getPath(stack.Reverse().GetEnumerator()),
                        Value = value
                    };
                }

                stack.Pop();
            }
        }
    }
}
