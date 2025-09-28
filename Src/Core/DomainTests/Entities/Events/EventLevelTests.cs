
using Domain.Entities.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTests.Entities.Events;

[TestClass]
public class EventLevelTests
{
    [TestMethod]
    public void AssertCorrectOrder()
    {
        Assert.AreEqual(0, (int)EventLevel.Info);
        Assert.AreEqual(1, (int)EventLevel.Warning);
        Assert.AreEqual(2, (int)EventLevel.Error);
        Assert.AreEqual(3, (int)EventLevel.Critical);
    }
}
