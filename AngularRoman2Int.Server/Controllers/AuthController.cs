using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AngularRoman2Int.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Check is the user is authorized to used functions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public bool Get()
        {
            return false;   // Either Authorized or Not
            //TODO transfer this concept to a database in SQLIte using Service Stack ORMLite
        }
    }
}
