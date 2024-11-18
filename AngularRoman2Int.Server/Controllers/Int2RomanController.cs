using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Int2Roman;
using AngularRoman2Int.Server.Model;
namespace AngularRoman2Int.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Int2RomanController : ControllerBase
    {
        IInt2Roman _service;
        public Int2RomanController(IInt2Roman service)
        {
            _service = service;
        }
        [HttpGet(Name ="GetRoman")]
        public IntegerRomanDTO Get(uint number)
        {
            //IInt2Roman  service = new Int2Roman.Int2Roman();
            //var answer = _service.ConvertToRoman(number);
            var answer = new IntegerRomanDTO() { Answer = _service.ConvertToRoman(number) };
            return answer;
        }
    }
}
