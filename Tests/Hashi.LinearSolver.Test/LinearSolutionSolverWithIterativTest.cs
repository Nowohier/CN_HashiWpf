using Autofac;
using System.Diagnostics;

namespace Hashi.LinearSolver.Test
{
    public class LinearSolutionSolverWithIterativTest
    {
        //private IContainer container;

        //[SetUp]
        //public void Setup()
        //{
        //    var builder = new ContainerBuilder();

        //    // Register your solver module
        //    builder.RegisterModule<AutoFacLinearSolverModule>();

        //    container = builder.Build();
        //}

        //[TearDown]
        //public void Teardown()
        //{
        //    container.Dispose();
        //}

        //[Test]
        //public async Task AllPuzzlesShouldBeSolvedOptimally()
        //{
        //    // Arrange
        //    var puzzleLoader = new HashiPuzzleLoader();
        //    var puzzles = Enum.GetValues(typeof(HashiFileEnum))
        //        .Cast<HashiFileEnum>()
        //        .Select(x => puzzleLoader.LoadPuzzle(x))
        //        .ToList();

        //    // Resolve the solver from Autofac
        //    var solver = container.Resolve<ILinearSolutionSolverWithIterativ>();

        //    // Act & Assert
        //    var count = 1;
        //    foreach (var puzzle in puzzles)
        //    {
        //        var puzzleNr = $"{count++}/{puzzles.Count}";
        //        var result = await solver.SolveAsync(puzzle);
        //        Assert.That(result, Is.EqualTo(SolverStatusEnum.Optimal), $"Puzzle {puzzleNr} was not solved optimally.");
        //        Debug.WriteLine($"Solved puzzle {puzzleNr} successfully");
        //    }
        //}

        [Test]
        public void TestHashiEffective()
        {
            // Load all file paths as string from the dataset directory recursively

            // Then use HashiSolver.Solve on each of them

            // Define the root dataset directory (adjust as needed)
            var datasetDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dataset/400");

            // Recursively get all files (assuming .txt or .has or all files are puzzles)
            var puzzleFiles = Directory.GetFiles(datasetDir, "*", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".has", StringComparison.OrdinalIgnoreCase))
                .ToList();

            Assert.That(puzzleFiles.Count, Is.GreaterThan(0), "No puzzle files found in dataset directory.");

            int count = 1;
            foreach (var file in puzzleFiles)
            {
                var puzzleNr = $"{count++}/{puzzleFiles.Count}";
                Debug.WriteLine($"Solving puzzle {puzzleNr}: {file}");
                var solveTime = HashiSolver.Solve(file);
                Assert.That(solveTime, Is.GreaterThanOrEqualTo(0), $"Puzzle {puzzleNr} failed to solve.");
                Debug.WriteLine($"Solved puzzle {puzzleNr} in {solveTime} seconds.");
            }
        }

        [Test]
        public void TestHashiEffective2()
        {
            // Define the root dataset directory (adjust as needed)
            var datasetDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dataset/400");

            // Recursively get all files (assuming .txt or .has or all files are puzzles)
            var puzzleFiles = Directory.GetFiles(datasetDir, "*", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".has", StringComparison.OrdinalIgnoreCase))
                .ToList();

            HashiSolver.Benchmark(puzzleFiles.ToArray());
        }
    }
}