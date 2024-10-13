namespace Notenmanager
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnNavigate(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Year");
        }

        private async void OnBeenden(System.Object sender, System.EventArgs e)
        {
            var result = await DisplayAlert("Beenden", "Wollen Sie die App wirklich beenden?", "Ja", "Nein");
            if (result) 
            {
                System.Environment.Exit(0);
            }
        }
    }
}