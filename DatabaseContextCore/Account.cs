using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DatabaseContext
{
    public class Account : IEquatable<Account>
    {
        public Account()
        {
        }


        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime Created { get; set; }
        public DateTime Subscription { get; set; }
        public string Email { get; set; }
        public string YoutubeToken { get; set; }
        public string TwitchToken { get; set; }
        public string TwitchId { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Account);
        }

        public bool Equals(Account other)
        {
            return other is not null &&
                   Id.Equals(other.Id) &&
                   Username == other.Username &&
                   Password == other.Password &&
                   Created == other.Created &&
                   Subscription == other.Subscription &&
                   Email == other.Email &&
                   YoutubeToken == other.YoutubeToken &&
                   TwitchToken == other.TwitchToken &&
                   TwitchId == other.TwitchId;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Id);
            hash.Add(Username);
            hash.Add(Password);
            hash.Add(Created);
            hash.Add(Subscription);
            hash.Add(Email);
            hash.Add(YoutubeToken);
            hash.Add(TwitchToken);
            hash.Add(TwitchId);
            return hash.ToHashCode();
        }
    }
}