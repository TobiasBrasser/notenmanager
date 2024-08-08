using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Notenmanager
{
    public partial class Kontakt : ContentPage
    {
        public Kontakt()
        {
            InitializeComponent();
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            // Hier kannst du den ausgewählten Datumswert verwenden
            DateTime selectedDate = e.NewDate;

            // Beispiel: Anzeige des ausgewählten Datums im Label
            SelectedDateLabel.Text = $"Ausgewähltes Datum: {selectedDate.ToString("D")} ";
        }

        private async void OnSendMessageClicked(object sender, EventArgs e)
        {
            string firstName = firstNameEntry.Text;
            string lastName = lastNameEntry.Text;
            string email = emailEntry.Text;
            string message = messageEntry.Text;
            bool consentGiven = consentSwitch.IsToggled;

            // Einfache Validierung
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
            {
                await DisplayAlert("Fehler", "Bitte füllen Sie alle Felder aus.", "OK");
                return;
            }

            // Zusätzliche Validierung
            if (!IsValidEmail(email))
            {
                await DisplayAlert("Fehler", "Bitte geben Sie eine gültige E-Mail-Adresse ein.", "OK");
                return;
            }

            // Hier kannst du die Logik für das Senden der Nachricht implementieren
            // Zum Beispiel: Validierung, Speichern der Daten, API-Aufruf, etc.

            // Nachricht senden
            await SendEmail(firstName, lastName, email, message, consentGiven);
        }

        private bool IsValidEmail(string email)
        {
            // Einfache Validierung für eine gültige E-Mail-Adresse
            return !string.IsNullOrWhiteSpace(email) && email.Contains("@") && email.Contains(".");
        }

        private async Task SendEmail(string firstName, string lastName, string email, string message, bool consentGiven)
        {
            try
            {
                var emailData = new
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Message = message,
                    ConsentGiven = consentGiven
                };

                var json = JsonConvert.SerializeObject(emailData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.PostAsync("https://dein-email-server.com/api/send-email", content);

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Erfolg", "Nachricht wurde erfolgreich gesendet.", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Fehler", "Fehler beim Senden der Nachricht.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fehler", $"Ein Fehler ist aufgetreten: {ex.Message}", "OK");
            }
        }
    }
}
