using System.Net;
using System.Net.Http.Json;
using Contact = ContactApp.Models.Contact;

namespace ContactApp.Services
{
    public class ContactService
    {
        private readonly HttpClient _httpClient;
        public ContactService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Contact>> GetContactsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<Contact>>("contacts");

            if (response == null)
            {
                throw new HttpRequestException("Failed to retrieve contacts from the server.");
            }

            return response;
        }

        public async Task<List<Contact>> GetContactByNameAsync(string name)
        {
            var response = await _httpClient.GetAsync($"contacts/search?name={WebUtility.UrlEncode(name)}");
            if (response.IsSuccessStatusCode)
            {
                var contacts = await response.Content.ReadFromJsonAsync<List<Contact>>();
                return contacts ?? new List<Contact>();
            }
            else
            {
                throw new HttpRequestException($"Failed to retrieve contact by name. Status code: {response.StatusCode}");
            }
        }

        public async Task<HttpResponseMessage> AddContactAsync(Contact contact)
        {
            return await _httpClient.PostAsJsonAsync("contacts", contact);
        }

        public async Task<HttpResponseMessage> UpdateContactAsync(Contact contact)
        {
            return await _httpClient.PatchAsJsonAsync($"contacts/{contact._Id}", contact);
        }

        public async Task<HttpResponseMessage> DeleteContactAsync(string contactId)
        {
            return await _httpClient.DeleteAsync($"contacts/{contactId}");
        }
    }
}
