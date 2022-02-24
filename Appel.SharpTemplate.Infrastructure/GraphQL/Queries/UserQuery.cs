using Appel.SharpTemplate.Domain.Entities;
using Appel.SharpTemplate.Domain.Interfaces;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.Infrastructure.GraphQL.Queries;

public class UserQuery
{
    [Authorize]
    public async Task<User> GetUserById(int id, [Service] IUserRepository repository) => await repository.GetByIdAsync(id);

    [Authorize]
    public async Task<IEnumerable<User>> GetUsers([Service] IUserRepository repository) => await repository.GetAsync();
}
