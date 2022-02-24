using Appel.SharpTemplate.Infrastructure.GraphQL.Queries;
using HotChocolate.Types;

namespace Appel.SharpTemplate.Infrastructure.GraphQL.Types;

public class UserType : ObjectType<UserQuery>
{
    protected override void Configure(IObjectTypeDescriptor<UserQuery> descriptor)
    {
        descriptor
            .Field(f => f.GetUsers(default));

        descriptor
            .Field(f => f.GetUserById(default, default));
    }
}
