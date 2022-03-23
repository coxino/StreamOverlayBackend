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

        public Account(Guid id, string username, string password, bool isbanned, DateTime created, DateTime subscription, int role, string email)
        {
            Id = id;
            this.Username = username;
            this.Password = password;
            this.IsBanned = isbanned;
            this.Created = created;
            this.Subscription = subscription;
            this.Role = role;
            this.Email = email;
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

        public override bool Equals(object obj)
        {
            return Equals(obj as Account);
        }

        public bool Equals(Account other)
        {
            return other != null &&
                   Id.Equals(other.Id) &&
                   Username == other.Username &&
                   Password == other.Password &&
                   IsBanned == other.IsBanned &&
                   Created == other.Created &&
                   Subscription == other.Subscription &&
                   Role == other.Role &&
                   Email == other.Email;
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
            return hash.ToHashCode();
        }
    }
}