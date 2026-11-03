using ContactApp.Models;
using ContactApp.ViewModels;
using Contact = ContactApp.Models.Contact;

namespace ContactApp.Views;

[QueryProperty(nameof(ContactId), "Id")]
public partial class EditContactPage : ContentPage
{
    private Contact? _contact;
    private readonly ContactsViewModel _viewModel;

    public string ContactId
    {
        set => LoadContact(value);
    }

    public EditContactPage(ContactsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
    }

    private void LoadContact(string id)
    {
        _contact = _viewModel.Contacts.FirstOrDefault(c => c._Id == id);

        if(_contact == null)
        {
            DisplayAlertAsync("Error", "Contact not found.", "OK");
            Shell.Current.GoToAsync("..");
            return;
        }

        contactControl.Name = _contact?.Name ?? string.Empty;
        contactControl.Phone = _contact?.PhoneNumber ?? string.Empty;
        contactControl.Email = _contact?.Email ?? string.Empty;
        contactControl.Address = _contact?.Address ?? string.Empty;
    }

    public async void BtnUpdate_Clicked(object sender, EventArgs e)
    {
        if (_contact != null)
        {
            _contact.Name = contactControl.Name;
            _contact.PhoneNumber = contactControl.Phone;
            _contact.Email = contactControl.Email;
            _contact.Address = contactControl.Address;

            var isUpdated = await _viewModel.UpdateContact(_contact);

            if (isUpdated)
            {
                await DisplayAlertAsync("Success", "Contact updated successfully.", "OK");
            }
        }

        await Shell.Current.GoToAsync("..");
    }

    public void BtnCancel_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }

    public void ContactControl_OnError(object sender, string errorMessage)
    {
        DisplayAlertAsync("Error", errorMessage, "OK");
    }

}