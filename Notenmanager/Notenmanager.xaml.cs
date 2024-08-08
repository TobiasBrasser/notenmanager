using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Notenmanager
{
    public partial class Notenmanager : ContentPage
    {
        private Dictionary<Grid, Label> averageLabels = new Dictionary<Grid, Label>();
        private Label totalAverageLabel;

        public Notenmanager()
        {
            InitializeComponent();
            InitializeAverageLabels();
            InitializeTotalAverageLabel();
        }

        private void InitializeAverageLabels()
        {
            averageLabels.Add(grid1, averageLabel1);
            averageLabels.Add(grid2, averageLabel2);
            averageLabels.Add(grid3, averageLabel3);
            averageLabels.Add(grid4, averageLabel4);
            averageLabels.Add(grid5, averageLabel5);
            averageLabels.Add(grid6, averageLabel6);
        }

        private void InitializeTotalAverageLabel()
        {
            totalAverageLabel = new Label();
            totalAverageLabel.Text = "Gesamtdurchschnitt: 0";
            totalAverageLabel.FontSize = 16;
            totalAverageLabel.FontAttributes = FontAttributes.Bold;

            // Add the total average label to the main layout
            // You may need to adjust the layout to fit your design
            mainLayout.Children.Add(totalAverageLabel);
        }

        private void UpdateTotalAverage()
        {
            double totalSum = 0;
            int totalCount = 0;

            foreach (var entryGridPair in averageLabels)
            {
                var grid = entryGridPair.Key;
                var averageLabel = entryGridPair.Value;

                if (averageLabel != null)
                {
                    if (double.TryParse(averageLabel.Text.Split(':').Last().Trim(), out double average))
                    {
                        totalSum += average;
                        totalCount++;
                    }
                }
            }

            // Update the total average label
            totalAverageLabel.Text = $"Gesamtdurchschnitt: {(totalCount > 0 ? totalSum / totalCount : 0)}";
        }

        private void OnNoteEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry)
            {
                var parentGrid = entry.Parent as Grid;

                if (parentGrid != null)
                {
                    if (IsNumericEntry(entry))
                    {
                        if (!IsValidNumber(entry.Text))
                        {
                            // If the current entry is not a valid number, clear the Entry text
                            entry.Text = string.Empty;

                            // Display an error message
                            DisplayAlert("Error", "In diese Zellen dürfen nur Zahlen zwischen 1 und 6 eingegeben werden. Kommas (,) sind ebenfalls nicht erlaubt, verwenden Sie einen Punkt (.)", "OK");
                            return; // Exit the method to avoid further processing
                        }
                    }

                    // Calculate the average as before
                    var numericEntries = parentGrid.Children.OfType<Entry>().Where(IsNumericEntry);

                    double sum = 0;
                    int count = 0;

                    foreach (var numericEntry in numericEntries)
                    {
                        if (double.TryParse(numericEntry.Text, out double value))
                        {
                            sum += value;
                            count++;
                        }
                    }

                    var averageLabel = averageLabels[parentGrid];

                    if (averageLabel != null)
                    {
                        averageLabel.Text = $"Durchschnitt: {(count > 0 ? sum / count : 0)}";
                    }

                    // Update the total average label
                    UpdateTotalAverage();
                }
            }
        }

        private bool IsValidNumber(string input)
        {
            // Check if the input is a valid number and is not greater than 6
            double result;
            if (double.TryParse(input, out result))
            {
                return result >= 1 && result <= 6;
            }
            if (input != string.Empty)
            {
                return false;
            }
            Regex regex = new Regex(@"^[1-6](?:\,[0-9]+)?$");
            return !regex.IsMatch(input) || double.TryParse(input, out result);
        }

        private bool IsNumericEntry(Entry entry)
        {
            return entry.Placeholder.StartsWith("Note");
        }

        private async void OnNavigate(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Notenmanager2");
        }
    }
}