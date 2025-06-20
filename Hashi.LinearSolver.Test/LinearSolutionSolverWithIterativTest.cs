using Autofac;
using Hashi.Enums;
using Hashi.LinearSolver.Interfaces;
using Hashi.SolvedPuzzles;
using Hashi.SolvedPuzzles.Interfaces;
using System.Diagnostics;

namespace Hashi.LinearSolver.Test
{
    public class LinearSolutionSolverWithIterativTest
    {
        private IContainer container;

        [SetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();

            // Register your solver module
            builder.RegisterModule<AutoFacLinearSolverModule>();

            container = builder.Build();
        }

        [TearDown]
        public void Teardown()
        {
            container.Dispose();
        }

        [Test]
        public async Task AllPuzzlesShouldBeSolvedOptimally()
        {
            // Arrange
            var puzzleLoader = new HashiPuzzleLoader();
            var puzzles = Enum.GetValues(typeof(HashiFileEnum))
                .Cast<HashiFileEnum>()
                .Select(x => puzzleLoader.LoadPuzzle(x))
                .ToList();

            // Resolve the solver from Autofac
            var solver = container.Resolve<ILinearSolutionSolverWithIterativ>();

            // Act & Assert
            var count = 1;
            foreach (var puzzle in puzzles)
            {
                var puzzleNr = $"{count++}/{puzzles.Count}";
                var result = await solver.SolveAsync(puzzle);
                Assert.That(result, Is.EqualTo(SolverStatusEnum.Optimal), $"Puzzle {puzzleNr} was not solved optimally.");
                Debug.WriteLine($"Solved puzzle {puzzleNr} successfully");
            }
        }
    }
}
