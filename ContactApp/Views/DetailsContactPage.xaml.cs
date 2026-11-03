using ContactApp.ViewModels;
using Contact = ContactApp.Models.Contact;

namespace ContactApp.Views;

[QueryProperty(nameof(ContactId), "Id")]
public partial class DetailsContactPage : ContentPage
{

	private Contact? _contact;
    private readonly ContactsViewModel _viewModel;

    public string ContactId
    {
        set => LoadContact(value);
    }

    public DetailsContactPage(ContactsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
	}

    private void LoadContact(string id)
    {
        _contact = _viewModel.Contacts.FirstOrDefault(c => c._Id == id);
        BindingContext = _contact;
    }

    private async void BtnViewMap_Clicked(object sender, EventArgs e)
    {
        if (_contact ==  null)
        {
            await DisplayAlertAsync("Error", "Contact not available.", "Ok");
            return;
        }

        if (string.IsNullOrEmpty(_contact.Address))
        {
            await DisplayAlertAsync("Error", "Address not available.", "Ok");
            return;
        }

        if (_contact.Coordinates == null)
        {
            Location? location = await _contact.GetCoordinatesAsync();

            if (location == null)
            {
                await DisplayAlertAsync("Error", "Coordinates not available.", "Ok");
                return;
            }

            OpenMap(_contact);
            return;
        }

        OpenMap(_contact);
    }

    private async void OpenMap(Contact contact)
    {
        if (contact?.Coordinates == null)
        {
            return;
        }

        bool opened = await Map.Default.TryOpenAsync(contact.Coordinates, new MapLaunchOptions { Name = "Contact" });

        if (!opened)
        {
            await DisplayAlertAsync("Error", "No maps app available.", "OK");
            return;
        }
    }
}