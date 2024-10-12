using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Linq;

namespace Notenmanager
{
    public partial class SubjectDetailPage : ContentPage
    {
        private List<GradeInfo> notenList;
        private GradeInfo currentGrade; // Um die aktuell bearbeitete Note zu speichern

        public SubjectDetailPage(string subjectName)
        {
            InitializeComponent();
            notenList = new List<GradeInfo>();

            // Setze den Titel des Faches in das Label
            subjectNameLabel.Text = subjectName;
        }

        // Methode zum Anzeigen der Eingabefelder
        private void OnShowInputFieldsClicked(object sender, EventArgs e)
        {
            inputFields.IsVisible = true; // Eingabefelder anzeigen
            ClearInputFields(); // Eingabefelder zur�cksetzen
        }

        // Methode zum Hinzuf�gen einer Note
        private void OnAddNoteClicked(object sender, EventArgs e)
        {
            string title = titleEntry.Text;
            string gradeStr = gradeEntry.Text;
            string weightStr = weightEntry.Text;

            // �berpr�fen, ob die Eingaben g�ltig sind
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(gradeStr) ||
                !double.TryParse(gradeStr, out double grade) || grade < 1 || grade > 6 ||
                !double.TryParse(weightStr, out double weight) || weight <= 0 || weight > 100)
            {
                DisplayAlert("Fehler", "Bitte geben Sie einen g�ltigen Titel, eine Note (1-6) und eine Gewichtung (0-100%) ein.", "OK");
                return;
            }

            // Hier wird die neue Instanz von GradeInfo mit Gewicht erstellt
            if (currentGrade != null) // Bearbeiten-Modus
            {
                currentGrade.Title = title;
                currentGrade.Grade = gradeStr;
                currentGrade.Weight = weightStr;
                currentGrade = null; // Bearbeiten abgeschlossen
            }
            else // Hinzuf�gen-Modus
            {
                var newGrade = new GradeInfo(title, gradeStr, weightStr);
                notenList.Add(newGrade);
            }

            // Aktualisiere die Anzeige der Noten
            UpdateNoteList();

            // Eingabefelder zur�cksetzen und ausblenden
            ClearInputFields();
            inputFields.IsVisible = false; // Eingabefelder ausblenden
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
            currentGrade = selectedGrade; // Setze die aktuelle Note f�r die Bearbeitung
            inputFields.IsVisible = true; // Eingabefelder anzeigen
        }

        // Methode zur Abbrechen der Noteneingabe
        private void OnCancelNoteClicked(object sender, EventArgs e)
        {
            ClearInputFields(); // Eingabefelder zur�cksetzen
            inputFields.IsVisible = false; // Eingabefelder ausblenden
        }

        // Methode zur Aktualisierung der Notenliste auf der Seite
        private void UpdateNoteList()
        {
            gradeListView.ItemsSource = null; // Setze die Source auf null, um die Liste neu zu laden
            gradeListView.ItemsSource = notenList; // Aktualisiere die Anzeige mit den neuen Werten
            // Hier kannst du auch die Berechnung des Durchschnitts hinzuf�gen, falls n�tig
        }

        // Methode zum L�schen einer Note
        private void OnDeleteNoteClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var selectedGrade = (GradeInfo)button.BindingContext;

            // Entferne die gew�hlte Note aus der Liste
            notenList.Remove(selectedGrade);
            UpdateNoteList(); // Aktualisiere die Anzeige
        }
        // Methode zum Zur�cksetzen der Eingabefelder
        private void ClearInputFields()
        {
            titleEntry.Text = string.Empty;
            gradeEntry.Text = string.Empty;
            weightEntry.Text = string.Empty;
            currentGrade = null; // Setze die aktuelle Note zur�ck
        }
    }
}
