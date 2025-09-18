namespace SmsScheduler
{
    public partial class App : Application
    {
        public static Database Database { get; private set; }

        public App()
        {
            InitializeComponent();

            string dbPath = Path.Combine(
                FileSystem.AppDataDirectory, "contacts.db3");

            Database = Database.GetInstance(dbPath);

            MainPage = new NavigationPage(new MainPage());
        }
    }
}
