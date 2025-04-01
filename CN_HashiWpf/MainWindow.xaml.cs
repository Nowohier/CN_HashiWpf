using CNHashiGenerator;
using CNHashiWpf.Models.V1;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CNHashiWpf
{
    public partial class MainWindow : Window
    {
        private V1GameGenerator gameGenerator;

        public MainWindow()
        {
            //var testField = new HashiPuzzleLoader().LoadPuzzle(HashiFileEnum.Hs_16_100_25_00_001);
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            //V1
            //gameGenerator = new V3GameGenerator();
            //gameGenerator.AddIslands();
            //DrawGame();

            //V2
            //var v2 = new V2Main();

            //V3
            var v3 = new HashiGenerator();
            var result = v3.GenerateHash(4);
        }

        private void DrawGame()
        {
            GameGrid.Children.Clear();
            SetGridSize();

            foreach (var island in gameGenerator.Islands)
            {
                // ToDo: Add drag drop events as well as mouse move for creating bridges whilst applying the bridge rules in the AddBridge method
                var ellipse = new Ellipse
                {
                    Width = 30,
                    Height = 30,
                    Fill = Brushes.LightBlue,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetColumn(ellipse, island.X);
                Grid.SetRow(ellipse, island.Y);
                GameGrid.Children.Add(ellipse);

                var textBlock = new TextBlock
                {
                    Text = island.BridgesNeeded.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(textBlock, island.X);
                Grid.SetRow(textBlock, island.Y);
                GameGrid.Children.Add(textBlock);
            }
        }

        private void SetGridSize()
        {
            GameGrid.ColumnDefinitions.Clear();
            GameGrid.RowDefinitions.Clear();

            // Add structure
            for (var i = 1; i <= gameGenerator.AmountColumns; i++)
            {
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
            for (var i = 1; i <= gameGenerator.AmountRows; i++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }

            // Add borders
            for (var column = 0; column < GameGrid.ColumnDefinitions.Count; column++)
            {
                for (var row = 0; row < GameGrid.RowDefinitions.Count; row++)
                {
                    var b = new Border
                    {
                        BorderBrush = Brushes.LightBlue,
                        BorderThickness = new Thickness(1)
                    };

                    Grid.SetColumn(b, column);
                    Grid.SetRow(b, row);

                    GameGrid.Children.Add(b);
                }
            }

            // Add border frame
            var border = new Border
            {
                BorderBrush = Brushes.LightBlue,
                BorderThickness = new Thickness(2)
            };

            Grid.SetColumnSpan(border, gameGenerator.AmountColumns);
            Grid.SetRowSpan(border, gameGenerator.AmountRows);
            GameGrid.Children.Add(border);
        }
    }
}