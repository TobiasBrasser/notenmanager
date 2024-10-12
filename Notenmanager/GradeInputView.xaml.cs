using Microsoft.Maui.Controls;
using System;

namespace Notenmanager
{
    public partial class GradeInputView : ContentView
    {
        public GradeInputView()
        {
            InitializeComponent();
        }

        public string Title => titleEntry.Text;
        public string Grade => gradeEntry.Text;
        public string Remark => remarkEditor.Text;

        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            // Validierung der Eingaben
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Grade) || !double.TryParse(Grade, out double grade) || grade < 1 || grade > 6)
            {
                await Application.Current.MainPage.DisplayAlert("Fehler", "Bitte geben Sie sowohl Titel als auch eine gültige Note (1-6) ein.", "OK");
                return;
            }

            // Wenn alles in Ordnung ist, schließe den Dialog
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
