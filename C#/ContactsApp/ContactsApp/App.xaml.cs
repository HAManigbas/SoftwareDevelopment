using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;

namespace ContactsApp
{
    public partial class App : Application
    {
        private static DataHelper database;

        public static DataHelper Database
        {
            get
            {
                if (database == null)
                    database = new DataHelper(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Contact.db3"));

                return database;
            }
        }
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = Color.ForestGreen,
                BarTextColor = Color.Black
            };
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
