namespace ExtensionsTests;
using Extensions;


[TestClass]
public sealed class EnumerableExtensionsTests
{
    [TestMethod]
    public void Shuffle_SameElements_AfterShuffle()
    {
        // Arrange
        var original = new List<int> { 1, 2, 3, 4, 5 };

        // Act
        var shuffled = original.Shuffle().ToList();

        // Assert
        CollectionAssert.AreEquivalent(original, shuffled); // same elements, order doesn't matter
    }

    [TestMethod]
    public void Shuffle_SameCount_AfterShuffle()
    {
        // Arrange
        var original = new List<string> { "a", "b", "c" };

        // Act
        var shuffled = original.Shuffle().ToList();

        // Assert
        Assert.AreEqual(original.Count, shuffled.Count);
    }

    [TestMethod]
    public void Shuffle_OrderIsUsuallyDifferent()
    {
        // Arrange
        var original = Enumerable.Range(0, 1000).ToList();

        // Act
        var shuffled = original.Shuffle().ToList();

        // Assert
        // Note: This is probabilistic. In rare cases it could fail due to same order.
        bool isOrderSame = original.SequenceEqual(shuffled);
        Assert.IsFalse(isOrderSame, "Shuffled result was the same order as the original (very unlikely).");
    }

    [TestMethod]
    public void Shuffle_EmptyList_ReturnsEmpty()
    {
        // Arrange
        var original = new List<int>();

        // Act
        var shuffled = original.Shuffle().ToList();

        // Assert
        Assert.AreEqual(0, shuffled.Count);
    }

    [TestMethod]
    public void Shuffle_SingleElement_ReturnsSameSingle()
    {
        // Arrange
        var original = new List<int> { 42 };

        // Act
        var shuffled = original.Shuffle().ToList();

        // Assert
        CollectionAssert.AreEqual(original, shuffled);
    }
}
