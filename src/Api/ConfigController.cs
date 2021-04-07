using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ConfigController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return Ok(_configuration.AsEnumerable().Select((k, v) => $"{v} : {k.Key} - {k.Value}"));
        }
    }
}
