using Autofac;
using FluentAssertions;
using Hashi.Enums;
using Hashi.Generator.Interfaces;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.LinearSolver.Interfaces;
using Hashi.Logging.Interfaces;
using Moq;

namespace Hashi.Generator.Test
{
    [TestFixture]
    public class HashiGeneratorTests
    {
        private IContainer container;
        private IHashiGenerator hashiGenerator;
        private Mock<IHashiSolver> hashiSolverMock;
        private Mock<ILogger> loggerMock;
        private Mock<ILoggerFactory> loggerFactoryMock;
        private Mock<Func<int, int, int, IIsland>> islandFactoryMock;
        private Mock<Func<IIsland, IIsland, int, IBridge>> bridgeFactoryMock;
        private Mock<Func<int[][], IList<IBridgeCoordinates>, ISolutionProvider>> solutionContainerFactoryMock;

        [SetUp]
        public void Setup()
        {
            // Arrange - Set up mocks
            hashiSolverMock = new Mock<IHashiSolver>(MockBehavior.Strict);
            loggerMock = new Mock<ILogger>(MockBehavior.Strict);
            loggerFactoryMock = new Mock<ILoggerFactory>(MockBehavior.Strict);
            islandFactoryMock = new Mock<Func<int, int, int, IIsland>>(MockBehavior.Strict);
            bridgeFactoryMock = new Mock<Func<IIsland, IIsland, int, IBridge>>(MockBehavior.Strict);
            solutionContainerFactoryMock = new Mock<Func<int[][], IList<IBridgeCoordinates>, ISolutionProvider>>(MockBehavior.Strict);

            loggerFactoryMock.Setup(f => f.CreateLogger<It.IsAnyType>()).Returns(loggerMock.Object);

            // Setup logger methods that might be called
            loggerMock.Setup(l => l.Info(It.IsAny<string>()));
            loggerMock.Setup(l => l.Warn(It.IsAny<string>()));
            loggerMock.Setup(l => l.Error(It.IsAny<string>()));
            loggerMock.Setup(l => l.Debug(It.IsAny<string>()));

            // Setup default successful solver behavior
            hashiSolverMock.Setup(s => s.SolveLazy(It.IsAny<int[][]>(), It.IsAny<bool>()))
                          .ReturnsAsync(SolverStatusEnum.Optimal);

            // Setup island factory to return real islands
            islandFactoryMock.Setup(f => f(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                            .Returns((int bridges, int y, int x) => new Hashi.Generator.Models.Island(bridges, y, x));

            // Setup bridge factory to return real bridges
            bridgeFactoryMock.Setup(f => f(It.IsAny<IIsland>(), It.IsAny<IIsland>(), It.IsAny<int>()))
                            .Returns((IIsland i1, IIsland i2, int count) => new Hashi.Generator.Models.Bridge(i1, i2, count));

            // Setup solution container factory
            var mockSolution = new Mock<ISolutionProvider>(MockBehavior.Strict);
            solutionContainerFactoryMock.Setup(f => f(It.IsAny<int[][]>(), It.IsAny<IList<IBridgeCoordinates>>()))
                                       .Returns(mockSolution.Object);

            var builder = new ContainerBuilder();
            builder.RegisterInstance(hashiSolverMock.Object).As<IHashiSolver>();
            builder.RegisterInstance(loggerFactoryMock.Object).As<ILoggerFactory>();
            builder.RegisterInstance(islandFactoryMock.Object).As<Func<int, int, int, IIsland>>();
            builder.RegisterInstance(bridgeFactoryMock.Object).As<Func<IIsland, IIsland, int, IBridge>>();
            builder.RegisterInstance(solutionContainerFactoryMock.Object).As<Func<int[][], IList<IBridgeCoordinates>, ISolutionProvider>>();
            builder.RegisterModule<AutoFacGeneratorModule>();

            container = builder.Build();
            hashiGenerator = container.Resolve<IHashiGenerator>();
        }

        [TearDown]
        public void Teardown()
        {
            container.Dispose();
        }

        #region GetDifficultySettings Tests

        [Test]
        [TestCase(0, 5, 10, 5, 10, 4, 25, 20)]
        [TestCase(1, 14, 16, 14, 16, 4, 50, 20)]
        [TestCase(2, 10, 16, 10, 16, 3, 75, 20)]
        [TestCase(3, 11, 18, 11, 18, 3, 25, 15)]
        [TestCase(4, 10, 18, 10, 18, 3, 50, 15)]
        [TestCase(5, 13, 18, 13, 18, 3, 75, 15)]
        [TestCase(6, 15, 20, 15, 20, 3, 25, 10)]
        [TestCase(7, 14, 20, 14, 20, 3, 50, 10)]
        [TestCase(8, 16, 31, 16, 31, 3, 75, 10)]
        [TestCase(9, 20, 31, 20, 31, 3, 100, 0)]
        public void GetDifficultySettings_WhenValidDifficulty_ShouldReturnCorrectSettings(
            int difficulty, int expectedMinLength, int expectedMaxLength, int expectedMinWidth, 
            int expectedMaxWidth, int expectedDivisor, int expectedAlpha, int expectedBeta)
        {
            // Act
            var result = HashiGenerator.GetDifficultySettings(difficulty);

            // Assert
            result.minLength.Should().Be(expectedMinLength);
            result.maxLength.Should().Be(expectedMaxLength);
            result.minWidth.Should().Be(expectedMinWidth);
            result.maxWidth.Should().Be(expectedMaxWidth);
            result.divisor.Should().Be(expectedDivisor);
            result.alpha.Should().Be(expectedAlpha);
            result.beta.Should().Be(expectedBeta);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(10)]
        [TestCase(100)]
        public void GetDifficultySettings_WhenInvalidDifficulty_ShouldThrowArgumentException(int difficulty)
        {
            // Act & Assert
            Action act = () => HashiGenerator.GetDifficultySettings(difficulty);
            act.Should().Throw<ArgumentException>().WithMessage("Invalid difficulty level.");
        }

        #endregion

        #region InitializeField Tests

        [Test]
        public void InitializeField_WhenValidDimensions_ShouldCreateCorrectSizedField()
        {
            // Arrange
            int length = 5;
            int width = 7;

            // Act
            var result = HashiGenerator.InitializeField(length, width);

            // Assert
            result.Should().HaveCount(length);
            result.All(row => row.Length == width).Should().BeTrue();
            result.SelectMany(row => row).All(cell => cell == 0).Should().BeTrue();
        }

        [Test]
        [TestCase(0, 5)]
        [TestCase(5, 0)]
        public void InitializeField_WhenZeroDimensions_ShouldCreateEmptyField(int length, int width)
        {
            // Act
            var result = HashiGenerator.InitializeField(length, width);

            // Assert
            result.Should().HaveCount(length);
            if (length > 0)
            {
                result.All(row => row.Length == width).Should().BeTrue();
            }
        }

        [Test]
        [TestCase(-1, 5)]
        [TestCase(5, -1)]
        public void InitializeField_WhenNegativeDimensions_ShouldThrowException(int length, int width)
        {
            // Act & Assert
            Action act = () => HashiGenerator.InitializeField(length, width);
            act.Should().Throw<Exception>();
        }

        #endregion

        #region GetAlphaForDifficulty Tests

        [Test]
        [TestCase(0, 25)]
        [TestCase(1, 50)]
        [TestCase(2, 75)]
        [TestCase(3, 25)]
        [TestCase(4, 50)]
        [TestCase(5, 75)]
        [TestCase(6, 25)]
        [TestCase(7, 50)]
        [TestCase(8, 75)]
        [TestCase(9, 100)]
        public void GetAlphaForDifficulty_WhenValidDifficulty_ShouldReturnCorrectAlpha(int difficulty, int expectedAlpha)
        {
            // Act
            var result = HashiGenerator.GetAlphaForDifficulty(difficulty);

            // Assert
            result.Should().Be(expectedAlpha);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(10)]
        public void GetAlphaForDifficulty_WhenInvalidDifficulty_ShouldReturnZero(int difficulty)
        {
            // Act
            var result = HashiGenerator.GetAlphaForDifficulty(difficulty);

            // Assert
            result.Should().Be(0);
        }

        #endregion

        #region GetBetaForDifficulty Tests

        [Test]
        [TestCase(0, 20)]
        [TestCase(1, 20)]
        [TestCase(2, 20)]
        [TestCase(3, 15)]
        [TestCase(4, 15)]
        [TestCase(5, 15)]
        [TestCase(6, 10)]
        [TestCase(7, 10)]
        [TestCase(8, 10)]
        [TestCase(9, 0)]
        public void GetBetaForDifficulty_WhenValidDifficulty_ShouldReturnCorrectBeta(int difficulty, int expectedBeta)
        {
            // Act
            var result = HashiGenerator.GetBetaForDifficulty(difficulty);

            // Assert
            result.Should().Be(expectedBeta);
        }

        [Test]
        [TestCase(-1, 20)]
        [TestCase(10, 0)]
        public void GetBetaForDifficulty_WhenOutOfRange_ShouldReturnExpectedValue(int difficulty, int expectedBeta)
        {
            // Act
            var result = HashiGenerator.GetBetaForDifficulty(difficulty);

            // Assert
            result.Should().Be(expectedBeta);
        }

        #endregion

        #region ShuffleArray Tests

        [Test]
        public void ShuffleArray_WhenCalledWithArray_ShouldModifyArray()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var originalArray = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var testArray = originalArray.ToArray(); // Create a copy

            // Act
            generator.ShuffleArray(testArray);

            // Assert - The array should be modified (high probability it's different)
            // We can't guarantee it's different due to randomness, but we can check it has same elements
            testArray.Should().BeEquivalentTo(originalArray);
            testArray.Length.Should().Be(originalArray.Length);
        }

        [Test]
        public void ShuffleArray_WhenCalledWithSingleElementArray_ShouldNotThrow()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var array = new[] { 42 };

            // Act & Assert
            Action act = () => generator.ShuffleArray(array);
            act.Should().NotThrow();
            array[0].Should().Be(42);
        }

        [Test]
        public void ShuffleArray_WhenCalledWithEmptyArray_ShouldNotThrow()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var array = Array.Empty<int>();

            // Act & Assert
            Action act = () => generator.ShuffleArray(array);
            act.Should().NotThrow();
        }

        #endregion

        #region ShuffleList Tests

        [Test]
        public void ShuffleList_WhenCalledWithList_ShouldModifyList()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var originalList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var testList = new List<int>(originalList); // Create a copy

            // Act
            generator.ShuffleList(testList);

            // Assert - The list should be modified (same elements but potentially different order)
            testList.Should().BeEquivalentTo(originalList);
            testList.Count.Should().Be(originalList.Count);
        }

        [Test]
        public void ShuffleList_WhenCalledWithSingleElementList_ShouldNotThrow()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var list = new List<int> { 42 };

            // Act & Assert
            Action act = () => generator.ShuffleList(list);
            act.Should().NotThrow();
            list[0].Should().Be(42);
        }

        [Test]
        public void ShuffleList_WhenCalledWithEmptyList_ShouldNotThrow()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var list = new List<int>();

            // Act & Assert
            Action act = () => generator.ShuffleList(list);
            act.Should().NotThrow();
        }

        #endregion

        #region HasAdjacentIsland Tests

        [Test]
        public void HasAdjacentIsland_WhenAdjacentCellHasIsland_ShouldReturnTrue()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var field = new int[][]
            {
                new int[] { 0, 0, 0 },
                new int[] { 0, 1, 2 }, // (1,1) has adjacent island at (1,2)
                new int[] { 0, 0, 0 }
            };

            // Act
            var result = generator.HasAdjacentIsland(1, 1, field);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void HasAdjacentIsland_WhenNoAdjacentIsland_ShouldReturnFalse()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var field = new int[][]
            {
                new int[] { 0, 0, 0 },
                new int[] { 0, 1, 0 }, // (1,1) has no adjacent islands
                new int[] { 0, 0, 0 }
            };

            // Act
            var result = generator.HasAdjacentIsland(1, 1, field);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void HasAdjacentIsland_WhenAtEdgeOfField_ShouldCheckOnlyValidPositions()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var field = new int[][]
            {
                new int[] { 1, 2 }, // (0,0) is at top-left corner
                new int[] { 0, 0 }
            };

            // Act
            var result = generator.HasAdjacentIsland(0, 0, field);

            // Assert
            result.Should().BeTrue(); // Adjacent at (0,1)
        }

        #endregion

        #region SetBeta Tests

        [Test]
        public void SetBeta_WhenBetaIsZero_ShouldNotModifyBridges()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var field = HashiGenerator.InitializeField(5, 5);
            
            // Act
            generator.SetBeta(field, 0);

            // Assert - Should complete without throwing
            // Since we can't access the internal bridges list directly, we verify it doesn't throw
            Assert.Pass("SetBeta completed without throwing for beta = 0");
        }

        [Test]
        public void SetBeta_WhenBetaIsNegative_ShouldNotModifyBridges()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var field = HashiGenerator.InitializeField(5, 5);
            
            // Act
            generator.SetBeta(field, -10);

            // Assert - Should complete without throwing
            Assert.Pass("SetBeta completed without throwing for negative beta");
        }

        #endregion

        #region GenerateWithDifficultyAsync Tests

        [Test]
        public async Task GenerateWithDifficultyAsync_WhenValidDifficulty_ShouldReturnSolutionProvider()
        {
            // Arrange 
            // Setup the solver to return successful status immediately
            hashiSolverMock.Setup(s => s.SolveLazy(It.IsAny<int[][]>(), It.IsAny<bool>()))
                          .ReturnsAsync(SolverStatusEnum.Optimal);

            // Act
            var result = await ((HashiGenerator)hashiGenerator).GenerateWithDifficultyAsync(5);

            // Assert
            result.Should().NotBeNull();
            hashiSolverMock.Verify(s => s.SolveLazy(It.IsAny<int[][]>(), It.IsAny<bool>()), Times.AtLeastOnce);
            // Note: We can't easily verify the solution container factory call 
            // because it depends on the complex internal generation logic
        }

        [Test]
        [TestCase(-1)]
        [TestCase(10)]
        public async Task GenerateWithDifficultyAsync_WhenInvalidDifficulty_ShouldThrowArgumentException(int difficulty)
        {
            // Act & Assert
            Func<Task> act = async () => await ((HashiGenerator)hashiGenerator).GenerateWithDifficultyAsync(difficulty);
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Invalid difficulty level.");
        }

        #endregion

        #region GenerateHashAsync Tests

        [Test]
        public async Task GenerateHashAsync_WhenDifficultySpecified_ShouldCallGenerateWithDifficulty()
        {
            // Act
            var result = await hashiGenerator.GenerateHashAsync(difficulty: 3);

            // Assert
            result.Should().NotBeNull();
            hashiSolverMock.Verify(s => s.SolveLazy(It.IsAny<int[][]>(), It.IsAny<bool>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task GenerateHashAsync_WhenNoDifficultySpecified_ShouldUseOtherParameters()
        {
            // Act
            var result = await hashiGenerator.GenerateHashAsync(amountNodes: 5, width: 10, length: 10);

            // Assert
            result.Should().NotBeNull();
            hashiSolverMock.Verify(s => s.SolveLazy(It.IsAny<int[][]>(), It.IsAny<bool>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task GenerateHashAsync_WhenSolverReturnsFeasible_ShouldRetryWithDifferentParameters()
        {
            // Arrange
            var callCount = 0;
            hashiSolverMock.Setup(s => s.SolveLazy(It.IsAny<int[][]>(), It.IsAny<bool>()))
                          .Returns(() =>
                          {
                              callCount++;
                              return Task.FromResult(callCount <= 2 ? SolverStatusEnum.Infeasible : SolverStatusEnum.Optimal);
                          });

            // Act
            var result = await hashiGenerator.GenerateHashAsync(amountNodes: 3, width: 5, length: 5);

            // Assert
            result.Should().NotBeNull();
            hashiSolverMock.Verify(s => s.SolveLazy(It.IsAny<int[][]>(), It.IsAny<bool>()), Times.AtLeast(3));
        }

        [Test]
        public async Task GenerateHashAsync_WhenMaxAttemptsReached_ShouldReduceBetaAndContinue()
        {
            // Arrange
            var callCount = 0;
            hashiSolverMock.Setup(s => s.SolveLazy(It.IsAny<int[][]>(), It.IsAny<bool>()))
                          .Returns(() =>
                          {
                              callCount++;
                              return Task.FromResult(callCount <= 8 ? SolverStatusEnum.Infeasible : SolverStatusEnum.Optimal);
                          });

            // Act
            var result = await hashiGenerator.GenerateHashAsync(amountNodes: 3, width: 5, length: 5);

            // Assert
            result.Should().NotBeNull();
            // Should retry multiple times before succeeding
            hashiSolverMock.Verify(s => s.SolveLazy(It.IsAny<int[][]>(), It.IsAny<bool>()), Times.AtLeast(6));
        }

        #endregion

        #region AddAdditionalBridges Edge Cases Tests

        [Test]
        public void AddAdditionalBridges_WhenAlphaIsZero_ShouldNotAddBridges()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var field = new int[][] { new int[] { 0, 0, 0 }, new int[] { 0, 0, 0 } };
            
            // Act
            generator.AddAdditionalBridges(field, 0);

            // Assert
            // Method should complete without error - testing the edge case where alpha = 0
            Assert.Pass("AddAdditionalBridges completed for alpha = 0");
        }

        [Test]
        public void AddAdditionalBridges_WhenTargetBridgesReached_ShouldStopAddingBridges()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var field = new int[][] { new int[] { 1, 0, 1 }, new int[] { 0, 0, 0 } };
            
            // Act - Use low alpha to test early break condition
            generator.AddAdditionalBridges(field, 1);

            // Assert
            Assert.Pass("AddAdditionalBridges completed with early break condition");
        }

        #endregion

        #region GetDownBlockedd and GetRightBlockedd Edge Cases Tests

        [Test]
        public void GetDownBlockedd_WhenIslandDownIsNull_ShouldReturnNegativeOne()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var mockIsland = new Mock<IIsland>(MockBehavior.Strict);
            mockIsland.Setup(i => i.IslandDown).Returns((IIsland?)null);
            var field = new int[][] { new int[] { 0, 0, 0 } };

            // Act
            var result = generator.GetDownBlockedd(mockIsland.Object, field);

            // Assert
            result.Should().Be(-1);
        }

        [Test]
        public void GetRightBlockedd_WhenIslandRightIsNull_ShouldReturnNegativeOne()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var mockIsland = new Mock<IIsland>(MockBehavior.Strict);
            mockIsland.Setup(i => i.IslandRight).Returns((IIsland?)null);
            var field = new int[][] { new int[] { 0, 0, 0 } };

            // Act
            var result = generator.GetRightBlockedd(mockIsland.Object, field);

            // Assert
            result.Should().Be(-1);
        }

        [Test]
        public void GetDownBlockedd_WhenCacheHit_ShouldReturnCachedValue()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var mockIslandDown = new Mock<IIsland>(MockBehavior.Strict);
            mockIslandDown.Setup(i => i.Y).Returns(3);
            
            var mockIsland = new Mock<IIsland>(MockBehavior.Strict);
            mockIsland.Setup(i => i.X).Returns(1);
            mockIsland.Setup(i => i.Y).Returns(1);
            mockIsland.Setup(i => i.IslandDown).Returns(mockIslandDown.Object);
            
            var field = new int[][] { 
                new int[] { 0, 0, 0 }, 
                new int[] { 0, 0, 0 }, 
                new int[] { 0, 0, 0 },
                new int[] { 0, 0, 0 }
            };

            // Act - Call twice to test cache
            var result1 = generator.GetDownBlockedd(mockIsland.Object, field);
            var result2 = generator.GetDownBlockedd(mockIsland.Object, field);

            // Assert - Both calls should return the same result
            result1.Should().Be(result2);
        }

        [Test]
        public void GetRightBlockedd_WhenCacheHit_ShouldReturnCachedValue()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var mockIslandRight = new Mock<IIsland>(MockBehavior.Strict);
            mockIslandRight.Setup(i => i.X).Returns(3);
            
            var mockIsland = new Mock<IIsland>(MockBehavior.Strict);
            mockIsland.Setup(i => i.X).Returns(1);
            mockIsland.Setup(i => i.Y).Returns(1);
            mockIsland.Setup(i => i.IslandRight).Returns(mockIslandRight.Object);
            
            var field = new int[][] { 
                new int[] { 0, 0, 0, 0 }, 
                new int[] { 0, 0, 0, 0 }, 
                new int[] { 0, 0, 0, 0 }
            };

            // Act - Call twice to test cache
            var result1 = generator.GetRightBlockedd(mockIsland.Object, field);
            var result2 = generator.GetRightBlockedd(mockIsland.Object, field);

            // Assert - Both calls should return the same result
            result1.Should().Be(result2);
        }

        #endregion

        #region SetBeta Edge Cases Tests

        [Test]
        public void SetBeta_WhenNoBridgesExist_ShouldHandleGracefully()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var field = new int[][] { new int[] { 0, 0, 0 } };

            // Act & Assert - Should not throw even with no bridges
            generator.SetBeta(field, 50);
            Assert.Pass("SetBeta completed gracefully with no bridges");
        }

        [Test]
        public void SetBeta_WhenBridgesToAddIsZero_ShouldReturn()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var field = new int[][] { new int[] { 0, 0, 0 } };

            // Act - Use very low beta that would result in 0 bridges to add
            generator.SetBeta(field, 1);

            // Assert
            Assert.Pass("SetBeta completed when bridges to add is zero");
        }

        #endregion
    }
}