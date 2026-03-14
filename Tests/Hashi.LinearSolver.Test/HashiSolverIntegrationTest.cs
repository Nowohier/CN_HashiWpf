using Autofac;
using FluentAssertions;
using Hashi.Enums;
using Hashi.LinearSolver.Interfaces;
using System.Diagnostics;

namespace Hashi.LinearSolver.Test;

[TestFixture]
public class HashiSolverIntegrationTest
{
    private IContainer container;

    [SetUp]
    public void Setup()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<AutoFacLinearSolverModule>();
        container = builder.Build();
    }

    [TearDown]
    public void Teardown()
    {
        container.Dispose();
    }

    [Test]
    [Ignore("Should not be run every time")]
    public async Task SolveLazy_WhenCalled_ShouldSolveAllPuzzlesOptimal()
    {
        // arrange
        var datasetDirectory = Path.Combine("Hashi_Puzzles2", "400");

        var puzzleFiles = Directory.GetFiles(datasetDirectory, "*", SearchOption.AllDirectories)
            .Where(f => f.EndsWith(".has", StringComparison.OrdinalIgnoreCase))
            .ToList();

        puzzleFiles.Count.Should().BeGreaterThan(0, "else no puzzle files found in dataset directory.");

        var hashiSolver = container.Resolve<IHashiSolver>();

        // act, assert
        var count = 1;
        foreach (var file in puzzleFiles)
        {
            var puzzleNr = $"{count++}/{puzzleFiles.Count}";
            Debug.WriteLine($"Solving puzzle {puzzleNr}: {file}");

            // act
            var result = await hashiSolver.SolveLazy(file, false);

            // assert
            result.Should().Be(SolverStatusEnum.Optimal, $"Puzzle {puzzleNr} was not solved optimally.");
        }
    }
}
