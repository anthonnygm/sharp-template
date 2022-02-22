using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Appel.SharpTemplate.Controllers.API
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : Controller { }
}
