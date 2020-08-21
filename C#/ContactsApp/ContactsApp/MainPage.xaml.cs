using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ContactsApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            searchBar.Text = String.Empty;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // get contacts everytime the app opens and comes back to this page
            lstContacts.ItemsSource = await App.Database.GetAllContactsAsync(searchBar.Text);
        }

        async private void DeleteMenuItem_Clicked(object sender, EventArgs e)
        {
            Contact con = (sender as MenuItem).CommandParameter as Contact;
            bool answer = await DisplayAlert("Warning", "Are you sure you want to delete " + con.FirstName + " " + con.LastName + "?", "Yes", "No");

            // delete contact if the user confirms it
            if (answer)
            {
                await App.Database.DeleteContactAsync(con);
                lstContacts.ItemsSource = await App.Database.GetAllContactsAsync(searchBar.Text);
            }
        }

        private void AddToolbarItem_Clicked(object sender, EventArgs e)
        {
            // add new contact
            Navigation.PushModalAsync(new NavigationPage(new AddEditContactPage(new Contact(), true))
            {
                BarBackgroundColor = Color.ForestGreen,
                BarTextColor = Color.Black
            });
        }

        private void LstContacts_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Contact con = e.Item as Contact;

            // display contact info and will be available for edit
            Navigation.PushModalAsync(new NavigationPage(new AddEditContactPage(con, false))
            {
                BarBackgroundColor = Color.ForestGreen,
                BarTextColor = Color.Black
            });
        }

        async private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            lstContacts.ItemsSource = await App.Database.GetAllContactsAsync(e.NewTextValue);
        }
    }
}
