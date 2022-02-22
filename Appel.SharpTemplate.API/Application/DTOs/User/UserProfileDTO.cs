using Appel.SharpTemplate.API.Extensions;
using Appel.SharpTemplate.Domain.Entities;
using System.Text.Json.Serialization;

namespace Appel.SharpTemplate.API.Application.DTOs.User
{
    public class UserProfileDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        [JsonConverter(typeof(JsonConverterExtensions.NumbersOnly))]
        public string IdentityDocument { get; set; }

        [JsonConverter(typeof(JsonConverterExtensions.NumbersOnly))]
        public string CpfCnpj { get; set; }

        [JsonConverter(typeof(JsonConverterExtensions.NumbersOnly))]
        public string ResponsibleCpf { get; set; }

        [JsonConverter(typeof(JsonConverterExtensions.NumbersOnly))]
        public string ResponsibleName { get; set; }

        [JsonConverter(typeof(JsonConverterExtensions.NumbersOnly))]
        public string StateRegistration { get; set; }

        [JsonConverter(typeof(JsonConverterExtensions.NumbersOnly))]
        public string Phone { get; set; }

        [JsonConverter(typeof(JsonConverterExtensions.NumbersOnly))]
        public string CellPhone { get; set; }

        public string Address { get; set; }

        public string AddressNumber { get; set; }

        public string AddressComplement { get; set; }

        public string Neighborhood { get; set; }

        public string City { get; set; }

        [JsonConverter(typeof(JsonConverterExtensions.NumbersOnly))]
        public string ZipCode { get; set; }

        public string FederativeUnit { get; set; }

        public UserType Type { get; set; }
    }
}