using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace quiz2
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            // get data when loading the app
            base.OnAppearing();
            lstDiets.ItemsSource = await App.Database.GetAllDietsAsync(searchBar.Text);
            
        }

        async private void BtnAdd_Clicked(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(entryCat.Text) &&
                !String.IsNullOrWhiteSpace(entryType.Text) &&
                !String.IsNullOrWhiteSpace(entryServings.Text))
            {
                //Add diet to database
                await App.Database.SaveDietAsync(new Diet
                {
                    Category = entryCat.Text,
                    Type = entryType.Text,
                    Servings = Double.Parse(entryServings.Text)
                });

                entryCat.Text = String.Empty;
                entryType.Text = String.Empty;
                entryServings.Text = String.Empty;

                // when a new item is added, delete any search filter then it will display all items
                searchBar.Text = String.Empty;

                // update the list
                lstDiets.ItemsSource = await App.Database.GetAllDietsAsync(searchBar.Text);
            }
            else
            {
                await DisplayAlert("Error", "Please fill all the data", "OK");
            }
        }

        async private void DeleteMenuItem_Clicked(object sender, EventArgs e)
        {
            Diet diet = (sender as MenuItem).CommandParameter as Diet;
            await App.Database.DeleteDietAsync(diet);
            lstDiets.ItemsSource = await App.Database.GetAllDietsAsync(searchBar.Text);
        }

        private void UpdateMenuItem_Clicked(object sender, EventArgs e)
        {
            Diet diet = (sender as MenuItem).CommandParameter as Diet;
            this.BindingContext = diet;
            UpdateAddSwapVisibility();
        }

        async private void BtnUpdate_Clicked(object sender, EventArgs e)
        {
            Diet diet = this.BindingContext as Diet;
            await App.Database.SaveDietAsync(diet);
            lstDiets.ItemsSource = await App.Database.GetAllDietsAsync(searchBar.Text);
            this.BindingContext = null;
            UpdateAddSwapVisibility();
        }

        private void LstDiets_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            lstDiets.SelectedItem = null;
        }

        async private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            lstDiets.ItemsSource = await App.Database.GetAllDietsAsync(e.NewTextValue);
        }

        private void UpdateAddSwapVisibility()
        {
            btnAdd.IsVisible = btnAdd.IsVisible ? false : true;
            btnUpdate.IsVisible = btnUpdate.IsVisible ? false : true;
        }
    }
}
