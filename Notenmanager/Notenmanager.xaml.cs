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

            // Datenbankpfad festlegen und DatabaseService initialisieren
            _yearName = yearName;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "notenmanager.db");
            _databaseService = new DatabaseService(dbPath);

            // F�cher aus der Datenbank laden
            LoadSubjectsAsync();
        }

        // Methode zum Laden der F�cher aus der Datenbank
        private async Task LoadSubjectsAsync()
        {
            int yearId = await GetYearId();
            var subjects = await _databaseService.GetSubjectsByYearIdAsync(yearId);

            foreach (var subject in subjects)
            {
                AddSubjectToLayout(subject); // F�ge jedes Fach zum Layout hinzu
            }
        }

        // Methode f�r das Hinzuf�gen eines neuen Faches
        private async void OnAddSubjectClicked(object sender, EventArgs e)
        {
            // Fragen Sie den Benutzer nach dem Fachnamen
            string subjectName = await DisplayPromptAsync("Fach hinzuf�gen", "Bitte geben Sie den Namen des Fachs ein:",
                maxLength: 30,
                keyboard: Keyboard.Default);

            if (!string.IsNullOrWhiteSpace(subjectName))
            {

                int yearId = await GetYearId();
                // Neues Fach zur Datenbank hinzuf�gen
                var newSubject = new Subject
                {
                    Name = subjectName,
                    YearId = yearId, 
                    YearName = _yearName 
                };

                await _databaseService.AddSubjectAsync(newSubject);

                // F�ge das neue Fach zum Layout hinzu
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
                Text = "L�schen",
                BackgroundColor = Color.FromHex("#FF0000"),
                TextColor = Colors.White,
                FontSize = 12, // Verkleinere die Schriftgr��e
                WidthRequest = 80, // Breite des Buttons
                HeightRequest = 40, // H�he des Buttons
                HorizontalOptions = LayoutOptions.End, // Rechtsb�ndig
                VerticalOptions = LayoutOptions.Center // Zentriere vertikal
            };

            // Event-Handler f�r das L�schen des Fachs
            deleteButton.Clicked += async (s, e) => await DeleteSubject(subject);

            Grid grid = new Grid
            {
                ColumnDefinitions =
        {
            new ColumnDefinition { Width = GridLength.Star }, // Platz f�r den Text
            new ColumnDefinition { Width = GridLength.Auto }  // Platz f�r den Button
        }
            };

            // Fachname und Beschreibung in der linken Spalte (Spalte 0)
            var stackLayout = new StackLayout
            {
                Children =
        {
            new Label
            {
                Text = subject.Name,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White // Setze die Textfarbe auf Wei�
            },
            new Label
            {
                Text = "Tippen, um Noten einzugeben",
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Colors.White // Setze die Textfarbe auf Wei�
            }
        }
            };

            // Positioniere den StackLayout in der ersten Spalte (Spalte 0)
            grid.Children.Add(stackLayout);
            Grid.SetColumn(stackLayout, 0);

            // Positioniere den L�schen-Button in der zweiten Spalte (Spalte 1)
            grid.Children.Add(deleteButton);
            Grid.SetColumn(deleteButton, 1);

            // F�ge das Grid in den Frame ein
            newFrame.Content = grid;

            // Tippen auf das Fach �ffnet die Detailseite f�r die Noteneingabe
            newFrame.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnSubjectTapped(subject.Name, _yearName))
            });

            // F�ge das neue Fach zur Hauptseite hinzu
            mainLayout.Children.Insert(mainLayout.Children.Count - 1, newFrame); // F�ge das Fach vor dem letzten Element ein
        }


        // Methode zum �ffnen der SubjectDetailPage
        private void OnSubjectTapped(string subjectName, string _yearname)
        {
            // �ffne die Detailseite, um Noten einzugeben
            Navigation.PushAsync(new SubjectDetailPage(subjectName, _yearname));
        }

        // Methode zum L�schen eines Fachs
        // Methode zum L�schen eines Fachs
        private async Task DeleteSubject(Subject subject)
        {
            bool confirm = await DisplayAlert("Best�tigen", $"M�chten Sie das Fach '{subject.Name}' wirklich l�schen?", "Ja", "Nein");
            if (confirm)
            {
                await _databaseService.DeleteSubjectAsync(subject); // Fach aus der Datenbank l�schen

                // Suche den Frame, der das Fach darstellt
                var frameToRemove = mainLayout.Children
                    .OfType<Frame>()
                    .FirstOrDefault(f => ((Label)((StackLayout)((Grid)f.Content).Children[0]).Children[0]).Text == subject.Name);

                // Entferne den Frame aus dem Layout, falls er gefunden wurde
                if (frameToRemove != null)
                {
                    mainLayout.Children.Remove(frameToRemove);
                }
            }
        }

    }
}
