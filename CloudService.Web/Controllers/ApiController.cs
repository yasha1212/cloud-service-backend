using Microsoft.AspNetCore.Mvc;

namespace CloudService.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiController : ControllerBase
    {
    }
}
