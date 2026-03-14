using Hashi.Enums;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.Services;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Translation;

namespace Hashi.Gui.Services;

/// <inheritdoc />
public class TestFieldService : ITestFieldService
{
    private readonly IDialogWrapper dialogWrapper;
    private readonly Func<IReadOnlyList<int[]>?, IReadOnlyList<IBridgeCoordinates>?, string?, ISolutionProvider> solutionProviderFactory;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TestFieldService" /> class.
    /// </summary>
    /// <param name="testSolutionProvider">The test solution provider.</param>
    /// <param name="dialogWrapper">The dialog wrapper.</param>
    /// <param name="solutionProviderFactory">The factory for creating solution providers.</param>
    public TestFieldService(
        ITestSolutionProvider testSolutionProvider,
        IDialogWrapper dialogWrapper,
        Func<IReadOnlyList<int[]>?, IReadOnlyList<IBridgeCoordinates>?, string?, ISolutionProvider> solutionProviderFactory)
    {
        TestSolutionProvider = testSolutionProvider ?? throw new ArgumentNullException(nameof(testSolutionProvider));
        this.dialogWrapper = dialogWrapper ?? throw new ArgumentNullException(nameof(dialogWrapper));
        this.solutionProviderFactory = solutionProviderFactory ?? throw new ArgumentNullException(nameof(solutionProviderFactory));
    }

    /// <inheritdoc />
    public ITestSolutionProvider TestSolutionProvider { get; }

    /// <inheritdoc />
    public string NewRuleName { get; set; } = string.Empty;

    /// <inheritdoc />
    public void SaveTestField(IEnumerable<IIslandViewModel> islandsFlat)
    {
        TestSolutionProvider.ConvertIslandsToSolutionProvider(islandsFlat);
        TestSolutionProvider.SaveTestFields();
    }

    /// <inheritdoc />
    public void DeleteTestField()
    {
        if (TestSolutionProvider.SelectedSolutionProvider == null)
        {
            return;
        }

        if (dialogWrapper.Show(TranslationSource.Instance.GetRequired("MessageDeleteScenarioCaption"),
                string.Format(TranslationSource.Instance.GetRequired("MessageDeleteScenarioText"),
                    TestSolutionProvider.SelectedSolutionProvider.Name),
                DialogButton.YesNo, DialogImage.Question) == DialogResult.Yes)
        {
            TestSolutionProvider.SolutionProviders.Remove(TestSolutionProvider.SelectedSolutionProvider);
            TestSolutionProvider.SelectedSolutionProvider = TestSolutionProvider.SolutionProviders.FirstOrDefault();
            TestSolutionProvider.SaveTestFields();
        }
    }

    /// <inheritdoc />
    public void CreateTestField()
    {
        var solutionProvider = solutionProviderFactory(TestSolutionProvider.HashiFieldReference, null, NewRuleName);
        TestSolutionProvider.SolutionProviders.Add(solutionProvider);
        TestSolutionProvider.SelectedSolutionProvider = solutionProvider;
        TestSolutionProvider.SaveTestFields();
    }

    /// <inheritdoc />
    public bool CanCreateTestField()
    {
        return !string.IsNullOrEmpty(NewRuleName) &&
               !TestSolutionProvider.SolutionProviders.Any(x => x.Name == null || x.Name.Equals(NewRuleName));
    }
}
