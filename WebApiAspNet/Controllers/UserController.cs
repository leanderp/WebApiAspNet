using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApiAspNet.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Comprueba el token
        /// </summary>
        /// <returns>ActionResult</returns>
        // POST: api/infoUser
        [Authorize]
        [HttpGet("info")]
        public async Task<ActionResult> UserTokenInfo()
        {
            var user = await Task.FromResult(HttpContext.User);

            string id = HttpContext.User.FindFirstValue("id");

            return Ok(new
            {
                Claims = user.Claims.Select(s => new
                {
                    s.Type,
                    s.Value
                }).ToList(),
                id,
                user.Identity.Name,
                user.Identity.IsAuthenticated,
                user.Identity.AuthenticationType
            });
        }

    }
}
