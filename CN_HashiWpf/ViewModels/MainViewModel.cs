using CNHashiGenerator;
using CNHashiWpf.Enums;
using CNHashiWpf.Extensions;
using CNHashiWpf.Helpers;
using CNHashiWpf.Messages;
using CNHashiWpf.Views;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CNHashiWpf.ViewModels
{
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public class MainViewModel : BaseViewModel, IRecipient<BridgeConnectionChangedMessage>, IRecipient<UpdateAllIslandColorsMessage>, IRecipient<AllConnectionsSetMessage>, IRecipient<CurrentSourceIslandChangedMessage>, IRecipient<PotentialTargetIslandChangedMessage>
    {
        internal readonly string HashiSettingsFileName = "HashiSettings.json";
        internal readonly string SaveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"CN_Hashi\Settings");
        private readonly HashiGenerator hashiGenerator = new();
        private IslandViewModel? currentSourceIsland;
        private IslandViewModel? potentialTargetIsland;
        private readonly DispatcherTimer dispatcherTimer = new() { Interval = TimeSpan.FromSeconds(1) };
        private bool isTimerRunning;
        private DifficultyEnum selectedDifficulty = DifficultyEnum.Easy3;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            WeakReferenceMessenger.Default.Register<BridgeConnectionChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<UpdateAllIslandColorsMessage>(this);
            WeakReferenceMessenger.Default.Register<AllConnectionsSetMessage>(this);
            WeakReferenceMessenger.Default.Register<CurrentSourceIslandChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<PotentialTargetIslandChangedMessage>(this);
            CreateNewGameCommand = new RelayCommand(CreateNewGame);
            RemoveAllBridgesCommand = new RelayCommand(RemoveAllBridgesExecute);

            dispatcherTimer.Tick += (_, _) => OnPropertyChanged(nameof(Timer));
            Settings = LoadSettings();
        }

        /// <summary>
        /// The Hashi settings.
        /// </summary>
        public SettingsViewModel Settings { get; }

        /// <summary>
        /// The highscore for the selected difficulty level.
        /// </summary>
        public TimeSpan? HighscoreForSelectedDifficulty => Settings.HighScores.FirstOrDefault(x => x.Difficulty.Equals(SelectedDifficulty))?.HighScoreTime;

        /// <summary>
        /// Determines if the timer is running.
        /// </summary>
        public bool IsTimerRunning
        {
            get => isTimerRunning;
            set => Set(ref isTimerRunning, value);
        }

        /// <summary>
        /// The connection manager for managing island connections.
        /// </summary>
        public ConnectionManagerViewModel ConnectionManager { get; } = new();

        /// <summary>
        /// The selected difficulty level.
        /// </summary>
        public DifficultyEnum SelectedDifficulty
        {
            get => selectedDifficulty;
            set
            {
                if (Set(ref selectedDifficulty, value))
                {
                    OnPropertyChanged(nameof(HighscoreForSelectedDifficulty));
                }
            }
        }

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
        /// The current source island.
        /// </summary>
        public Stopwatch Timer { get; } = new();

        /// <summary>
        /// Gets or sets the potential target island.
        /// </summary>
        public IslandViewModel? PotentialTargetIsland
        {
            get => potentialTargetIsland;
            set => Set(ref potentialTargetIsland, value);
        }

        /// <summary>
        /// Loads the settings from the JSON file.
        /// </summary>
        public SettingsViewModel LoadSettings()
        {
            SettingsViewModel loadedSettings;
            try
            {
                var path = Path.Combine(SaveFilePath, HashiSettingsFileName);
                if (File.Exists(path))
                {
                    using StreamReader file = File.OpenText(path);
                    var serializer = new JsonSerializer();
                    loadedSettings = (SettingsViewModel)serializer.Deserialize(file, typeof(SettingsViewModel))!;
                    OnPropertyChanged(nameof(HighscoreForSelectedDifficulty));
                    return loadedSettings;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }

            loadedSettings = new SettingsViewModel();
            loadedSettings.HighScores.AddRange(Enum.GetValues<DifficultyEnum>().Select(x => new HighScorePerDifficultyViewModel(x)));
            OnPropertyChanged(nameof(HighscoreForSelectedDifficulty));
            return loadedSettings;
        }

        public void SaveSettings()
        {
            if (Settings == null)
            {
                throw new InvalidOperationException("Settings cannot be null.");
            }

            var jsonArray = JsonConvert.SerializeObject(Settings, Formatting.Indented);
            var path = Path.Combine(SaveFilePath, HashiSettingsFileName);

            try
            {
                if (!Directory.Exists(SaveFilePath))
                {
                    Directory.CreateDirectory(SaveFilePath);
                }

                File.WriteAllText(path, jsonArray);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Creates a new game.
        /// </summary>
        public void CreateNewGame()
        {
            var result = hashiGenerator.GenerateHash((int)SelectedDifficulty);
            DrawGame(result);

            // Stop timers
            Timer.Reset();
            IsTimerRunning = false;
            dispatcherTimer.Stop();
            OnPropertyChanged(nameof(Timer));
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

            if (Timer.IsRunning)
            {
                Timer.Reset();
                IsTimerRunning = false;
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
                BridgeOperationType.Add => () =>
                {
                    if (!Timer.IsRunning)
                    {
                        // Start timers
                        dispatcherTimer.Start();
                        Timer.Start();
                        IsTimerRunning = true;
                    }
                    ConnectionManager.AddConnection(CurrentSourceIsland, PotentialTargetIsland);
                }
                ,
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
            Timer.Stop();
            var caption = "Game Over";
            var dialogMessage = "All connections are set!";

            //ToDo: Check if all islands are connected

            //Check if highscore - if yes, write highscore to json and show message
            var actualScore = Timer.Elapsed;
            var currentSettingForSetDifficulty = Settings.HighScores.FirstOrDefault(x => x.Difficulty == SelectedDifficulty);
            var currentHighScore = currentSettingForSetDifficulty?.HighScoreTime;
            if (currentSettingForSetDifficulty != null && (currentHighScore == null || actualScore < currentHighScore))
            {
                caption = "New Highscore";
                dialogMessage += $"\n\nCongratulations! You have achieved a new highscore for {SelectedDifficulty}.\n" +
                                $"Your time: {actualScore:hh\\:mm\\:ss}\n" +
                                $"Previous highscore: {currentHighScore:hh\\:mm\\:ss}";
                currentSettingForSetDifficulty.HighScoreTime = actualScore;
                SaveSettings();
                OnPropertyChanged(nameof(HighscoreForSelectedDifficulty));
            }

            Dialog.Show(caption, dialogMessage, DialogButton.Ok, DialogImage.Success);

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
