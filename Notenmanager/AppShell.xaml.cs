namespace Notenmanager
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("//MainPage", typeof(MainPage));
            Routing.RegisterRoute("//Notenmanager", typeof(Notenmanager));
            Routing.RegisterRoute("//Notenmanager2", typeof(Notenmanager2));    
            Routing.RegisterRoute("//Notenmanager3", typeof(Notenmanager3));
            Routing.RegisterRoute("//Impressum", typeof(Impressum));
            Routing.RegisterRoute(nameof(SubjectDetailPage), typeof(SubjectDetailPage));
        }

        
    }
}