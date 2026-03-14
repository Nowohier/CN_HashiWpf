using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Interfaces.Services;

/// <summary>
///     Handles test field CRUD operations.
/// </summary>
public interface ITestFieldService
{
    /// <summary>
    ///     Gets the test solution provider for managing test fields.
    /// </summary>
    ITestSolutionProvider TestSolutionProvider { get; }

    /// <summary>
    ///     Gets or sets the new rule name for creating test fields.
    /// </summary>
    string NewRuleName { get; set; }

    /// <summary>
    ///     Saves the current test field state.
    /// </summary>
    /// <param name="islandsFlat">The flat collection of island view models to convert.</param>
    void SaveTestField(IEnumerable<IIslandViewModel> islandsFlat);

    /// <summary>
    ///     Deletes the currently selected test field.
    /// </summary>
    void DeleteTestField();

    /// <summary>
    ///     Creates a new test field with the current rule name.
    /// </summary>
    void CreateTestField();

    /// <summary>
    ///     Determines whether a new test field can be created.
    /// </summary>
    /// <returns><c>true</c> if a new test field can be created; otherwise, <c>false</c>.</returns>
    bool CanCreateTestField();
}
