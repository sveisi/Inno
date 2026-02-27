namespace Inno.Models
{
    public class Address : BaseEntity
    {
        public int CustomerId { get; set; }
        public int CityId { get; set; }
        public string AddressLine { get; set; } = null!;
        public string ZipCode { get; set; }
        public bool IsDefault { get; set; }

        public Customer Customer { get; set; }
        public Region City { get; set; }
    }
}