using NeuralNetworks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeuralNetworks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Controler control;

        public MainWindow()
        {
            InitializeComponent();
            GenerateStartPointsDefinitionPanel();
            control = new Controler(Graphic_Image,
                () => new Size(
                    Image_Column.ActualWidth,
                    Image_Row.ActualHeight),
                () => new Range(
                    NumValidate(X_From.Text),
                    NumValidate(X_To.Text)),
                () => new Range(
                    NumValidate(Y_From.Text),
                    NumValidate(Y_To.Text)));
            control.TeachNetwork();
            Draw();
        }

        private async void Draw()
        {
            while (true)
            {
                //control.Predict(GetPoints(), NumValidate(X_Next.Text));
                control.Clasificate(GetPoints());
                await Task.Delay(200);
            }
        }

        private void GenerateStartPointsDefinitionPanel()
        {
            for (int i = 0; i < 5; i++)
            {
                PointsDefinitionGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < 5; i++)
            {
                GeneratePointsDefinitionBlock(i);
            }
        }

        private void GeneratePointsDefinitionBlock(int position)
        {
            StackPanel stackPanel = new()
            {
                Orientation = Orientation.Horizontal
            };

            TextBlock textBlock = new()
            {
                Text = $"p{position + 1}:",
                FontSize = 20,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Brushes.White
            };
            stackPanel.Children.Add(textBlock);

            StackPanel pointDefinitionPanel = CreateStackPanelForPointDefinition();
            stackPanel.Children.Add(pointDefinitionPanel);

            Grid.SetRow(stackPanel, position);

            PointsDefinitionGrid.Children.Add(stackPanel);
        }

        private StackPanel CreateStackPanelForPointDefinition()
        {
            StackPanel stackPanel = new()
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock textBlock1 = new()
            {
                Text = "(",
                FontSize = 20
            };

            TextBox textBox2 = new()
            {
                Text = "0",
                Width = 30,
                Height = 20,
                FontSize = 14,
                Foreground = Brushes.White,
                Background = Brushes.Transparent
            };

            TextBlock textBlock3 = new()
            {
                Text = ";",
                FontSize = 20
            };

            TextBox textBox4 = new()
            {
                Text = "0",
                Width = 30,
                Height = 20,
                FontSize = 14,
                Foreground = Brushes.White,
                Background = Brushes.Transparent
            };

            TextBlock textBlock5 = new()
            {
                Text = ")",
                FontSize = 20
            };

            stackPanel.Children.Add(textBlock1);
            stackPanel.Children.Add(textBox2);
            stackPanel.Children.Add(textBlock3);
            stackPanel.Children.Add(textBox4);
            stackPanel.Children.Add(textBlock5);

            return stackPanel;
        }

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            PointsDefinitionGrid.RowDefinitions.Add(new RowDefinition());
            GeneratePointsDefinitionBlock(PointsDefinitionGrid.RowDefinitions.Count - 1);
        }

        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            if (PointsDefinitionGrid.Children.Count > 0)
            {
                PointsDefinitionGrid.Children.RemoveRange(PointsDefinitionGrid.Children.Count - 1, 1);
                PointsDefinitionGrid.RowDefinitions.Remove(PointsDefinitionGrid.RowDefinitions.Last());
            }
        }

        private Point[] GetPoints()
        {
            Point[] Points = new Point[PointsDefinitionGrid.Children.Count];

            string[] Xpoints = GetXPoints();
            string[] Ypoints = GetYPoints();


            for (int i = 0; i < Points.Length; i++)
            {
                Points[i] = new Point(NumValidate(Xpoints[i]), NumValidate(Ypoints[i]));
            }

            return Points;
        }

        private string[] GetXPoints()
        {
            string[] Xpoints = new string[PointsDefinitionGrid.Children.Count];

            for (int i = 0; i < Xpoints.Length; i++)
            {
                Xpoints[i] = (((PointsDefinitionGrid
                    .Children[i] as StackPanel)
                    .Children[1] as StackPanel)
                    .Children[1] as TextBox)
                    .Text;
            }

            return Xpoints;
        }

        private string[] GetYPoints()
        {
            string[] Ypoints = new string[PointsDefinitionGrid.Children.Count];

            for (int i = 0; i < Ypoints.Length; i++)
            {
                Ypoints[i] = (((PointsDefinitionGrid
                    .Children[i] as StackPanel)
                    .Children[1] as StackPanel)
                    .Children[3] as TextBox)
                    .Text;
            }

            return Ypoints;
        }

        private double NumValidate(string stringWithNum)
        {
            if (string.IsNullOrEmpty(stringWithNum) || stringWithNum == "-")
            {
                return 0;
            }
            stringWithNum = stringWithNum.Replace('.', ',');
            stringWithNum = stringWithNum.Trim();

            double.TryParse(stringWithNum, out double X);

            return X;
        }
    }
}
