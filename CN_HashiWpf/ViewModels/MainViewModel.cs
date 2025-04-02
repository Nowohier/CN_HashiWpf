using CNHashiGenerator;
using System.Collections.ObjectModel;

namespace CNHashiWpf.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<ObservableCollection<IslandViewModel>> Islands { get; } = new();

        public void Initialize()
        {
            var v3 = new HashiGenerator();
            var result = v3.GenerateHash(2);
            DrawGame(result);
        }

        private void DrawGame(IReadOnlyList<int[]> mainArray)
        {
            for (var row = 0; row < mainArray.Count; row++)
            {
                var rowCollection = new ObservableCollection<IslandViewModel>();
                for (var column = 0; column < mainArray[0].Length; column++)
                {
                    rowCollection.Add(new IslandViewModel(column, row, mainArray[row][column]));
                }
                Islands.Add(rowCollection);
            }
        }
    }
}
