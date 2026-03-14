using FluentAssertions;
using Hashi.Enums;
using Hashi.LinearSolver.Extensions;
using Hashi.LinearSolver.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Hashi.LinearSolver.Test;

[TestFixture]
public class HashiSolverIntegrationTest
{
    private ServiceProvider serviceProvider;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddLinearSolverServices();
        serviceProvider = services.BuildServiceProvider();
    }

    [TearDown]
    public void Teardown()
    {
        serviceProvider.Dispose();
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

        var hashiSolver = serviceProvider.GetRequiredService<IHashiSolver>();

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
