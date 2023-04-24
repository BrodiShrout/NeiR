using System;
using System.Collections.Generic;

namespace NeiR.DATA.EF.Models
{
    public partial class Damage
    {
        public Damage()
        {
            Weapons = new HashSet<Weapon>();
        }

        public int DamageId { get; set; }
        public int MinDamg { get; set; }
        public int MaxDamg { get; set; }

        public virtual ICollection<Weapon> Weapons { get; set; }
    }
}
