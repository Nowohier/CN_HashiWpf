using CNHashiGenerator;
using CNHashiWpf.Enums;
using CNHashiWpf.Messages;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CNHashiWpf.ViewModels
{
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public class MainViewModel : BaseViewModel, IRecipient<BridgeConnectionChangedMessage>, IRecipient<UpdateAllIslandColorsMessage>, IRecipient<AllConnectionsSetMessage>
    {
        private readonly HashiGenerator hashiGenerator = new();

        public MainViewModel()
        {
            WeakReferenceMessenger.Default.Register<BridgeConnectionChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<UpdateAllIslandColorsMessage>(this);
            WeakReferenceMessenger.Default.Register<AllConnectionsSetMessage>(this);
            CreateNewGameCommand = new RelayCommand(CreateNewGame);
        }

        public ConnectionManagerViewModel ConnectionManager { get; } = new();

        public int SelectedDifficulty { get; set; } = 2;

        /// <summary>
        /// List of difficulty settings.
        /// </summary>
        public List<int> DifficultySettings { get; } = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        /// <summary>
        /// Command to create a new game.
        /// </summary>
        public ICommand CreateNewGameCommand { get; }

        /// <summary>
        /// Creates a new game.
        /// </summary>
        public void CreateNewGame()
        {
            var result = hashiGenerator.GenerateHash(SelectedDifficulty);
            DrawGame(result);
        }

        /// <summary>
        /// Draws the game.
        /// </summary>
        /// <param name="mainArray"></param>
        private void DrawGame(IReadOnlyList<int[]> mainArray)
        {
            ConnectionManager.Islands.Clear();
            for (var row = 0; row < mainArray.Count; row++)
            {
                var rowCollection = new ObservableCollection<IslandViewModel>();
                for (var column = 0; column < mainArray[0].Length; column++)
                {
                    rowCollection.Add(new IslandViewModel(column, row, mainArray[row][column]));
                }
                ConnectionManager.Islands.Add(rowCollection);
            }
        }

        public void Receive(BridgeConnectionChangedMessage message)
        {
            var connectionInfos = message.Value;

            if (connectionInfos.BridgeOperationType != BridgeOperationType.RemoveAll && connectionInfos.IsDiagonalConnection)
            {
                return;
            }

            var sourceIsland = connectionInfos.SourceIsland;
            var targetIsland = connectionInfos.TargetIsland;

            Action bridgeAction = connectionInfos.BridgeOperationType switch
            {
                BridgeOperationType.Add => () => ConnectionManager.AddConnection(sourceIsland, targetIsland),
                BridgeOperationType.Remove => () => ConnectionManager.RemoveConnection(sourceIsland, targetIsland),
                BridgeOperationType.RemoveAll => () => ConnectionManager.RemoveAllConnections(sourceIsland, targetIsland),
                _ => throw new ArgumentOutOfRangeException()
            };

            bridgeAction();
        }

        public void Receive(UpdateAllIslandColorsMessage message)
        {
            var color = message.Value;

            foreach (var row in ConnectionManager.Islands)
            {
                foreach (var island in row)
                {
                    island.IslandColor = island.MaxConnectionsReached ? Brushes.LightCoral : color;
                }
            }
        }

        public void Receive(AllConnectionsSetMessage message)
        {
            MessageBox.Show("All connections are set!", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);

            //ToDo: Check if all islands are connected

            CreateNewGame();
        }
    }
}
