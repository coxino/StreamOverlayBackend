using DatabaseContext;
using DataLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JWTManager
{
	public enum ValidationResponse
	{
		Success,
		Error,
		UserNotFound,
		SubscriptionExpired
	}

	public class ValidationModel
	{
		public ValidationResponse ValidationResponse { get; set; }
		public string UserName { get; set; }
		public Guid UserGuid { get; set; }
	}

	public static class ClaimNames
	{
		public static string YoutubeToken = "YoutubeToken";
        public static string Username = "Username";
		public static string Password = "Password";
		public static string StreamerId = "UserId";

	}
	public class JwtManager
	{
		private static string mySecret = "bQeThWmZq4t7w!z%C*F-J@NcRfUjXn2r5u8x/A?D(G+KbPdSgVkYp3s6v9y$B&E)H@McQfThWmZq4t7w!z%C*F-JaNdRgUkXn2r5u8x/A?D(G+KbPeShVmYq3s6v9y$B&E)H@McQfTjWnZr4u7w!z%C*F-JaNdRgUkXp2s5v8y/A?D(G+KbPeShVmYq3t6w9z$C&E)H@McQfTjWnZr4u7x!A%D*G-JaNdRgUkXp2s5v8y/B?E(H+MbPeShVmYq3t";

        public static bool VerifyViewerToken(LocalUser userData)
        {
			return GetClaim(userData.authToken, ClaimNames.Username) == userData.userYoutubeID;
        }

        public static string GenerateToken(Account user)
		{
			var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

			var myIssuer = "http://mysite.com";
			var myAudience = "http://myaudience.com";

			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimNames.Username, user.Username),
					new Claim(ClaimNames.Password, user.Password),
					new Claim(ClaimNames.StreamerId, user.Id.ToString())
				}),
				Expires = DateTime.UtcNow.AddDays(7),
				Issuer = myIssuer,
				Audience = myAudience,
				SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			var tt = tokenHandler.WriteToken(token);

			return Newtonsoft.Json.JsonConvert.SerializeObject(new { token = tt });
		}


		public static bool ValidateCurrentToken(string token)
		{
			var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

			var myIssuer = "http://streamagapi.azure";
			var myAudience = "http://streamagapi.azure";

			var tokenHandler = new JwtSecurityTokenHandler();
			try
			{
				tokenHandler.ValidateToken(token, new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidIssuer = myIssuer,
					ValidAudience = myAudience,
					IssuerSigningKey = mySecurityKey
				}, out SecurityToken validatedToken);
			}
			catch (Exception e)
			{
				var msg = e.Message;
				return false;
			}
			return true;
		}

		public static string GetClaim(string token, string claimType)
		{
            if (string.IsNullOrWhiteSpace(token))
            {
				return "error";
            }

			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

				var stringClaimValue = securityToken.Claims.First(claim => claim.Type == claimType).Value;
				return stringClaimValue;
			}
			catch
			{
				return "";
			}
		}


		public static async Task<ValidationModel> ValidateTokenAsync(string token, ApplicationDbContext _context)
		{
			if (string.IsNullOrEmpty(token))
				return new ValidationModel() { ValidationResponse = ValidationResponse.Error };

			if (ValidateCurrentToken(token) == true)
			{
				var usr = GetClaim(token, ClaimNames.Username);
				var pwd = GetClaim(token, ClaimNames.Username);
				var id = GetClaim(token, ClaimNames.StreamerId);

				var user = await _context.Accounts.Where(x => x.Username == usr && x.Password == pwd).FirstOrDefaultAsync();
				if (user == null)
				{
					return new ValidationModel() { ValidationResponse = ValidationResponse.UserNotFound };
				}

				if (user.Subscription < DateTime.Now)
				{
					return new ValidationModel() { UserName = user.Username, ValidationResponse = ValidationResponse.SubscriptionExpired };
				}
			}
			var userName = GetClaim(token, ClaimNames.Username).Replace(" ", "");
			var userId = GetClaim(token, ClaimNames.StreamerId).Replace(" ", "");

			var userAccount = await _context.Accounts.Where(x => x.Username == userName).FirstOrDefaultAsync();

			if (userAccount.Subscription < DateTime.Now)
			{
				return new ValidationModel() { UserName = userAccount.Username, ValidationResponse = ValidationResponse.SubscriptionExpired, UserGuid = Guid.Parse(userId) };
			}

			return new ValidationModel() { UserName = userName, ValidationResponse = ValidationResponse.Success, UserGuid = Guid.Parse(userId) };
		}

		public static string ValidateAccountAsync(string valcode, out bool validated)
		{
			if (string.IsNullOrEmpty(valcode))
			{
				validated = false;
				return "";
			}
			var usr = GetClaim(valcode, ClaimNames.Username);
			if (string.IsNullOrEmpty(usr))
			{
				validated = false;
				return "";
			}
			validated = true;
			return usr;
		}

		public static string GenerateViewerToken(string userId,string youtubeToken)
		{
			var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

			var myIssuer = "http://mysite.com";
			var myAudience = "http://myaudience.com";

			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimNames.Username, userId),
					new Claim(ClaimNames.YoutubeToken, youtubeToken),
                }),
				Expires = DateTime.UtcNow.AddDays(30),
				Issuer = myIssuer,
				Audience = myAudience,
				SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			var tt = tokenHandler.WriteToken(token);

			return tt;
		}

        public static string GenerateViewerLoginToken(string userYoutubeID, string youtubeToken)
        {
			return GenerateViewerToken(userYoutubeID, youtubeToken);
        }
    }
}
