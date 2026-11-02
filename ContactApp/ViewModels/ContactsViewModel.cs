using ContactApp.Services;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Contact = ContactApp.Models.Contact;

namespace ContactApp.ViewModels
{
    public partial class ContactsViewModel : ObservableObject
    {
        private readonly ContactService _service;
        public ObservableCollection<Contact> Contacts { get; } = [];

        public ContactsViewModel(ContactService service)
        {
            _service = service;
        }

        [RelayCommand]
        async Task LoadAsync()
        {
            try
            {
                var contacts = await _service.GetContactsAsync();
                Contacts.Clear();

                foreach (var contact in contacts)
                {
                    Contacts.Add(contact);
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"---------------- {ex}", "CONNECTION");
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "Ok");
            }
        }

        public async Task GetContactByName(string name)
        {
            try
            {
                var contacts = await _service.GetContactByNameAsync(name);
                Contacts.Clear();
                foreach (var c in contacts)
                {
                    Contacts.Add(c);
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"---------------- {ex}","CONNECTION");
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "Ok");
            }
        }

        public async Task<bool> UpdateContact(Contact contact)
        {
            try
            {
                var response = await _service.UpdateContactAsync(contact);
                if (response.IsSuccessStatusCode)
                {
                    var existingContact = Contacts.FirstOrDefault(c => c.Id == contact.Id);
                    if (existingContact != null)
                    {
                        existingContact.Name = contact.Name;
                        existingContact.PhoneNumber = contact.PhoneNumber;
                        existingContact.Email = contact.Email;
                        existingContact.Address = contact.Address;
                        // Notify the UI that the collection has changed
                        OnPropertyChanged(nameof(Contacts));

                        return true;
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Error", "Failed to update contact.", "Ok");
                }

                return false;
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"---------------- {ex}", "CONNECTION");
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "Ok");
                return false;
            }
        }

        public async Task<bool> DeleteContact(int contactId)
        {
            try
            {
                var response = await _service.DeleteContactAsync(contactId);
                if (response.IsSuccessStatusCode)
                {
                    var existingContact = Contacts.FirstOrDefault(c => c.Id == contactId);
                    if (existingContact != null)
                    {
                        Contacts.Remove(existingContact);
                        return true;
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Error", "Failed to delete contact.", "Ok");
                }
                return false;
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"---------------- {ex}", "CONNECTION");
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "Ok");
                return false;
            }
        }

        public async Task<bool> AddContact(Contact contact)
        {
            try
            {
                var response = await _service.AddContactAsync(contact);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Error", "Failed to add contact.", "Ok");
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"---------------- {ex}", "CONNECTION");
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "Ok");
            }

            return false;
        }
    }
}
