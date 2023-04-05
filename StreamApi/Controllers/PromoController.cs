using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamApi.Controllers
{
    public class Promo
    {
        public string descriere { get; set; }
        public string image { get; set; }
        public string link { get; set; }
        public string name { get; set; }
        public int rating { get; set; }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class PromoController : ControllerBase
    {
        [HttpGet("getallpromo")]
        public async Task<ActionResult<Promo[]>> GetAllPromoAsync()
        {

            var promo = await System.IO.File.ReadAllTextAsync(@"C:\API\database\promotii.json");
            var allPromo = Newtonsoft.Json.JsonConvert.DeserializeObject<Promo[]>(promo);
            return allPromo;
        }

        [HttpGet("getpromo")]
        public async Task<ActionResult<string>> GetAsync()
        {
            
            var promo = await System.IO.File.ReadAllTextAsync(@"C:\oferta\oferta1.txt");
            return promo;
        }

        [HttpGet("getmpromo")]
        public async Task<ActionResult<string>> Getmpromo()
        {

            var promo = await System.IO.File.ReadAllTextAsync(@"C:\oferta\oferta1m.txt");
            return promo;
        }

    }
}
