using DatabaseContext;
using JWTManager;
using LocalDatabaseManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaticDatabase;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StreamApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChatbotController(ApplicationDbContext context)
        {
            _context = context;
        }



        [HttpPost("transfer")]
        public async Task<ActionResult<string>> TransferOptionAsync([FromHeader] string token, [FromHeader] string userID, [FromBody] BodyRequest bodyRequest)
        {
            if (bodyRequest?.Ammount <= 0)
            {
                return Ok(new { success = false });
            }

            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                if (await db.IsUserValidAsync(userID, bodyRequest.Username))
                {
                    var user = await db.GetViewerAsync(userID);
                    if (user != null)
                    {
                        return Ok(new { success = await db.AddPointsToOneUser(user, bodyRequest.Ammount, true), message="" });
                    }
                }
                else
                {
                    return Ok(new { success = true, message = $" you have to setup an account on {db.GetStreamerSettings().ChannelName}`s store page." });
                }
            }

            //LOG THIS is SOME SORT OF FLOOD
            return Ok(new { success = false, message = "you have to setup an account on the shop!" });
        }

        [HttpPost("updatebet")]
        public async Task<ActionResult<string>> UpdateOptionAsync([FromHeader] string token, [FromHeader] string userID, [FromBody] BodyRequest bodyRequest)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                if(await db.IsUserValidAsync(userID, bodyRequest.Username))
                {
                    return Ok(new { success = true, message = await db.IncrementBettingOptionAsync(userID, bodyRequest.Option, bodyRequest.Ammount) });
                }
                else
                {
                    Ok(new { success = true, message = $" you have to setup an account on {db.GetStreamerSettings().ChannelName}`s store page." });
                }
            }

            //LOG THIS is SOME SORT OF FLOOD
            return Ok(new { success = false, message = "you have to setup an account on the !shop !" });
        }

        [HttpPost("final")]
        public async Task<ActionResult<string>> BalantaFinalaAsync([FromHeader] string token, [FromHeader] string userID, [FromBody] BodyRequest bodyRequest)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {
                if (await db.IsUserValidAsync(userID, bodyRequest.Username))
                {
                    if (FinalBalance.GetUserFinalBallanceVotes(db.GetAccountGuID()).AdaugaBalanta(userID, bodyRequest.Ammount))
                    {
                        return Ok(new { success = true, message = $" you voted that the final balance will be {bodyRequest.Ammount}!" });
                    }
                    else
                    {
                        return Ok(new { success = true, message = $" you cannot vote the final balance twice!" });
                    }
                }
                else
                {
                    return Ok(new { success = true, message = $" you have to setup an account on {db.GetStreamerSettings().ChannelName}`s store page." });
                }
            }

            //LOG THIS is SOME SORT OF FLOOD
            return Ok(new { success = false, message = " you have to setup an account on the shop!" });
        }

        [HttpPost("extragebf")]
        public async Task<ActionResult<string>> ExtrageBalantaFinalaAsync([FromHeader] string token, [FromBody] BodyRequest bodyRequest)
        {
            var db = await UserDatabase.GetDatabaseAsync(token, _context);
            if (db.ValidationResponse.ValidationResponse == ValidationResponse.Success)
            {

                var winner = FinalBalance.GetUserFinalBallanceVotes(db.GetAccountGuID()).CastigatorBalantaFinala(bodyRequest.Ammount);
                var winnerName = await db.GetViewerAsync(winner);

                //await new YoutubeChatWriter().WriteMessageAsync($"Congratulations - @{winnerName.Name} you were the closest to the final balance with your prediction {GiveawayAniversar.BalantaFinala[winnerName.Id]}!");
                //TODO: SAVE WINNER
                return Ok(new { success = true, message = $"Congratulations - {winnerName.Name}  you were the closest to the final balance with your prediction {FinalBalance.GetUserFinalBallanceVotes(db.GetAccountGuID()).BalantaFinala[winnerName.Id]}!" });
            }

            return Ok(new { success = true, message = $"   I could not draw a winner!" });
        }

        public class BodyRequest : IEquatable<BodyRequest>
        {
            public BodyRequest()
            {
            }

            public int Ammount { get; set; }
            public string Option { get; set; }
            public string Username { get; set; }

            public override bool Equals(object obj)
            {
                return Equals(obj as BodyRequest);
            }

            public bool Equals(BodyRequest other)
            {
                return other is not null &&
                       Ammount == other.Ammount &&
                       Option == other.Option &&
                       Username == other.Username;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Ammount, Option, Username);
            }
        }
    }
}
