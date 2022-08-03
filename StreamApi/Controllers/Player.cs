using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Player : ControllerBase
    {
        [HttpGet("login/{key}")]
        public string Login(string key)
        {
            return key;
        }
    }
}
