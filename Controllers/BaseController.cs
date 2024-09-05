using Microsoft.AspNetCore.Mvc;
using Superpower;
using System.Security.Claims;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController :ControllerBase
    {
        protected int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
    }
}
