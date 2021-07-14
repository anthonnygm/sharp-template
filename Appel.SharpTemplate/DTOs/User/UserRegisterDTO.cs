using Appel.SharpTemplate.Models;
using Appel.SharpTemplate.Utils;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Appel.SharpTemplate.DTOs
{
    public class UserRegisterDTO
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordConfirmation { get; set; }

        [JsonConverter(typeof(NumbersOnly))]
        public string IdentityDocument { get; set; }

        [JsonConverter(typeof(NumbersOnly))]
        public string CpfCnpj { get; set; }

        [JsonConverter(typeof(NumbersOnly))]
        public string ResponsibleCpf { get; set; }

        public string ResponsibleName { get; set; }

        [JsonConverter(typeof(NumbersOnly))]
        public string StateRegistration { get; set; }

        [JsonConverter(typeof(NumbersOnly))]
        public string Phone { get; set; }

        [JsonConverter(typeof(NumbersOnly))]
        public string CellPhone { get; set; }

        public string Address { get; set; }

        public string AddressNumber { get; set; }

        public string AddressComplement { get; set; }

        public string Neighborhood { get; set; }

        public string City { get; set; }

        [JsonConverter(typeof(NumbersOnly))]
        public string ZipCode { get; set; }

        public string FederativeUnit { get; set; }

        public UserType Type { get; set; }
    }

    public class NumbersOnly : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString().ToNumbersOnly();
            }

            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
