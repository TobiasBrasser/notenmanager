using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Notenmanager
{
    public partial class SubjectDetailPage : ContentPage
    {
        private List<GradeInfo> notenList;
        private GradeInfo currentGrade; // Um die aktuell bearbeitete Note zu speichern
        private DatabaseService _databaseService;
        private string _yearName;

        public SubjectDetailPage(string subjectName, string yearName)
        {
            InitializeComponent();
            notenList = new List<GradeInfo>();

            // Setze den Titel des Faches in das Label
            subjectNameLabel.Text = subjectName;
            _yearName = yearName;

            // Datenbankpfad festlegen und DatabaseService initialisieren
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "grades.db");
            _databaseService = new DatabaseService(dbPath);

            // Noten laden
            LoadGradesAsync();
        }

        private async void LoadGradesAsync()
        {
            // Lade die Noten für das spezifische Fach und das aktuelle Jahr
            notenList = await _databaseService.GetGradesBySubjectAndYearAsync(subjectNameLabel.Text, _yearName);

            UpdateNoteList(); // Aktualisiere die Anzeige
            CalculateAndDisplayAverage(); // Berechne den Durchschnitt und aktualisiere das Label
        }


        // Methode zur Berechnung des Durchschnitts
        private void CalculateAndDisplayAverage()
        {
            if (notenList != null && notenList.Count > 0)
            {
                double totalGrades = 0;
                double totalWeight = 0;

                foreach (var grade in notenList)
                {
                    if (double.TryParse(grade.Grade, out double gradeValue) && double.TryParse(grade.Weight, out double weightValue))
                    {
                        totalGrades += gradeValue * (weightValue / 100);
                        totalWeight += weightValue;
                    }
                }

                // Berechne den gewichteten Durchschnitt
                double averageGrade = totalGrades / (totalWeight / 100);
                averageGradeLabel.Text = $"Durchschnitt: {averageGrade:F2}"; // Zeige den Durchschnitt an, auf 2 Dezimalstellen gerundet
            }
            else
            {
                averageGradeLabel.Text = "Keine Noten vorhanden.";
            }
        }

        // Methode zum Anzeigen der Eingabefelder
        private void OnShowInputFieldsClicked(object sender, EventArgs e)
        {
            inputFields.IsVisible = true; // Eingabefelder anzeigen
            ClearInputFields(); // Eingabefelder zurücksetzen
        }

        // Methode zum Hinzufügen einer Note
        private async void OnAddNoteClicked(object sender, EventArgs e)
        {
            string title = titleEntry.Text;
            string gradeStr = gradeEntry.Text;
            string weightStr = weightEntry.Text;

            // Überprüfen, ob die Eingaben gültig sind
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(gradeStr) ||
                !double.TryParse(gradeStr, out double grade) || grade < 1 || grade > 6 ||
                !double.TryParse(weightStr, out double weight) || weight <= 0 || weight > 100)
            {
                await DisplayAlert("Fehler", "Bitte geben Sie einen gültigen Titel, eine Note (1-6) und eine Gewichtung (0-100%) ein.", "OK");
                return;
            }

            if (currentGrade != null) // Bearbeiten-Modus
            {
                currentGrade.Title = title;
                currentGrade.Grade = gradeStr;
                currentGrade.Weight = weightStr;
                await _databaseService.UpdateGradeAsync(currentGrade);
                currentGrade = null;
            }
            else // Hinzufügen-Modus
            {
                var newGrade = new GradeInfo
                {
                    Title = title,
                    Grade = gradeStr,
                    Weight = weightStr,
                    SubjectName = subjectNameLabel.Text,
                    YearName = _yearName
                };

                await _databaseService.AddGradeAsync(newGrade); // Neue Note hinzufügen
            }

            LoadGradesAsync(); // Liste neu laden

            ClearInputFields();
            inputFields.IsVisible = false;
        }

        // Methode zur Bearbeitung einer Note
        private void OnEditNoteClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var selectedGrade = (GradeInfo)button.BindingContext;

            // Setze die Eingabefelder mit den aktuellen Werten
            titleEntry.Text = selectedGrade.Title;
            gradeEntry.Text = selectedGrade.Grade;
            weightEntry.Text = selectedGrade.Weight;
            currentGrade = selectedGrade; // Setze die aktuelle Note für die Bearbeitung
            inputFields.IsVisible = true; // Eingabefelder anzeigen
        }

        // Methode zur Abbrechen der Noteneingabe
        private void OnCancelNoteClicked(object sender, EventArgs e)
        {
            ClearInputFields(); // Eingabefelder zurücksetzen
            inputFields.IsVisible = false; // Eingabefelder ausblenden
        }

        // Methode zur Aktualisierung der Notenliste auf der Seite
        private void UpdateNoteList()
        {
            gradeListView.ItemsSource = null; // Setze die Source auf null, um die Liste neu zu laden
            gradeListView.ItemsSource = notenList; // Aktualisiere die Anzeige mit den neuen Werten
        }

        // Methode zum Löschen einer Note
        private async void OnDeleteNoteClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var selectedGrade = (GradeInfo)button.BindingContext;

            // Entferne die gewählte Note aus der Liste
            await _databaseService.DeleteGradeAsync(selectedGrade); // Löschen der Note aus der DB
            LoadGradesAsync(); // Aktualisiere die Anzeige
        }

        // Methode zum Zurücksetzen der Eingabefelder
        private void ClearInputFields()
        {
            titleEntry.Text = string.Empty;
            gradeEntry.Text = string.Empty;
            weightEntry.Text = string.Empty;
            currentGrade = null; // Setze die aktuelle Note zurück
        }
    }
}
