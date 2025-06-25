using FluentAssertions;
using Hashi.LinearSolver.Models;

namespace Hashi.LinearSolver.Test
{
    [TestFixture]
    public class EdgeTests
    {
        [Test]
        public void Constructor_WhenValidParameters_ShouldSetAllProperties()
        {
            // arrange
            const int id = 1;
            const int islandA = 2;
            const int islandB = 3;

            // act
            var edge = new Edge(id, islandA, islandB);

            // assert
            edge.Id.Should().Be(id);
            edge.IslandA.Should().Be(islandA);
            edge.IslandB.Should().Be(islandB);
        }

        [Test]
        public void Constructor_WhenZeroValues_ShouldSetProperties()
        {
            // arrange
            const int id = 0;
            const int islandA = 0;
            const int islandB = 0;

            // act
            var edge = new Edge(id, islandA, islandB);

            // assert
            edge.Id.Should().Be(id);
            edge.IslandA.Should().Be(islandA);
            edge.IslandB.Should().Be(islandB);
        }

        [Test]
        public void Constructor_WhenNegativeValues_ShouldSetProperties()
        {
            // arrange
            const int id = -1;
            const int islandA = -2;
            const int islandB = -3;

            // act
            var edge = new Edge(id, islandA, islandB);

            // assert
            edge.Id.Should().Be(id);
            edge.IslandA.Should().Be(islandA);
            edge.IslandB.Should().Be(islandB);
        }

        [Test]
        public void Constructor_WhenSameIslandIds_ShouldSetProperties()
        {
            // arrange
            const int id = 1;
            const int islandA = 2;
            const int islandB = 2;

            // act
            var edge = new Edge(id, islandA, islandB);

            // assert
            edge.Id.Should().Be(id);
            edge.IslandA.Should().Be(islandA);
            edge.IslandB.Should().Be(islandB);
        }
    }
}