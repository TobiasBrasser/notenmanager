namespace Notenmanager
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("//MainPage", typeof(MainPage));
            Routing.RegisterRoute("//Notenmanager", typeof(Notenmanager));
            Routing.RegisterRoute("//Impressum", typeof(Impressum));
            Routing.RegisterRoute("//Year", typeof(Year));
            Routing.RegisterRoute(nameof(SubjectDetailPage), typeof(SubjectDetailPage));
        }

        
    }
}