using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace Notenmanager
{
    public partial class Notenmanager : ContentPage
    {
        public Notenmanager()
        {
            InitializeComponent();
        }

        // Zähler für die Fächer
        private int subjectCounter = 1;

        // Methode für das Hinzufügen eines neuen Faches
        private async void OnAddSubjectClicked(object sender, EventArgs e)
        {
            // Fragen Sie den Benutzer nach dem Fachnamen
            string subjectName = await DisplayPromptAsync("Fach hinzufügen", "Bitte geben Sie den Namen des Fachs ein:",
                maxLength: 30,
                keyboard: Keyboard.Default);

            if (!string.IsNullOrWhiteSpace(subjectName))
            {
                Frame newFrame = new Frame
                {
                    BorderColor = Color.FromHex("#4630D9"),
                    BackgroundColor = Color.FromHex("#4630D9"),
                    CornerRadius = 5,
                    Margin = new Thickness(5),
                    Padding = new Thickness(10)
                };


                newFrame.Content = new StackLayout
                {
                    Children =
                    {
                        new Label
                        {
                            Text = subjectName,
                            FontSize = 18,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = Colors.White // Setze die Textfarbe auf Weiß
                        },
                        new Label
                        {
                            Text = "Tippen, um Noten einzugeben",
                            HorizontalOptions = LayoutOptions.Center,
                            TextColor = Colors.White // Setze die Textfarbe auf Weiß
                        }
                    }
                };


                // Tippen auf das Fach öffnet die Detailseite für die Noteneingabe
                newFrame.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => OnSubjectTapped(subjectName))
                });

                // Füge das neue Fach zur Hauptseite hinzu
                mainLayout.Children.Insert(mainLayout.Children.Count - 1, newFrame); // Füge das Fach vor dem letzten Element ein
            }
        }

        // Methode zum Öffnen der SubjectDetailPage
        private void OnSubjectTapped(string subjectName)
        {
            // Öffne die Detailseite, um Noten einzugeben
            Navigation.PushAsync(new SubjectDetailPage(subjectName));
        }
    }
}
