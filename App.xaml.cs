namespace PM2Examen2Grupo1
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();


            //MainPage = new AppShell();
            MainPage = new NavigationPage(new MainPage());
        }
    }
}