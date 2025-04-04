using CNHashiGenerator;
using CNHashiWpf.Enums;
using CNHashiWpf.Helpers;
using CNHashiWpf.Messages;
using CNHashiWpf.Views;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
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

        /// <summary>
        /// The connection manager for managing island connections.
        /// </summary>
        public ConnectionManagerViewModel ConnectionManager { get; } = new();

        /// <summary>
        /// The selected difficulty level.
        /// </summary>
        public DifficultyEnum SelectedDifficulty { get; set; } = DifficultyEnum.Easy3;

        /// <summary>
        /// Command to create a new game.
        /// </summary>
        public ICommand CreateNewGameCommand { get; }

        /// <summary>
        /// Command to remove all bridges.
        /// </summary>
        public ICommand RemoveAllBridgesCommand { get; }

        /// <summary>
        /// The current source island.
        /// </summary>
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
            var result = hashiGenerator.GenerateHash((int)SelectedDifficulty);
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
        /// Handles the message when a bridge connection is changed.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="ArgumentOutOfRangeException">The <see cref="BridgeConnectionChangedMessage"/>.</exception>
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

        /// <summary>
        /// Updates the color of all islands.
        /// </summary>
        /// <param name="message">The <see cref="UpdateAllIslandColorsMessage"/>.</param>
        public void Receive(UpdateAllIslandColorsMessage message)
        {
            var color = message.Value;

            foreach (var row in ConnectionManager.Islands)
            {
                foreach (var island in row)
                {
                    island.IslandColor = island.MaxConnectionsReached ? HashiColors.MaxBridgesReachedBrush : color;
                }
            }
        }

        /// <summary>
        /// Handles the message when all connections are set.
        /// </summary>
        /// <param name="message">The <see cref="AllConnectionsSetMessage"/>.</param>
        public void Receive(AllConnectionsSetMessage message)
        {
            Dialog.Show("Game Over", "All connections are set!", DialogButton.Ok, DialogImage.Success);

            //ToDo: Check if all islands are connected

            CreateNewGame();
        }

        /// <summary>
        /// Handles the message when the current source island is changed.
        /// </summary>
        /// <param name="message">The <see cref="CurrentSourceIslandChangedMessage"/>.</param>
        public void Receive(CurrentSourceIslandChangedMessage message)
        {
            CurrentSourceIsland = message.Value;
        }

        /// <summary>
        /// Handles the message when the potential target island is changed.
        /// </summary>
        /// <param name="islandChangedMessage">The <see cref="PotentialTargetIslandChangedMessage"/>.</param>
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

            target.IslandColor = HashiColors.GreenIslandBrush;
            PotentialTargetIsland = target;

            ConnectionManager.RemoveAllHighlights();
            ConnectionManager.HighlightPathToTargetIsland(CurrentSourceIsland, PotentialTargetIsland);
        }

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
    }
}
