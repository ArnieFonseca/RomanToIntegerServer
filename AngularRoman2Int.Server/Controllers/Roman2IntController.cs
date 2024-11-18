using Microsoft.AspNetCore.Mvc;

using Roman2Int.Lib;
using AngularRoman2Int.Server.Model;

namespace AngularRoman2Int.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class Roman2IntController : Controller
{
    private readonly IRoman2Int romanToInteger;

    public Roman2IntController(IRoman2Int romanToInteger)
    {
        this.romanToInteger = romanToInteger;
    }

    [HttpGet(Name = "GetConvertion")]
    public RomanDTO Get(string romanNumber)
    {

        // Call the Convert Method to get the Tuple Result
        var (Success, Token, Answer) = romanToInteger.Convert(romanNumber.ToUpper());

        // Load the Data Transfer Object with the return value
        var dto = new RomanDTO()
        {
            Success = Success,
            Token = Token,
            Answer = Answer
        };

        // Send the Data Transfer Object back to the client
        return dto;
    }
}
