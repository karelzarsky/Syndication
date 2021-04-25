using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Mongo.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIGraph
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        Model model = new();

        public MainWindow()
        {
            this.InitializeComponent();
            Gap_from.Text = model.gapMin.ToString();
            Gap_to.Text = model.gapMax.ToString();
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.ColumnDefinitions.Clear();
            DataGrid.RowDefinitions.Clear();
            
            double.TryParse(Gap_from.Text, out model.gapMin);
            double.TryParse(Gap_to.Text, out model.gapMax);

            model.Calculate();

            for (int c = 0; c <= model.stopLoses.Length; c++)
            {
                DataGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int r = 0; r <= model.targets.Length; r++)
            {
                DataGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int r = 0; r < model.targets.Length; r++)
            {
                TextBlock tgtText = new()
                {
                    Text = $"T:{model.targets[r]:F1}%",
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(tgtText, r+1);
                Grid.SetColumn(tgtText, 0);
                DataGrid.Children.Add(tgtText);
            }

            for (int c = 0; c < model.stopLoses.Length; c++)
            {
                TextBlock slText = new()
                {
                    Text = $"SL: {model.stopLoses[c]:F1}%",
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(slText, 0);
                Grid.SetColumn(slText, c+1);
                DataGrid.Children.Add(slText);
                for (int r = 0; r < model.targets.Length; r++)
                {
                    Border border = AddCell(DataGrid, c, r);
                }
            }

            if (model.best == null)
                InfoText.Text = "No trades.";
            else InfoText.Text = $"Trades: {model.best.Trades}\n" + 
                $"OUT: {model.best.TimeoutCount} => {model.best.AvgDaysTimeout:F2} days\n" +
                $"SL : {model.best.StopLossCount} => {model.best.AvgDaysSL:F2} days\n" +
                $"PT : {model.best.TakeProfitCount} => {model.best.AvgDaysTP:F2} days\n";
        }

        public SolidColorBrush GetBrush(double value, double min, double max)
        {
            if (value == max)
                return new SolidColorBrush(Colors.Brown);
            byte Blue;
            if (value > max)
                Blue = 255;
            else if (value < min)
                Blue = 0;
            else
                Blue = (byte)((value - min)/(max-min)*255);
            return new SolidColorBrush(Color.FromArgb(0xFF, Blue, Blue, Blue));
        }

        private Border AddCell(Grid dataGrid, int c, int r)
        {
            var value = model.cellData[c, r].RealizedProfitValueAvg;
            Border b = new()
            {
                Background = GetBrush(value, model.worst.RealizedProfitValueAvg, model.best.RealizedProfitValueAvg)
            };
            Grid.SetRow(b, r+1);
            Grid.SetColumn(b, c+1);
            dataGrid.Children.Add(b);

            TextBlock cellText = new()
            {
                Text = $"{value:F0}",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = value > 0 ? new SolidColorBrush(Colors.DarkBlue) : new SolidColorBrush(Colors.White),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Grid.SetRow(cellText, r+1);
            Grid.SetColumn(cellText, c+1);
            dataGrid.Children.Add(cellText);
            return b;
        }
    }
}
