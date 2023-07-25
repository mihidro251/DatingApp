using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // it becomes GET .../api/users
    public class BaseAPIController: ControllerBase
    {
    }
}
