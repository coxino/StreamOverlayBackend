using DatabaseContext;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamApi.Controllers
{
    public class BroadcastMessage : IEquatable<BroadcastMessage>
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as BroadcastMessage);
        }

        public bool Equals(BroadcastMessage other)
        {
            return other != null &&
                   UserId == other.UserId &&
                   UserName == other.UserName &&
                   Message == other.Message;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserId, UserName, Message);
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class BroadcastController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BroadcastController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("broadcast")]
        public async Task<ActionResult<string>> SetAsync([FromHeader] string token, [FromBody] BroadcastMessage message)
        {            
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                var viewer = await db.GetViewerAsync(message.UserId);
                if ((int)viewer.MemberLevel >= (int)MemberLevels.Gangster)
                {
                    if (viewer.BroadcastMessageCount > 0)
                    {
                        await db.RemoveBroadcastMessageAsync(viewer);
                        db.BroadcastMessage(message.UserName, message.Message.Remove(0,message.Message.IndexOf(" ")));
                    }
                }
                else
                {
                    return "Doar membrii de la gangster in sus pot trimite broadcast messages.";
                }
            }

            return Ok(true);
        }
    }
}
