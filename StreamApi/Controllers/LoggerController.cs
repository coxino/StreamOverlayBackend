using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace StreamApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggerController : ControllerBase
    {
        [HttpPost("feed")]
        public async Task<ActionResult<bool>> SetAsync([FromHeader] string token, [FromBody] object data)
        {            
            await System.IO.File.AppendAllTextAsync("C:/Logger/log.txt", "\r\n" + DateTime.Now.ToString("HH:mm:ss:ff") + ":" + data);
            return Ok(true);
        }
    }
}
