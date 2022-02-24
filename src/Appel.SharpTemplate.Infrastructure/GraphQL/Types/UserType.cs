using Appel.SharpTemplate.Domain.Entities;
using HotChocolate.Types;

namespace Appel.SharpTemplate.Infrastructure.GraphQL.Types;

public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor.Field(f => f.Id);
        descriptor.Field(f => f.Name);
        descriptor.Field(f => f.Email);
        descriptor.Field(f => f.IdentityDocument);
        descriptor.Field(f => f.CpfCnpj);
        descriptor.Field(f => f.ResponsibleCpf);
        descriptor.Field(f => f.ResponsibleName);
        descriptor.Field(f => f.StateRegistration);
        descriptor.Field(f => f.CellPhone);
        descriptor.Field(f => f.Address);
        descriptor.Field(f => f.AddressNumber);
        descriptor.Field(f => f.AddressComplement);
        descriptor.Field(f => f.Neighborhood);
        descriptor.Field(f => f.ZipCode);
        descriptor.Field(f => f.FederativeUnit);
        descriptor.Field(f => f.Type);
        descriptor.Field(f => f.Role);

        descriptor.Ignore(f => f.Password);
    }
}
