using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace OperationApplicator.Tests.OperationPropertyPathTests
{
    [TestClass]
    public class ToString
    {
        [TestMethod]
        public void GeneratesRecursively()
        {
            var prop1 = typeof(DateTime).GetProperty(nameof(DateTime.Day));
            var prop2 = typeof(DateTime).GetProperty(nameof(DateTime.Hour));
            var prop3 = typeof(DateTime).GetProperty(nameof(DateTime.Minute));

            var op = new Operation
            {
                PropertyPath = new OperationPropertyPath
                {
                    Property = prop1,
                    Next = new OperationPropertyPath
                    {
                        Property = prop2,
                        Next = new OperationPropertyPath
                        {
                            Property = prop3
                        }
                    }
                }
            };

            Assert.AreEqual("/Day/Hour/Minute", op.PropertyPath.ToString());
        }
    }
}
