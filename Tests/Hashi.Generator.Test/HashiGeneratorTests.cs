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
        private IContainer container = null!;
        private IHashiGenerator hashiGenerator = null!;
        private Mock<IHashiSolver> hashiSolverMock = null!;
        private Mock<ILogger> loggerMock = null!;
        private Mock<ILoggerFactory> loggerFactoryMock = null!;
        private Mock<Func<int, int, int, IIsland>> islandFactoryMock = null!;
        private Mock<Func<IIsland, IIsland, int, IBridge>> bridgeFactoryMock = null!;
        private Mock<Func<int[][], List<IBridgeCoordinates>, ISolutionProvider>> solutionContainerFactoryMock = null!;

        [SetUp]
        public void Setup()
        {
            // Arrange - Set up mocks
            hashiSolverMock = new Mock<IHashiSolver>(MockBehavior.Strict);
            loggerMock = new Mock<ILogger>(MockBehavior.Strict);
            loggerFactoryMock = new Mock<ILoggerFactory>(MockBehavior.Strict);
            islandFactoryMock = new Mock<Func<int, int, int, IIsland>>(MockBehavior.Strict);
            bridgeFactoryMock = new Mock<Func<IIsland, IIsland, int, IBridge>>(MockBehavior.Strict);
            solutionContainerFactoryMock = new Mock<Func<int[][], List<IBridgeCoordinates>, ISolutionProvider>>(MockBehavior.Strict);

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
            solutionContainerFactoryMock.Setup(f => f(It.IsAny<int[][]>(), It.IsAny<List<IBridgeCoordinates>>()))
                                       .Returns(mockSolution.Object);

            var builder = new ContainerBuilder();
            builder.RegisterInstance(hashiSolverMock.Object).As<IHashiSolver>();
            builder.RegisterInstance(loggerFactoryMock.Object).As<ILoggerFactory>();
            builder.RegisterInstance(islandFactoryMock.Object).As<Func<int, int, int, IIsland>>();
            builder.RegisterInstance(bridgeFactoryMock.Object).As<Func<IIsland, IIsland, int, IBridge>>();
            builder.RegisterInstance(solutionContainerFactoryMock.Object).As<Func<int[][], List<IBridgeCoordinates>, ISolutionProvider>>();
            builder.RegisterModule<AutoFacGeneratorModule>();

            container = builder.Build();
            hashiGenerator = container.Resolve<IHashiGenerator>();
        }

        [TearDown]
        public void Teardown()
        {
            container.Dispose();
        }

        #region GenerateWithDifficultyAsync Tests

        [Test]
        public async Task GenerateWithDifficultyAsync_WhenValidDifficulty_ShouldReturnSolutionProvider()
        {
            // Arrange
            hashiSolverMock.Setup(s => s.SolveLazy(It.IsAny<int[][]>(), It.IsAny<bool>()))
                          .ReturnsAsync(SolverStatusEnum.Optimal);

            // Act
            var result = await ((HashiGenerator)hashiGenerator).GenerateWithDifficultyAsync(5);

            // Assert
            result.Should().NotBeNull();
            hashiSolverMock.Verify(s => s.SolveLazy(It.IsAny<int[][]>(), It.IsAny<bool>()), Times.AtLeastOnce);
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
            hashiSolverMock.Verify(s => s.SolveLazy(It.IsAny<int[][]>(), It.IsAny<bool>()), Times.AtLeast(6));
        }

        #endregion

        #region CreateHashAsync Tests

        [Test]
        public async Task CreateHashAsync_WhenLargeGrid_ShouldUseParallelProcessing()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var context = new GenerationContext();

            // Act
            var result = await generator.CreateHashAsync(context, 25, 20, 20, 5, 10, true);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(20);
        }

        [Test]
        public async Task CreateHashAsync_WhenSmallGrid_ShouldUseSequentialProcessing()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var context = new GenerationContext();

            // Act
            var result = await generator.CreateHashAsync(context, 5, 10, 10, 3, 5, true);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(10);
        }

        [Test]
        public async Task CreateHashAsync_WhenMaxIterationsReached_ShouldStopAndReturn()
        {
            // Arrange
            var generator = (HashiGenerator)hashiGenerator;
            var context = new GenerationContext();

            // Act
            var result = await generator.CreateHashAsync(context, 3, 5, 5, 1, 1, false);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(5);
        }

        #endregion
    }
}
