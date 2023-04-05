using DatabaseContextCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseContext
{
    public class Viewer : IEquatable<Viewer>
    {
        private string id;
        private string name;
        private int broadcastMessageCount;
        private string email;
        private string emailSecundar;
        private string ipadress;
        private DateTime creationTime;
        private string superbetName;
        private DateTime lastActive;
        private MemberLevels memberLevel;
        private bool isActive;
        private DateTime expiresMember;

        public Viewer()
        {

        }

        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Email { get => email; set => email = value; }
        public string Ipadress { get => ipadress; set => ipadress = value; }
        public DateTime CreationTime { get => creationTime; set => creationTime = value; }
        public string SuperbetName { get => superbetName; set => superbetName = value; }
        public DateTime LastActive { get => lastActive; set => lastActive = value; }
        public MemberLevels MemberLevel { get => memberLevel; set => memberLevel = value; }
        public int BroadcastMessageCount { get => broadcastMessageCount; set => broadcastMessageCount = value; }
        public bool IsActive { get => isActive; set => isActive = value; }
        public DateTime ExpiresMember { get => expiresMember; set => expiresMember = value; }
        public string EmailSecundar { get => emailSecundar; set => emailSecundar = value; }

        public ICollection<ViewerWallet> Wallets { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Viewer);
        }

        public bool Equals(Viewer other)
        {
            return other is not null &&
                   id == other.id &&
                   name == other.name &&
                   broadcastMessageCount == other.broadcastMessageCount &&
                   email == other.email &&
                   emailSecundar == other.emailSecundar &&
                   ipadress == other.ipadress &&
                   creationTime == other.creationTime &&
                   superbetName == other.superbetName &&
                   lastActive == other.lastActive &&
                   memberLevel == other.memberLevel &&
                   isActive == other.isActive &&
                   expiresMember == other.expiresMember &&
                   Id == other.Id &&
                   Name == other.Name &&
                   Email == other.Email &&
                   Ipadress == other.Ipadress &&
                   CreationTime == other.CreationTime &&
                   SuperbetName == other.SuperbetName &&
                   LastActive == other.LastActive &&
                   MemberLevel == other.MemberLevel &&
                   BroadcastMessageCount == other.BroadcastMessageCount &&
                   IsActive == other.IsActive &&
                   ExpiresMember == other.ExpiresMember &&
                   EmailSecundar == other.EmailSecundar &&
                   EqualityComparer<ICollection<ViewerWallet>>.Default.Equals(Wallets, other.Wallets);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(id);
            hash.Add(name);
            hash.Add(broadcastMessageCount);
            hash.Add(email);
            hash.Add(emailSecundar);
            hash.Add(ipadress);
            hash.Add(creationTime);
            hash.Add(superbetName);
            hash.Add(lastActive);
            hash.Add(memberLevel);
            hash.Add(isActive);
            hash.Add(expiresMember);
            hash.Add(Id);
            hash.Add(Name);
            hash.Add(Email);
            hash.Add(Ipadress);
            hash.Add(CreationTime);
            hash.Add(SuperbetName);
            hash.Add(LastActive);
            hash.Add(MemberLevel);
            hash.Add(BroadcastMessageCount);
            hash.Add(IsActive);
            hash.Add(ExpiresMember);
            hash.Add(EmailSecundar);
            hash.Add(Wallets);
            return hash.ToHashCode();
        }
    }
}
