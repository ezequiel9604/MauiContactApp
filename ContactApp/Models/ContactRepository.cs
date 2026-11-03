
namespace ContactApp.Models
{
    public static class ContactRepository
    {
        public static List<Contact> contacts =
        [
            new("John Doe", "809-111-2222"),
            new("Samantha Adens", "809-111-3333"),
            new("Maria Lopez", "809-111-4444", "maria.lopez@example.com"),
            new("Tom Hanks", "809-111-5555", "tom.hanks@example.com"),
            new("Ezequiel Martinez", "809-111-6666", "ezequiel.martinez@example.com", "Av. Winston Churchill, Santo Domingo, República Dominicana")
        ];

        public static List<Contact> GetContacts()
        {
            return contacts;
        }

        public static Contact? GetContactById(string id)
        {
            return contacts.FirstOrDefault(c => c._Id == id);
        }

        public static void UpdateContact(Contact contact)
        {
            var existingContact = GetContactById(contact._Id);
            if (existingContact != null)
            {
                existingContact.Name = contact.Name;
                existingContact.PhoneNumber = contact.PhoneNumber;
                existingContact.Email = contact.Email;
                existingContact.Initials = contact.GetInitials(contact.Name);
            }
        }

        public static void AddContact(Contact contact)
        {
            contacts.Add(contact);
        }

        public static void DeleteContact(string id)
        {
            var contactToDelete = GetContactById(id);
            if (contactToDelete != null)
            {
                contactToDelete.IsDeleted = true;
            }
        }

        public static List<Contact> SearchContacts(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return GetContacts();
            }
            searchText = searchText.ToLower();
            return contacts.Where(c => c.Name.ToLower().Contains(searchText) || c.PhoneNumber.Contains(searchText)).ToList();
        }

    }
}
