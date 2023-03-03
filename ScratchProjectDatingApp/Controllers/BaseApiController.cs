using Microsoft.AspNetCore.Mvc;
using ScratchProjectDatingApp.Helper;

namespace ScratchProjectDatingApp.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController: ControllerBase
    {
    }
}
