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

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "notenmanager.db");
            _databaseService = new DatabaseService(dbPath);

            LoadYearsAsync();
        }

        private async Task LoadYearsAsync()
        {
            var years = await _databaseService.GetYearsAsync();

            if (years.Count == 0)
            {
                string yearName = GenerateCurrentSemesterName();

                var newYear = new YearModel { Name = yearName };
                await _databaseService.AddYearAsync(newYear);
                years.Add(newYear); 
            }

            foreach (var year in years)
            {
                AddYearToLayout(year);
            }
        }


        private async void OnAddYearClicked(object sender, EventArgs e)
        {
            string yearName = GenerateCurrentSemesterName();

            var existingYears = await _databaseService.GetYearsAsync();
            if (existingYears.Any(y => y.Name == yearName))
            {
                await DisplayAlert("Fehler", $"Das Jahr '{yearName}' existiert bereits.", "OK");
                return;
            }

            var newYear = new YearModel { Name = yearName };
            await _databaseService.AddYearAsync(newYear);

            AddYearToLayout(newYear);
        }

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

            newFrame.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnYearTapped(year.Name))
            });

            mainLayout.Children.Insert(mainLayout.Children.Count - 2, newFrame);
        }

        private void OnYearTapped(string yearName)
        {
            Navigation.PushAsync(new Notenmanager(yearName));
        }


        private async Task DeleteYear(YearModel year)
        {
            bool confirm = await DisplayAlert("Bestätigen", $"Möchten Sie das Jahr '{year.Name}' wirklich löschen?", "Ja", "Nein");
            if (confirm)
            {
                await _databaseService.DeleteSubjectsByYearAsync(year.Name);
                await _databaseService.DeleteGradesByYearAsync(year.Name);

                await _databaseService.DeleteYearAsync(year);

                var frameToRemove = mainLayout.Children
                    .OfType<Frame>()
                    .FirstOrDefault(f => ((Label)((StackLayout)((Grid)f.Content).Children[0]).Children[0]).Text == year.Name);

                if (frameToRemove != null)
                {
                    mainLayout.Children.Remove(frameToRemove);
                }
            }
        }

        private string GenerateCurrentSemesterName()
        {
            var currentYear = DateTime.Now.Year;
            var nextYear = currentYear + 1;

            string semesterType = DateTime.Now.Month >= 7 || DateTime.Now.Month <= 1 ? "Wintersemester" : "Sommersemester";

            if (semesterType == "Wintersemester")
            {
                return $"Semester {currentYear} - {nextYear}";
            }
            else
            {
                return $"Semester {currentYear}";
            }
        }

        private async void OnBackToHome(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
