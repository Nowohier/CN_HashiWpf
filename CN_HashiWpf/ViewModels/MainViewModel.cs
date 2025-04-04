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
    public class MainViewModel : BaseViewModel, IRecipient<BridgeConnectionChangedMessage>, IRecipient<UpdateAllIslandColorsMessage>, IRecipient<AllConnectionsSetMessage>, IRecipient<CurrentSourceIslandChangedMessage>, IRecipient<PotentialTargetIslandChangedMessage>
    {
        private readonly HashiGenerator hashiGenerator = new();
        private IslandViewModel? currentSourceIsland;
        private IslandViewModel? potentialTargetIsland;

        public MainViewModel()
        {
            WeakReferenceMessenger.Default.Register<BridgeConnectionChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<UpdateAllIslandColorsMessage>(this);
            WeakReferenceMessenger.Default.Register<AllConnectionsSetMessage>(this);
            WeakReferenceMessenger.Default.Register<CurrentSourceIslandChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<PotentialTargetIslandChangedMessage>(this);
            CreateNewGameCommand = new RelayCommand(CreateNewGame);
            RemoveAllBridgesCommand = new RelayCommand(RemoveAllBridgesExecute);
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
        /// Command to remove all bridges.
        /// </summary>
        public ICommand RemoveAllBridgesCommand { get; }

        public IslandViewModel? CurrentSourceIsland
        {
            get => currentSourceIsland;
            set => Set(ref currentSourceIsland, value);
        }

        /// <summary>
        /// Gets or sets the potential target island.
        /// </summary>
        public IslandViewModel? PotentialTargetIsland
        {
            get => potentialTargetIsland;
            set => Set(ref potentialTargetIsland, value);
        }

        /// <summary>
        /// Creates a new game.
        /// </summary>
        public void CreateNewGame()
        {
            var result = hashiGenerator.GenerateHash(SelectedDifficulty);
            DrawGame(result);
        }

        /// <summary>
        /// Creates a new game.
        /// </summary>
        public void RemoveAllBridgesExecute()
        {
            foreach (var row in ConnectionManager.Islands)
            {
                foreach (var island in row)
                {
                    island.AllConnections.Clear();
                    island.NotifyBridgeConnections();
                }
            }

            WeakReferenceMessenger.Default.Send(new UpdateAllIslandColorsMessage(Brushes.LightBlue));
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
            var bridgeOperationType = message.Value.BridgeOperationType;
            var sourceIsland = message.Value.SourceIsland;

            if (bridgeOperationType == BridgeOperationType.Add &&
                !ConnectionManager.IsValidDropTarget(CurrentSourceIsland, PotentialTargetIsland))
            {
                return;
            }

            Action bridgeAction = bridgeOperationType switch
            {
                BridgeOperationType.Add => () => ConnectionManager.AddConnection(CurrentSourceIsland, PotentialTargetIsland),
                BridgeOperationType.RemoveAll => () => ConnectionManager.RemoveAllConnections(sourceIsland, null),
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

        public void Receive(CurrentSourceIslandChangedMessage message)
        {
            CurrentSourceIsland = message.Value;
        }

        public void Receive(PotentialTargetIslandChangedMessage islandChangedMessage)
        {
            if (CurrentSourceIsland == null)
            {
                return;
            }

            if (islandChangedMessage.Value == null)
            {
                ConnectionManager.RemoveAllHighlights();
                ConnectionManager.RemoveAllPotentialIslandCoordinates();
                PotentialTargetIsland = null;
                return;
            }

            var target = ConnectionManager.GetPotentialTargetIsland(CurrentSourceIsland, islandChangedMessage.Value);

            if (target == null)
            {
                ConnectionManager.RemoveAllHighlights();
                ConnectionManager.RemoveAllPotentialIslandCoordinates();
                PotentialTargetIsland = null;
                return;
            }

            target.IslandColor = Brushes.LightGreen;
            PotentialTargetIsland = target;

            ConnectionManager.RemoveAllHighlights();
            ConnectionManager.HighlightPathToTargetIsland(CurrentSourceIsland, PotentialTargetIsland);
        }
    }
}
