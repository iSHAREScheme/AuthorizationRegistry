namespace iSHARE.Abstractions.Email
{
    public class EmailAddress
    {
        public string Address { get; set; }
        public string DisplayName { get; set; }

        public EmailAddress(string address, string displayName)
        {
            address.NotNullOrEmpty(nameof(address));
            Address = address;
            DisplayName = displayName;
        }

        public EmailAddress()
        {
            // leave the constructor here as it is used by ASP.NET Core DI
        }
    }
}
