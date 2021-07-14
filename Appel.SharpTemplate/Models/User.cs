namespace Appel.SharpTemplate.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string IdentityDocument { get; set; }
        public string CpfCnpj { get; set; }
        public string ResponsibleCpf { get; set; }
        public string ResponsibleName { get; set; }
        public string StateRegistration { get; set; }
        public string Phone { get; set; }
        public string CellPhone { get; set; }
        public string Address { get; set; }
        public string AddressNumber { get; set; }
        public string AddressComplement { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string FederativeUnit { get; set; }
        public UserType Type { get; set; }
        public UserRole Role { get; set; }
    }

    public enum UserType : byte
    {
        Physical = 0,
        Legal = 1
    }

    public enum UserRole : byte
    {
        Customer = 0,
        Admin = 1
    }
}
