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
        private int inventory;
        private string email;
        private string ipadress;
        private DateTime creationTime;
        private string superbetName;
        private DateTime lastActive;

        public Viewer()
        {
        }

        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public int Inventory { get => inventory; set => inventory = value; }
        public string Email { get => email; set => email = value; }
        public string Ipadress { get => ipadress; set => ipadress = value; }
        public DateTime CreationTime { get => creationTime; set => creationTime = value; }
        public string SuperbetName { get => superbetName; set => superbetName = value; }
        public DateTime LastActive { get => lastActive; set => lastActive = value; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Viewer);
        }

        public bool Equals(Viewer other)
        {
            return other != null &&
                   id == other.id &&
                   name == other.name &&
                   inventory == other.inventory &&
                   email == other.email &&
                   ipadress == other.ipadress &&
                   creationTime == other.creationTime &&
                   superbetName == other.superbetName &&
                   lastActive == other.lastActive &&
                   Id == other.Id &&
                   Name == other.Name &&
                   Inventory == other.Inventory &&
                   Email == other.Email &&
                   Ipadress == other.Ipadress &&
                   CreationTime == other.CreationTime &&
                   SuperbetName == other.SuperbetName &&
                   LastActive == other.LastActive;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(id);
            hash.Add(name);
            hash.Add(inventory);
            hash.Add(email);
            hash.Add(ipadress);
            hash.Add(creationTime);
            hash.Add(superbetName);
            hash.Add(lastActive);
            hash.Add(Id);
            hash.Add(Name);
            hash.Add(Inventory);
            hash.Add(Email);
            hash.Add(Ipadress);
            hash.Add(CreationTime);
            hash.Add(SuperbetName);
            hash.Add(LastActive);
            return hash.ToHashCode();
        }
    }
}
