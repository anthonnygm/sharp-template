using Appel.SharpTemplate.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Appel.SharpTemplate.Infrastructure
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params UserRole[] roles)
        {
            Roles = string.Join(",", roles.Select(r => r.ToString()));
        }
    }
}
