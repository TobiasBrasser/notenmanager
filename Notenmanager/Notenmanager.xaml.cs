using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Notenmanager
{
    public partial class Notenmanager : ContentPage
    {
        private string _yearName;
        private DatabaseService _databaseService;

        public Notenmanager(string yearName)
        {
            InitializeComponent();

            _yearName = yearName;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "notenmanager.db");
            _databaseService = new DatabaseService(dbPath);

            LoadSubjectsAsync();
        }

        private async Task LoadSubjectsAsync()
        {
            int yearId = await GetYearId();
            var subjects = await _databaseService.GetSubjectsByYearIdAsync(yearId);

            foreach (var subject in subjects)
            {
                AddSubjectToLayout(subject); 
            }
        }

        private async void OnAddSubjectClicked(object sender, EventArgs e)
        {
            string subjectName = await DisplayPromptAsync("Fach hinzufügen", "Bitte geben Sie den Namen des Fachs ein:",
                maxLength: 30,
                keyboard: Keyboard.Default);

            if (!string.IsNullOrWhiteSpace(subjectName))
            {
                int yearId = await GetYearId();

                bool subjectExists = await _databaseService.SubjectExistsAsync(subjectName, yearId);

                if (subjectExists)
                {
                    await DisplayAlert("Fehler", $"Das Fach '{subjectName}' ist bereits vorhanden.", "OK");
                    return; 
                }

                var newSubject = new Subject
                {
                    Name = subjectName,
                    YearId = yearId,
                    YearName = _yearName
                };

                await _databaseService.AddSubjectAsync(newSubject);

                AddSubjectToLayout(newSubject);
            }
        }


        private async Task<int> GetYearId()
        {
            return await _databaseService.GetYearIdByNameAsync(_yearName);
        }


        private void AddSubjectToLayout(Subject subject)
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

            deleteButton.Clicked += async (s, e) => await DeleteSubject(subject);

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
                Text = subject.Name,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White 
            },
            new Label
            {
                Text = "Tippen, um Noten einzugeben",
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
                Command = new Command(() => OnSubjectTapped(subject.Name, _yearName))
            });

            mainLayout.Children.Insert(mainLayout.Children.Count - 2, newFrame); 
        }

        private void OnSubjectTapped(string subjectName, string _yearname)
        {
            Navigation.PushAsync(new SubjectDetailPage(subjectName, _yearname));
        }

        private async Task DeleteSubject(Subject subject)
        {
            bool confirm = await DisplayAlert("Bestätigen", $"Möchten Sie das Fach '{subject.Name}' wirklich löschen?", "Ja", "Nein");
            if (confirm)
            {
                await _databaseService.DeleteSubjectAsync(subject); 

                var frameToRemove = mainLayout.Children
                    .OfType<Frame>()
                    .FirstOrDefault(f => ((Label)((StackLayout)((Grid)f.Content).Children[0]).Children[0]).Text == subject.Name);

                if (frameToRemove != null)
                {
                    mainLayout.Children.Remove(frameToRemove);
                }
            }
        }

        private async void OnBackToYear(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Year");
        }

    }
}
