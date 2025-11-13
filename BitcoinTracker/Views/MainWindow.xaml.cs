using BitcoinTracker.Models; // Imports the model classes
using BitcoinTracker.ViewModels; // Imports the ViewModel layer
using System.Windows; // For basic WPF window and UI components
using System.Windows.Controls; // For UI components
using System.Windows.Media; // For drawing and coloring graphics
using System.Windows.Shapes; // For shape elements like Line and Polyline

namespace BitcoinTracker
{
    public partial class MainWindow : Window
    {
        private MainViewModel vm;

        // Constructor: initializes the window and sets up the DataContext for data binding
        public MainWindow()
        {
            InitializeComponent(); // Loads the XAML components
            vm = new MainViewModel(); // Creates the ViewModel
            DataContext = vm;  // Binds the ViewModel to the View (MainWindow)
        }


        // Event handler for "Analyze" button click
        private async void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            // Calls the async method in ViewModel to fetch and analyze Bitcoin data
            var data = await vm.AnalyzeAndGetDataAsync();

            // If data is available, draw the graph
            if (data != null) DrawGraph(data);
        }


        // Draws a simple line graph of Bitcoin prices on the Canvas
        private void DrawGraph(List<DailySummary> data)
        {
            // Clear previous drawings
            GraphCanvas.Children.Clear();

            // If there's no data, exit early
            if (data.Count == 0) return;

            // Get canvas dimensions
            double width = GraphCanvas.ActualWidth;
            double height = GraphCanvas.ActualHeight;

            // Find the maximum and minimum price to scale the graph correctly
            double maxPrice = data.Max(d => d.Price);
            double minPrice = data.Min(d => d.Price);

            // Prevent divide-by-zero if all prices are same
            if (maxPrice == minPrice) maxPrice += 1;

            // Draw background gridlines
            int gridLines = 5;
            for (int i = 0; i <= gridLines; i++)
            {
                double y = height - (i * height / gridLines);
                Line gridLine = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = width,
                    Y2 = y,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 0.5
                };
                GraphCanvas.Children.Add(gridLine);

                // Label for gridline (price scale)
                double labelValue = minPrice + ((maxPrice - minPrice) * i / gridLines);
                var label = new System.Windows.Controls.TextBlock
                {
                    Text = labelValue.ToString("0.00"),
                    Foreground = Brushes.DarkGray,
                    FontSize = 10
                };
                Canvas.SetLeft(label, 5);
                Canvas.SetTop(label, y - 10);
                GraphCanvas.Children.Add(label);
            }

            // Create a polyline to represent the Bitcoin price trend
            Polyline line = new Polyline
            {
                Stroke = Brushes.SteelBlue,
                StrokeThickness = 2,
                StrokeLineJoin = PenLineJoin.Round
            };

            // Loop through the data points and convert them into graph coordinates
            for (int i = 0; i < data.Count; i++)
            {
                // X position is evenly spaced based on number of data points
                double x = i * (width / (data.Count - 1));

                // Y position is scaled between min and max price (inverted for canvas coordinates)
                double y = height - ((data[i].Price - minPrice) / (maxPrice - minPrice) * height);

                // Add the point to the polyline
                line.Points.Add(new Point(x, y));
            }
            // Add the completed polyline (price graph) to the canvas
            GraphCanvas.Children.Add(line);

            // --- Draw X and Y axes for reference ---

            // X-axis: horizontal line at bottom
            Line xAxis = new Line
            {
                X1 = 0,
                Y1 = height,
                X2 = width,
                Y2 = height,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            GraphCanvas.Children.Add(xAxis);

            // Y-axis: vertical line on left
            Line yAxis = new Line
            {
                X1 = 0,
                Y1 = 0,
                X2 = 0,
                Y2 = height,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            GraphCanvas.Children.Add(yAxis);

            //  Add labels for min and max prices 
            var minLabel = new System.Windows.Controls.TextBlock
            {
                Text = $"Min: {minPrice:0.00}",
                Foreground = Brushes.Green,
                FontSize = 12
            };
            Canvas.SetLeft(minLabel, width - 80);
            Canvas.SetTop(minLabel, height - 20);
            GraphCanvas.Children.Add(minLabel);

            var maxLabel = new System.Windows.Controls.TextBlock
            {
                Text = $"Max: {maxPrice:0.00}",
                Foreground = Brushes.Red,
                FontSize = 12
            };
            Canvas.SetLeft(maxLabel, width - 80);
            Canvas.SetTop(maxLabel, 0);
            GraphCanvas.Children.Add(maxLabel);
        }
    }
}
