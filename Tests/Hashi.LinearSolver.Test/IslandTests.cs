using FluentAssertions;
using Hashi.LinearSolver.Models;

namespace Hashi.LinearSolver.Test
{
    [TestFixture]
    public class IslandTests
    {
        [Test]
        public void Constructor_WhenValidParameters_ShouldSetAllProperties()
        {
            // arrange
            const int id = 1;
            const int row = 2;
            const int col = 3;
            const int bridgesRequired = 4;

            // act
            var island = new Island(id, row, col, bridgesRequired);

            // assert
            island.Id.Should().Be(id);
            island.Row.Should().Be(row);
            island.Col.Should().Be(col);
            island.BridgesRequired.Should().Be(bridgesRequired);
            island.Neighbors.Should().NotBeNull();
            island.Neighbors.Should().BeEmpty();
        }

        [Test]
        public void Constructor_WhenZeroValues_ShouldSetProperties()
        {
            // arrange
            const int id = 0;
            const int row = 0;
            const int col = 0;
            const int bridgesRequired = 0;

            // act
            var island = new Island(id, row, col, bridgesRequired);

            // assert
            island.Id.Should().Be(id);
            island.Row.Should().Be(row);
            island.Col.Should().Be(col);
            island.BridgesRequired.Should().Be(bridgesRequired);
            island.Neighbors.Should().NotBeNull();
            island.Neighbors.Should().BeEmpty();
        }

        [Test]
        public void Neighbors_WhenAddingIds_ShouldContainAllIds()
        {
            // arrange
            var island = new Island(1, 2, 3, 4);
            var neighborIds = new[] { 5, 6, 7 };

            // act
            foreach (var neighborId in neighborIds)
            {
                island.Neighbors.Add(neighborId);
            }

            // assert
            island.Neighbors.Should().HaveCount(3);
            island.Neighbors.Should().Contain(neighborIds);
        }

        [Test]
        public void Neighbors_WhenAddingDuplicateIds_ShouldContainDuplicates()
        {
            // arrange
            var island = new Island(1, 2, 3, 4);
            const int duplicateId = 5;

            // act
            island.Neighbors.Add(duplicateId);
            island.Neighbors.Add(duplicateId);

            // assert
            island.Neighbors.Should().HaveCount(2);
            island.Neighbors.Should().OnlyContain(id => id == duplicateId);
        }

        [Test]
        public void Neighbors_WhenClearingList_ShouldBeEmpty()
        {
            // arrange
            var island = new Island(1, 2, 3, 4);
            island.Neighbors.Add(5);
            island.Neighbors.Add(6);

            // act
            island.Neighbors.Clear();

            // assert
            island.Neighbors.Should().BeEmpty();
        }
    }
}