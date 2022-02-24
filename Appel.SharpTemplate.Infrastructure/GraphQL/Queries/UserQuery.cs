using Appel.SharpTemplate.Domain.Entities;
using Appel.SharpTemplate.Domain.Interfaces;
using HotChocolate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.Infrastructure.GraphQL.Queries;

public class UserQuery
{
    public async Task<User> GetUserById(int id, [Service] IUserRepository repository) => await repository.GetByIdAsync(id);

    public async Task<IEnumerable<User>> GetUsers([Service] IUserRepository repository) => await repository.GetAsync();
}
