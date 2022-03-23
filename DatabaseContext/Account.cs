using System;
using System.ComponentModel.DataAnnotations;

namespace StreamApi.DatabaseContext
{
	public class Account : IEquatable<Account>
    {
        public Account()
        {
        }

        public Account(Guid id, string username, string password, bool isbanned, DateTime created, DateTime subscription, int role)
        {
            Id = id;
            this.username = username;
            this.password = password;
            this.isbanned = isbanned;
            this.created = created;
            this.subscription = subscription;
            this.role = role;
        }

        [Key]
        public Guid Id { get; set; }
		public string username { get; set; }
		public string password { get; set; }
		public bool isbanned { get; set; }
		public DateTime created { get; set; }
		public DateTime subscription { get; set; }
		public int role { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Account);
        }

        public bool Equals(Account other)
        {
            return other != null &&
                   Id.Equals(other.Id) &&
                   username == other.username &&
                   password == other.password &&
                   isbanned == other.isbanned &&
                   created == other.created &&
                   subscription == other.subscription &&
                   role == other.role;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, username, password, isbanned, created, subscription, role);
        }
    }
}