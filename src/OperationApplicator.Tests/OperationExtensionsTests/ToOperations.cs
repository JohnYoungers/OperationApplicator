using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationApplicator.Tests.Models;
using System.Collections.Generic;
using System.Linq;

namespace OperationApplicator.Tests.OperationExtensionsTests
{
    [TestClass]
    public class ToOperations
    {
        [TestMethod]
        public void Basic()
        {
            var stringList = new List<string> { "abc" };
            var model = new SampleSource
            {
                IntValue = 5,
                StringList = stringList
            };

            var operations = model.ToOperations().OrderBy(p => p.PropertyPath.Property.Name).ToList();

            var intValueOp = operations.FirstOrDefault(o => o.PropertyPath.Property.Name == nameof(SampleSource.IntValue));
            Assert.AreEqual(OperationTypes.replace, intValueOp.OperationType);
            Assert.AreEqual(5, intValueOp.Value);

            var nullableIntValueOp = operations.FirstOrDefault(o => o.PropertyPath.Property.Name == nameof(SampleSource.NullableIntValue));
            Assert.AreEqual(OperationTypes.replace, nullableIntValueOp.OperationType);
            Assert.AreEqual(null, nullableIntValueOp.Value);

            var stringValueOp = operations.FirstOrDefault(o => o.PropertyPath.Property.Name == nameof(SampleSource.StringValue));
            Assert.AreEqual(OperationTypes.replace, stringValueOp.OperationType);
            Assert.AreEqual(null, stringValueOp.Value);

            var stringListOp = operations.FirstOrDefault(o => o.PropertyPath.Property.Name == nameof(SampleSource.StringList));
            Assert.AreEqual(OperationTypes.replace, stringListOp.OperationType);
            Assert.AreEqual(stringList, stringListOp.Value);

            var subItemsOp = operations.FirstOrDefault(o => o.PropertyPath.Property.Name == nameof(SampleSource.SubItems));
            Assert.AreEqual(OperationTypes.replace, subItemsOp.OperationType);
            Assert.AreEqual(0, (subItemsOp.Value as List<SampleSelfReferencingItem>).Count);

            var cantUpdateInOneOperationOps = operations.Where(o => o.PropertyPath.Property.Name == nameof(SampleSource.CantUpdateInOneOperation)).ToList();
            Assert.IsTrue(cantUpdateInOneOperationOps.Any());
            Assert.IsTrue(cantUpdateInOneOperationOps.All(o => o.OperationType == OperationTypes.replace));
            Assert.IsTrue(cantUpdateInOneOperationOps.All(o => o.Value == null || o.Value is List<SampleSelfReferencingItem>)); //Children collection will be instantiated as new list

            //Ignored
            Assert.IsTrue(!operations.Any(p => p.PropertyPath.Property.Name == nameof(SampleSource.ThisShouldNotGenerateAnOperation)));
        }

        [TestMethod]
        public void Collection()
        {
            var subItems = new List<SampleSelfReferencingItem>
                {
                    new SampleSelfReferencingItem
                    {
                        Code = "A",
                        Description = "AA"
                    },
                    new SampleSelfReferencingItem
                    {
                        Code = "B",
                        Description = "BB",
                        Children = new List<SampleSelfReferencingItem>
                        {
                            new SampleSelfReferencingItem
                            {
                                Code = "C",
                                Description = "CC"
                            }
                        }
                    }
            };

            var model = new SampleSource { SubItems = subItems };

            var operations = model.ToOperations();
            var subItemOperations = operations.Where(o => o.PropertyPath.Property.Name == nameof(SampleSource.SubItems)).ToList();

            Assert.AreEqual(OperationTypes.replace, subItemOperations[0].OperationType);
            Assert.AreEqual(nameof(SampleSource.SubItems), subItemOperations[0].PropertyPath.Property.Name);
            Assert.AreEqual(subItems, subItemOperations[0].Value);
        }

        [TestMethod]
        public void RecursiveProperty()
        {
            var child = new SampleSelfReferencingItem
            {
                Code = "C",
                Description = "CC"
            };
            var model = new SampleSource
            {
                CantUpdateInOneOperation = new SampleChildItem
                {
                    Code = "B",
                    Description = "BB",
                    Child = child
                }
            };

            var operations = model.ToOperations();
            Assert.IsTrue(operations.All(o => o.OperationType == OperationTypes.replace));

            var subItemOperations = operations.Where(o => o.PropertyPath.Property.Name == nameof(SampleSource.CantUpdateInOneOperation)).ToList();

            var codeOp = subItemOperations.First(o => o.PropertyPath.Next?.Property.Name == nameof(SampleSelfReferencingItem.Code));
            Assert.AreEqual("B", codeOp.Value);

            var descriptionOp = subItemOperations.First(o => o.PropertyPath.Next?.Property.Name == nameof(SampleSelfReferencingItem.Description));
            Assert.AreEqual("BB", descriptionOp.Value);

            var childCodeOp = subItemOperations.First(o => o.PropertyPath.Next?.Property.Name == nameof(SampleChildItem.Child) && o.PropertyPath.Next?.Next?.Property.Name == nameof(SampleSelfReferencingItem.Code));
            Assert.AreEqual("C", childCodeOp.Value);

            var childDescriptionOp = subItemOperations.First(o => o.PropertyPath.Next?.Property.Name == nameof(SampleChildItem.Child) && o.PropertyPath.Next?.Next?.Property.Name == nameof(SampleSelfReferencingItem.Description));
            Assert.AreEqual("CC", childDescriptionOp.Value);

            var childChildrenOp = subItemOperations.First(o => o.PropertyPath.Next?.Property.Name == nameof(SampleChildItem.Child) && o.PropertyPath.Next?.Next?.Property.Name == nameof(SampleSelfReferencingItem.Children));
            Assert.AreEqual(0, (childChildrenOp.Value as List<SampleSelfReferencingItem>).Count);
        }

        [TestMethod]
        public void RecursiveProperty_Null()
        {
            var model = new SampleSource();
            var subItemOperations = model.ToOperations().Where(o => o.PropertyPath.Property.Name == nameof(SampleSource.CantUpdateInOneOperation)).ToList();

            var codeOp = subItemOperations.First(o => o.PropertyPath.Next?.Property.Name == nameof(SampleSelfReferencingItem.Code));
            Assert.AreEqual(null, codeOp.Value);

            var descriptionOp = subItemOperations.First(o => o.PropertyPath.Next?.Property.Name == nameof(SampleSelfReferencingItem.Description));
            Assert.AreEqual(null, descriptionOp.Value);

            var childCodeOp = subItemOperations.First(o => o.PropertyPath.Next?.Property.Name == nameof(SampleChildItem.Child) && o.PropertyPath.Next?.Next?.Property.Name == nameof(SampleSelfReferencingItem.Code));
            Assert.AreEqual(null, childCodeOp.Value);

            var childDescriptionOp = subItemOperations.First(o => o.PropertyPath.Next?.Property.Name == nameof(SampleChildItem.Child) && o.PropertyPath.Next?.Next?.Property.Name == nameof(SampleSelfReferencingItem.Description));
            Assert.AreEqual(null, childDescriptionOp.Value);

            var childChildrenOp = subItemOperations.First(o => o.PropertyPath.Next?.Property.Name == nameof(SampleChildItem.Child) && o.PropertyPath.Next?.Next?.Property.Name == nameof(SampleSelfReferencingItem.Children));
            Assert.AreEqual(0, (childChildrenOp.Value as List<SampleSelfReferencingItem>).Count);
        }
    }
}
