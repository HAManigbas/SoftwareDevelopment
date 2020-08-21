using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContactsApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEditContactPage : ContentPage
    {
        private Contact _contact;
        public AddEditContactPage(Contact contact, bool isNew)
        {
            InitializeComponent();

            _contact = contact;

            // check if adding a new contact or showing and existing one
            if (isNew)
                this.Title = "New Contact";
            else
            {
                this.Title = "Contact";
                this.BindingContext = _contact;

                // disable the fields unless user wants to edit it
                entryFName.IsEnabled = false;
                entryLName.IsEnabled = false;
                entryPhone.IsEnabled = false;
                entryEmail.IsEnabled = false;
                switchBlocked.IsEnabled = false;

                ToolbarItem toolbarItem = new ToolbarItem()
                {
                    Text = "Edit"
                };
                toolbarItem.Clicked += EditToolbarItem_Clicked;
                this.ToolbarItems.Add(toolbarItem);
            }
        }

        private void EditToolbarItem_Clicked(object sender, EventArgs e)
        {
            // enable all fields for editing
            entryFName.IsEnabled = true;
            entryLName.IsEnabled = true;
            entryPhone.IsEnabled = true;
            entryEmail.IsEnabled = true;
            switchBlocked.IsEnabled = true;
        }

        async private void SaveButton_Clicked(object sender, EventArgs e)
        {
            // save or add the contact
            // first and last names are required
            if (!String.IsNullOrWhiteSpace(entryFName.Text) && !String.IsNullOrWhiteSpace(entryLName.Text))
            {
                _contact.FirstName = entryFName.Text;
                _contact.LastName = entryLName.Text;
                _contact.PhoneNumber = entryPhone.Text;
                _contact.Email = entryEmail.Text;
                _contact.IsBlocked = switchBlocked.On;

                await App.Database.SaveContactAsync(_contact);

                await Navigation.PopModalAsync();
            }
            else
                await DisplayAlert("ERROR", "Please enter the name.", "OK");
        }
    }
}