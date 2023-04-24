using System;
using System.Collections.Generic;

namespace NeiR.DATA.EF.Models
{
    public partial class Category
    {
        public Category()
        {
            Weapons = new HashSet<Weapon>();
        }

        public int CategoryId { get; set; }
        public string WeaponType { get; set; } = null!;

        public virtual ICollection<Weapon> Weapons { get; set; }
    }
}
