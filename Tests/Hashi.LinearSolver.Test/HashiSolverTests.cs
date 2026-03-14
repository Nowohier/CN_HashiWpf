using Autofac;
using FluentAssertions;
using Google.OrTools.Sat;
using Hashi.Enums;
using Hashi.LinearSolver.Interfaces;
using Hashi.LinearSolver.Interfaces.Models;
using Hashi.Logging.Interfaces;
using Moq;

namespace Hashi.LinearSolver.Test;

[TestFixture]
public class HashiSolverTests
{
    private IContainer container;
    private IHashiSolver hashiSolver;
    private Mock<ILogger> loggerMock;
    private Mock<ILoggerFactory> loggerFactoryMock;

    [SetUp]
    public void Setup()
    {
        loggerMock = new Mock<ILogger>(MockBehavior.Strict);
        loggerFactoryMock = new Mock<ILoggerFactory>(MockBehavior.Strict);
        loggerFactoryMock.Setup(f => f.CreateLogger<It.IsAnyType>()).Returns(loggerMock.Object);

        // Setup logger methods that might be called
        loggerMock.Setup(l => l.Info(It.IsAny<string>()));
        loggerMock.Setup(l => l.Debug(It.IsAny<string>()));

        var builder = new ContainerBuilder();
        builder.RegisterInstance(loggerFactoryMock.Object).As<ILoggerFactory>();
        builder.RegisterModule<AutoFacLinearSolverModule>();
        container = builder.Build();

        hashiSolver = container.Resolve<IHashiSolver>();
    }

    [TearDown]
    public void Teardown()
    {
        container.Dispose();
    }

    [Test]
    public async Task ConvertData_WhenValidSimplePuzzle_ShouldReturnCorrectIslandsAndNeighbors()
    {
        // arrange
        var data = new int[][]
        {
            [1, 0, 1],
            [0, 0, 0],
            [1, 0, 1]
        };

        // act
        var (islands, intersections) = await hashiSolver.ConvertData(data);

        // assert
        islands.Should().HaveCount(4);

        // Check island properties
        islands[0].Should().Match<IIsland>(i => i.Id == 0 && i.Row == 0 && i.Col == 0 && i.BridgesRequired == 1);
        islands[1].Should().Match<IIsland>(i => i.Id == 1 && i.Row == 0 && i.Col == 2 && i.BridgesRequired == 1);
        islands[2].Should().Match<IIsland>(i => i.Id == 2 && i.Row == 2 && i.Col == 0 && i.BridgesRequired == 1);
        islands[3].Should().Match<IIsland>(i => i.Id == 3 && i.Row == 2 && i.Col == 2 && i.BridgesRequired == 1);

        // Check neighbors - each island should see the other islands
        islands[0].Neighbors.Should().Contain([1, 2]); // top-left sees top-right and bottom-left
        islands[1].Neighbors.Should().Contain([0, 3]); // top-right sees top-left and bottom-right
        islands[2].Neighbors.Should().Contain([0, 3]); // bottom-left sees top-left and bottom-right
        islands[3].Neighbors.Should().Contain([1, 2]); // bottom-right sees top-right and bottom-left

        // Note: Intersections require 4 neighbors from a single empty cell, which this simple case doesn't create
        intersections.Should().BeEmpty();
    }

    [Test]
    public async Task ConvertData_WhenEmptyData_ShouldReturnEmptyLists()
    {
        // arrange
        var data = new int[][]
        {
            [0, 0],
            [0, 0]
        };

        // act
        var (islands, intersections) = await hashiSolver.ConvertData(data);

        // assert
        islands.Should().BeEmpty();
        intersections.Should().BeEmpty();
    }

    [Test]
    public async Task ConvertData_WhenSingleIsland_ShouldReturnOneIslandNoIntersections()
    {
        // arrange
        var data = new int[][]
        {
            [0, 0, 0],
            [0, 2, 0],
            [0, 0, 0]
        };

        // act
        var (islands, intersections) = await hashiSolver.ConvertData(data);

        // assert
        islands.Should().HaveCount(1);
        islands[0].Should().Match<IIsland>(i => i.Id == 0 && i.Row == 1 && i.Col == 1 && i.BridgesRequired == 2);
        islands[0].Neighbors.Should().BeEmpty(); // No neighbors for isolated island
        intersections.Should().BeEmpty();
    }

    [Test]
    public async Task ConvertData_WhenTwoHorizontalIslands_ShouldReturnCorrectNeighbors()
    {
        // arrange
        var data = new int[][]
        {
            [1, 0, 1]
        };

        // act
        var (islands, intersections) = await hashiSolver.ConvertData(data);

        // assert
        islands.Should().HaveCount(2);
        islands[0].Neighbors.Should().Contain(1);
        islands[1].Neighbors.Should().Contain(0);
        intersections.Should().BeEmpty(); // No intersections with just horizontal line
    }

    [Test]
    public async Task ConvertData_WhenTwoVerticalIslands_ShouldReturnCorrectNeighbors()
    {
        // arrange
        var data = new int[][]
        {
            [1],
            [0],
            [1]
        };

        // act
        var (islands, intersections) = await hashiSolver.ConvertData(data);

        // assert
        islands.Should().HaveCount(2);
        islands[0].Neighbors.Should().Contain(1);
        islands[1].Neighbors.Should().Contain(0);
        intersections.Should().BeEmpty(); // No intersections with just vertical line
    }

    [Test]
    public async Task ReadFile_WhenValidFile_ShouldReturnCorrectData()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(tempFile, "2 2\n1 1\n1 1");

            // act
            var (islands, _) = await hashiSolver.ReadFile(tempFile);

            // assert
            islands.Should().HaveCount(4);
            islands[0].Should().Match<IIsland>(i => i.Row == 0 && i.Col == 0 && i.BridgesRequired == 1);
            islands[1].Should().Match<IIsland>(i => i.Row == 0 && i.Col == 1 && i.BridgesRequired == 1);
            islands[2].Should().Match<IIsland>(i => i.Row == 1 && i.Col == 0 && i.BridgesRequired == 1);
            islands[3].Should().Match<IIsland>(i => i.Row == 1 && i.Col == 1 && i.BridgesRequired == 1);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Test]
    public async Task ReadFile_WhenFileDoesNotExist_ShouldThrowException()
    {
        // arrange
        const string nonExistentFile = "non_existent_file.txt";

        // act & assert
        await hashiSolver.Invoking(s => s.ReadFile(nonExistentFile))
            .Should().ThrowAsync<FileNotFoundException>();
    }

    [Test]
    public async Task ReadFile_WhenInvalidFileFormat_ShouldThrowException()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(tempFile, "invalid format");

            // act & assert
            await hashiSolver.Invoking(s => s.ReadFile(tempFile))
                .Should().ThrowAsync<Exception>();
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Test]
    public async Task SolveLazy_WhenSimpleSolvablePuzzle_ShouldReturnOptimal()
    {
        // arrange
        var data = new int[][]
        {
            [1, 0, 1]
        };

        // act
        var result = await hashiSolver.SolveLazy(data, false);

        // assert
        result.Should().Be(SolverStatusEnum.Optimal);
    }

    [Test]
    public async Task SolveLazy_WhenUnsolvablePuzzle_ShouldReturnInfeasible()
    {
        // arrange
        var data = new int[][]
        {
            [3] // Single island requiring 3 bridges but having no neighbors
        };

        // act
        var result = await hashiSolver.SolveLazy(data, false);

        // assert
        result.Should().Be(SolverStatusEnum.Infeasible);
    }

    [Test]
    public async Task SolveLazy_WithFile_WhenSimpleSolvablePuzzle_ShouldReturnOptimal()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(tempFile, "1 2\n1 1");

            // act
            var result = await hashiSolver.SolveLazy(tempFile, false);

            // assert
            result.Should().Be(SolverStatusEnum.Optimal);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Test]
    public async Task SolveLazy_WithIslands_WhenValidInput_ShouldReturnOptimal()
    {
        // arrange
        var islandFactory = container.Resolve<Func<int, int, int, int, IIsland>>();
        var islands = new List<IIsland>
        {
            islandFactory(0, 0, 0, 1),
            islandFactory(1, 0, 2, 1)
        };

        // Add neighbors
        islands[0].AddNeighbor(1);
        islands[1].AddNeighbor(0);

        var intersections = new List<(int, int, int, int)>();

        // act
        var result = await hashiSolver.SolveLazy(islands, intersections, false);

        // assert
        result.Should().Be(SolverStatusEnum.Optimal);
    }

    [Test]
    public async Task ConvertData_WhenCrossPattern_ShouldReturnIntersection()
    {
        // arrange - create a cross pattern with center intersection
        var data = new int[][]
        {
            [0, 1, 0],
            [1, 0, 1],
            [0, 1, 0]
        };

        // act
        var (islands, intersections) = await hashiSolver.ConvertData(data);

        // assert
        islands.Should().HaveCount(4);
        intersections.Should().HaveCount(1);
        // The intersection should have the IDs of the 4 islands
        intersections[0].Should().Match<(int, int, int, int)>(t =>
            new[] { t.Item1, t.Item2, t.Item3, t.Item4 }.OrderBy(x => x).SequenceEqual(new[] { 0, 1, 2, 3 }));
    }

    [Test]
    public async Task PrettyPrint_WhenCalledWithoutSolution_ShouldStillComplete()
    {
        // arrange
        var islandFactory = container.Resolve<Func<int, int, int, int, IIsland>>();
        var edgeFactory = container.Resolve<Func<int, int, int, IEdge>>();

        var islands = new List<IIsland>
        {
            islandFactory(0, 0, 0, 1),
            islandFactory(1, 0, 1, 1)
        };

        var model = new CpModel();
        var x = new IntVar[2, 3];
        x[1, 1] = model.NewBoolVar("x_1_1");
        x[1, 2] = model.NewBoolVar("x_1_2");

        // Set values manually to 0 (no bridge)
        model.Add(x[1, 1] == 0);
        model.Add(x[1, 2] == 0);

        var solver = new CpSolver();
        // ReSharper disable once UnusedVariable
        var status = solver.Solve(model);

        var edgeMap = new Dictionary<int, IEdge>
        {
            { 1, edgeFactory(1, 0, 1) }
        };

        // act & assert
        await hashiSolver.Invoking(s => s.PrettyPrint(islands, x, solver, edgeMap))
            .Should().NotThrowAsync();
    }

    [Test]
    public async Task HashiSolver_WhenLoggerWritesInfo_ShouldCallLogger()
    {
        // arrange
        var data = new int[][]
        {
            [1, 0, 1]
        };

        // act
        await hashiSolver.SolveLazy(data, false);

        // assert
        loggerMock.Verify(l => l.Info(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Test]
    public async Task ReadFile_WhenFileIsEmpty_ShouldThrowFormatException()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(tempFile, string.Empty);

            // act & assert
            await hashiSolver.Invoking(s => s.ReadFile(tempFile))
                .Should().ThrowAsync<FormatException>()
                .WithMessage("*empty*");
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Test]
    public async Task ReadFile_WhenHeaderTooShort_ShouldThrowFormatException()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(tempFile, "5");

            // act & assert
            await hashiSolver.Invoking(s => s.ReadFile(tempFile))
                .Should().ThrowAsync<FormatException>()
                .WithMessage("*Header*");
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Test]
    public async Task ReadFile_WhenDimensionsNotPositive_ShouldThrowFormatException()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(tempFile, "0 3");

            // act & assert
            await hashiSolver.Invoking(s => s.ReadFile(tempFile))
                .Should().ThrowAsync<FormatException>()
                .WithMessage("*positive*");
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Test]
    public async Task ReadFile_WhenInsufficientDataRows_ShouldThrowFormatException()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(tempFile, "3 3\n1 0 1");

            // act & assert
            await hashiSolver.Invoking(s => s.ReadFile(tempFile))
                .Should().ThrowAsync<FormatException>()
                .WithMessage("*Expected*");
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
}
