using Appel.SharpTemplate.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Appel.SharpTemplate.Infrastructure.Application
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params UserRole[] roles)
        {
            Roles = string.Join(",", roles.Select(r => r.ToString()));
        }
    }
}
