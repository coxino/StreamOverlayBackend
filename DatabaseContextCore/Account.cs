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
        public bool IsBanned { get; set; }
        public DateTime Created { get; set; }
        public DateTime Subscription { get; set; }
        public int Role { get; set; }
        public string Email { get; set; }
        public string YoutubeToken { get; set; }

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
                   IsBanned == other.IsBanned &&
                   Created == other.Created &&
                   Subscription == other.Subscription &&
                   Role == other.Role &&
                   Email == other.Email &&
                   YoutubeToken == other.YoutubeToken;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Id);
            hash.Add(Username);
            hash.Add(Password);
            hash.Add(IsBanned);
            hash.Add(Created);
            hash.Add(Subscription);
            hash.Add(Role);
            hash.Add(Email);
            hash.Add(YoutubeToken);
            return hash.ToHashCode();
        }
    }
}