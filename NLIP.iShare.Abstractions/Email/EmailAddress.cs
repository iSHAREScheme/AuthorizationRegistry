namespace NLIP.iShare.Abstractions.Email
{
    public class EmailAddress
    {
        public string Address { get; }
        public string DisplayName { get; }

        public EmailAddress(string address, string displayName)
        {
            address.NotNullOrEmpty(nameof(address));
            Address = address;
            DisplayName = displayName;
        }
    }
}
