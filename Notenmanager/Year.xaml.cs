using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Notenmanager
{
    public partial class Year : ContentPage
    {
        private DatabaseService _databaseService;

        public Year()
        {
            InitializeComponent();

            // Datenbankpfad festlegen und DatabaseService initialisieren
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "notenmanager.db");
            _databaseService = new DatabaseService(dbPath);

            // Jahre aus der Datenbank laden
            LoadYearsAsync();
        }

        // Methode zum Laden der Jahre aus der Datenbank
        private async Task LoadYearsAsync()
        {
            var years = await _databaseService.GetYearsAsync();

            foreach (var year in years)
            {
                AddYearToLayout(year); // Füge jedes Jahr zum Layout hinzu
            }
        }

        // Methode für das Hinzufügen eines neuen Jahres
        private async void OnAddYearClicked(object sender, EventArgs e)
        {
            // Generiere den nächsten Jahr-Namen
            string yearName = await GenerateNextYearName();

            // Neues Jahr zur Datenbank hinzufügen
            var newYear = new YearModel { Name = yearName };
            await _databaseService.AddYearAsync(newYear);

            // Füge das neue Jahr zum Layout hinzu
            AddYearToLayout(newYear);
        }


        // Methode zum Hinzufügen eines Jahres zum Layout
        private void AddYearToLayout(YearModel year)
        {
            Frame newFrame = new Frame
            {
                BorderColor = Color.FromHex("#4630D9"),
                BackgroundColor = Color.FromHex("#4630D9"),
                CornerRadius = 5,
                Margin = new Thickness(5),
                Padding = new Thickness(10)
            };

            var deleteButton = new Button
            {
                Text = "Löschen",
                BackgroundColor = Color.FromHex("#FF0000"),
                TextColor = Colors.White,
                FontSize = 12,
                WidthRequest = 80,
                HeightRequest = 40,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };

            // Event-Handler für das Löschen des Jahres
            deleteButton.Clicked += async (s, e) => await DeleteYear(year);

            Grid grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            var stackLayout = new StackLayout
            {
                Children =
                {
                    new Label
                    {
                        Text = year.Name,
                        FontSize = 18,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Colors.White
                    },
                    new Label
                    {
                        Text = "Tippen, um Fächer zu verwalten",
                        HorizontalOptions = LayoutOptions.Center,
                        TextColor = Colors.White
                    }
                }
            };

            grid.Children.Add(stackLayout);
            Grid.SetColumn(stackLayout, 0);

            grid.Children.Add(deleteButton);
            Grid.SetColumn(deleteButton, 1);


            newFrame.Content = grid;

            // Tippen auf das Jahr öffnet die Seite für die Fächer des Jahres
            newFrame.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnYearTapped(year.Name))
            });

            mainLayout.Children.Insert(mainLayout.Children.Count - 1, newFrame);
        }

        // Methode zum Öffnen der Subject-Verwaltung für ein Jahr
        private void OnYearTapped(string yearName)
        {
            // Öffne die Notenmanager-Seite mit den Fächern für das gewählte Jahr
            Navigation.PushAsync(new Notenmanager(yearName)); // Übergabe des Jahres an Notenmanager
        }


        private async Task DeleteYear(YearModel year)
        {
            bool confirm = await DisplayAlert("Bestätigen", $"Möchten Sie das Jahr '{year.Name}' wirklich löschen?", "Ja", "Nein");
            if (confirm)
            {
                // Lösche die Fächer und Noten, die zu diesem Jahr gehören
                await _databaseService.DeleteSubjectsByYearAsync(year.Name);
                await _databaseService.DeleteGradesByYearAsync(year.Name);

                // Lösche das Jahr selbst
                await _databaseService.DeleteYearAsync(year);

                // Entferne den Frame aus dem Layout
                var frameToRemove = mainLayout.Children
                    .OfType<Frame>()
                    .FirstOrDefault(f => ((Label)((StackLayout)((Grid)f.Content).Children[0]).Children[0]).Text == year.Name);

                if (frameToRemove != null)
                {
                    mainLayout.Children.Remove(frameToRemove);
                }
            }
        }
        private async Task<string> GenerateNextYearName()
        {
            var years = await _databaseService.GetYearsAsync();
            int yearCount = years.Count; // Anzahl der vorhandenen Jahre
            return $"Jahr {yearCount + 1}"; // Generiere den nächsten Jahr-Namen
        }
    }
}
