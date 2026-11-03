using Microsoft.Maui.Devices.Sensors;

namespace ContactApp.Models
{
    public class Contact
    {
        public string _Id { get; set; } = string.Empty;
        public string Initials { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public Location? Coordinates { get; set; } = null;

        public bool IsDeleted { get; set; } = false;

        public Contact() { }

        public Contact(string name, string phone)
        {
            Initials = GetInitials(name);
            Name = name;
            PhoneNumber = phone;
        }

        public Contact(string name, string phone, string email) :
           this(name, phone)
        {
            Email = email;
        }

        public Contact(string name, string phone, string email, string address) :
           this(name, phone)
        {
            Email = email;
            Address = address;

            if (!string.IsNullOrEmpty(address))
                SetCoordinatesAsync(address);

        }

        public Contact(Contact contact)
        {
            _Id = contact._Id;
            Initials = contact.Initials;
            Name = contact.Name;
            PhoneNumber = contact.PhoneNumber;
            Email = contact.Email ?? string.Empty;
            Address = contact.Address ?? string.Empty;

            if (!string.IsNullOrEmpty(contact.Address))
                SetCoordinatesAsync(contact.Address);
        }

        public string GetInitials(string name)
        {
            string[] names = name.Split(' ');
            if (names.Length < 2)
            {
                return names[0][0].ToString().ToUpper();
            }
            return $"{names[0][0]}{names[1][0]}".ToUpper();
        }

        public async void SetCoordinatesAsync(string address)
        {
            try
            {
                if (string.IsNullOrEmpty(address))
                {
                    throw new ArgumentNullException("Address cannot be null or empty to set coordinates!");
                }

                IEnumerable<Location> locations =
                    await Geocoding.Default.GetLocationsAsync(address);

                Location? location = locations?.FirstOrDefault();

                Coordinates = location;
            }
            catch (FeatureNotSupportedException)
            {
                throw new NotSupportedException("Geocoding is not supported on this device.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error setting coordinates: {ex.Message}");
            }
        }

        public async Task<Location?> GetCoordinatesAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(Address))
                {
                    throw new ArgumentNullException("Address cannot be null or empty to get coordinates!");
                }
                IEnumerable<Location> locations =
                    await Geocoding.Default.GetLocationsAsync(Address);
                Location? location = locations?.FirstOrDefault();

                if (location != null)
                {
                    Coordinates = location;
                    return location;
                }

                return null;
            }
            catch (FeatureNotSupportedException)
            {
                throw new NotSupportedException("Geocoding is not supported on this device.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting coordinates: {ex.Message}");
            }
        }

    }
}
