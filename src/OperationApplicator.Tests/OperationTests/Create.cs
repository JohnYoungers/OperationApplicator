using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationApplicator.Tests.Models;
using System.Linq;

namespace OperationApplicator.Tests.OperationTests
{
    [TestClass]
    public class Create
    {
        [TestMethod]
        public void HandlesPathsAndDefaults()
        {
            var model = new SampleSource
            {
                IntValue = 2,
                CantUpdateInOneOperation = new SampleChildItem { Code = "A" }
            };

            (new[]
            {
                Operation.Create(model, i => i.IntValue),
                Operation.Create(model, i => i.CantUpdateInOneOperation.Code),
                Operation.Create(model, i => i.CantUpdateInOneOperation.Child.Code),
                Operation.Create(null as SampleSource, i => i.IntValue)
            })
                .Select(op => new { Path = op.PropertyPath.ToString(), op.Value })
                .Should().BeEquivalentTo(new[]
                {
                    new { Path = "/IntValue", Value = (object)2 },
                    new { Path = "/CantUpdateInOneOperation/Code", Value = (object)"A" },
                    new { Path = "/CantUpdateInOneOperation/Child/Code", Value = (object)null },
                    new { Path = "/IntValue", Value = (object)0 }
                });
        }
    }
}
