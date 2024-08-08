namespace Notenmanager;

public partial class Impressum : ContentPage
{
	public Impressum()
	{
		InitializeComponent();
	}

	private async void OnNavigate(object sender, EventArgs e)
	{
        await Shell.Current.GoToAsync("//MainPage");
    }
}