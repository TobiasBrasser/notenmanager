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
        private GradeInfo currentGrade; 
        private DatabaseService _databaseService;
        private string _yearName;

        public SubjectDetailPage(string subjectName, string yearName)
        {
            InitializeComponent();
            notenList = new List<GradeInfo>();

            subjectNameLabel.Text = subjectName;
            _yearName = yearName;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "grades.db");
            _databaseService = new DatabaseService(dbPath);

            LoadGradesAsync();
        }

        private async void LoadGradesAsync()
        {
            notenList = await _databaseService.GetGradesBySubjectAndYearAsync(subjectNameLabel.Text, _yearName);

            UpdateNoteList(); 
            CalculateAndDisplayAverage(); 
        }

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

                double averageGrade = totalGrades / (totalWeight / 100);
                averageGradeLabel.Text = $"Durchschnitt: {averageGrade:F2}"; 
            }
            else
            {
                averageGradeLabel.Text = "Keine Noten vorhanden.";
            }
        }

        private void OnShowInputFieldsClicked(object sender, EventArgs e)
        {
            inputFields.IsVisible = true; 
            ClearInputFields();
        }
        private async void OnAddNoteClicked(object sender, EventArgs e)
        {
            string title = titleEntry.Text;
            string gradeStr = gradeEntry.Text;
            string weightStr = weightEntry.Text;

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(gradeStr) ||
                !double.TryParse(gradeStr, out double grade) || grade < 1 || grade > 6 ||
                !double.TryParse(weightStr, out double weight) || weight <= 0 || weight > 100)
            {
                await DisplayAlert("Fehler", "Bitte geben Sie einen gültigen Titel, eine Note (1-6) und eine Gewichtung (0-100%) ein.", "OK");
                return;
            }

            bool gradeExists = await _databaseService.GradeExistsAsync(title, subjectNameLabel.Text, _yearName);
            if (gradeExists)
            {
                await DisplayAlert("Fehler", $"Die Note '{title}' existiert bereits für das Fach '{subjectNameLabel.Text}'", "OK");
                return; 
            }

            if (currentGrade != null) 
            {
                currentGrade.Title = title;
                currentGrade.Grade = gradeStr;
                currentGrade.Weight = weightStr;
                await _databaseService.UpdateGradeAsync(currentGrade);
                currentGrade = null;
            }
            else 
            {
                var newGrade = new GradeInfo
                {
                    Title = title,
                    Grade = gradeStr,
                    Weight = weightStr,
                    SubjectName = subjectNameLabel.Text,
                    YearName = _yearName
                };

                await _databaseService.AddGradeAsync(newGrade);
            }

            LoadGradesAsync();

            ClearInputFields();
            inputFields.IsVisible = false;
        }

        private void OnEditNoteClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var selectedGrade = (GradeInfo)button.BindingContext;

            titleEntry.Text = selectedGrade.Title;
            gradeEntry.Text = selectedGrade.Grade;
            weightEntry.Text = selectedGrade.Weight;
            currentGrade = selectedGrade; 
            inputFields.IsVisible = true;
        }

        private void OnCancelNoteClicked(object sender, EventArgs e)
        {
            ClearInputFields(); 
            inputFields.IsVisible = false; 
        }

        private void UpdateNoteList()
        {
            gradeListView.ItemsSource = null; 
            gradeListView.ItemsSource = notenList; 
        }
        private async void OnDeleteNoteClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var selectedGrade = (GradeInfo)button.BindingContext;

            await _databaseService.DeleteGradeAsync(selectedGrade); 
            LoadGradesAsync();
        }

        private void ClearInputFields()
        {
            titleEntry.Text = string.Empty;
            gradeEntry.Text = string.Empty;
            weightEntry.Text = string.Empty;
            currentGrade = null; 
        }

        private async void OnBackToSubject(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }



    }
}
