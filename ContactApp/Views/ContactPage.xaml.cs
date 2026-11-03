using ContactApp.Models;
using ContactApp.ViewModels;
using Communication = Microsoft.Maui.ApplicationModel.Communication;
using Contact = ContactApp.Models.Contact;

namespace ContactApp.Views;

public partial class ContactPage : ContentPage
{
    private bool _isSwiping;
    private bool _phoneContactsImported;
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

    private async Task ImportPhoneContactsAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.ContactsRead>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.ContactsRead>();

        if (status != PermissionStatus.Granted)
        {
            await DisplayAlertAsync("Permission Denied",
                "Access to contacts is denied. Please enable it in settings.", "OK");
            return;
        }

        try
        {
            var phoneContacts = await Communication.Contacts.Default.GetAllAsync();
            var known = ContactRepository.GetContacts()
                .Select(c => Normalize(c.PhoneNumber))
                .Where(n => n.Length > 0)
                .ToHashSet();

            foreach (var pc in phoneContacts ?? Enumerable.Empty<Communication.Contact>())
            {
                var number = pc.Phones.FirstOrDefault()?.PhoneNumber ?? string.Empty;
                var key = Normalize(number);
                if (key.Length == 0 || !known.Add(key)) continue;

                var name = !string.IsNullOrWhiteSpace(pc.DisplayName)
                    ? pc.DisplayName
                    : $"{pc.GivenName} {pc.FamilyName}".Trim();

                ContactRepository.AddContact(new Contact(name, number));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private static string Normalize(string? n) =>
        new string((n ?? string.Empty).Where(char.IsDigit).ToArray());

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
}