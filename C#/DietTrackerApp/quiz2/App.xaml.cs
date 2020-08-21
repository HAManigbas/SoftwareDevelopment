using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;

namespace quiz2
{
    public partial class App : Application
    {
        private static LocalDB database;

        public static LocalDB Database
        {
            get
            {
                if (database == null)
                    database = new LocalDB(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Diet.db3"));

                return database;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
