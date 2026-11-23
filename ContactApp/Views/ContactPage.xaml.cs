using ContactApp.Models;
using ContactApp.ViewModels;
using Xamarin.Google.Crypto.Tink.Signature;
using Communication = Microsoft.Maui.ApplicationModel.Communication;
using Contact = ContactApp.Models.Contact;

namespace ContactApp.Views;

public partial class ContactPage : ContentPage
{
    private bool _isSwiping;
    private CancellationTokenSource? _searchCts;
    private string _currentSearch = string.Empty;

    private readonly ContactsViewModel _viewModel;

    public ContactPage(ContactsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // await _viewModel.LoadCommand.ExecuteAsync(null);
        //if (!_phoneContactsImported)
        //{
        //    _phoneContactsImported = true;
        //    await ImportPhoneContactsAsync();
        //}
        RefreshList();
    }

    private async void RefreshList()
    {
        if (string.IsNullOrWhiteSpace(_currentSearch))
        {
            await _viewModel.LoadCommand.ExecuteAsync(null);
        }
        else
        {
            await _viewModel.GetContactByName(_currentSearch);
        }

        BindingContext = _viewModel;
    }

    private async Task<List<Contact>> ImportPhoneContactsAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.ContactsRead>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.ContactsRead>();

        if (status != PermissionStatus.Granted)
        {
            await DisplayAlertAsync("Permission Denied",
                "Access to contacts is denied. Please enable it in settings.", "OK");
            return new List<Contact>();
        }

        try
        {
            var phoneContacts = await Communication.Contacts.Default.GetAllAsync();
            var known = ContactRepository.GetContacts()
                .Select(c => Normalize(c.PhoneNumber))
                .Where(n => n.Length > 0)
                .ToHashSet();

            List<Contact> importedContacts = new List<Contact>();

            foreach (var pc in phoneContacts ?? Enumerable.Empty<Communication.Contact>())
            {
                string phone = pc.Phones.FirstOrDefault()?.PhoneNumber ?? string.Empty;
                var key = Normalize(phone);
                if (key.Length == 0 || !known.Add(key)) continue;

                string name = !string.IsNullOrWhiteSpace(pc.DisplayName)
                    ? pc.DisplayName
                    : $"{pc.GivenName} {pc.FamilyName}".Trim();

                string email = pc.Emails.FirstOrDefault()?.EmailAddress ?? string.Empty;

                var existingContact = _viewModel.Contacts.FirstOrDefault(c => c.PhoneNumber == phone);

                if (existingContact == null)
                {
                    importedContacts.Add(new Contact(name, phone, email));
                }

               

            }

            return importedContacts;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");  
        }

        return new List<Contact>();
    }

    private static string Normalize(string? n) =>
        new string((n ?? string.Empty).Where(char.IsDigit).ToArray());


    public async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        try { await Task.Delay(300, _searchCts.Token); }
        catch (TaskCanceledException) { return; }

        _currentSearch = e.NewTextValue?.Trim() ?? string.Empty;
        RefreshList();
    }

    public async void OnDeleteSwipeInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem { BindingContext: Contact contact })
        {
            bool confirm = await DisplayAlertAsync("Delete Contact",
                $"Are you sure you want to delete {contact.Name}?", "Yes", "No");
            if (!confirm) return;

            var isDeleted = await _viewModel.DeleteContact(contact._Id);
            if (isDeleted)
            {
                RefreshList();
            }
        }
    }

    public async void OnEditSwipeInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem { BindingContext: Contact contact })
            await Shell.Current.GoToAsync($"{nameof(EditContactPage)}?Id={contact._Id}");
    }

    private void OnSwipeEnded(object sender, SwipeEndedEventArgs e) => _isSwiping = true;

    public async void OnContactTapped(object sender, TappedEventArgs e)
    {
        if (_isSwiping) { _isSwiping = false; return; }
        if (sender is BindableObject { BindingContext: Contact contact })
            await Shell.Current.GoToAsync($"{nameof(DetailsContactPage)}?Id={contact._Id}");
    }

    public async void BtnAddContact_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddContactPage));
    }

    public async void BtnImportContacts_Clicked(object sender, EventArgs e)
    {
        var importedContacts = await ImportPhoneContactsAsync();

        if (importedContacts.Count == 0)
        {
            await DisplayAlertAsync("No Contacts Found", "No new contacts were found to import.", "OK");
            return;
        }

        bool confirm = await DisplayAlertAsync("Import Contacts",
            $"Are you sure you want to import {importedContacts.Count} contacts?", "Yes", "No");

        if (!confirm) return;
        
        await _viewModel.AddImportedContacts(importedContacts);
        
        RefreshList();
    }
}