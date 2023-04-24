using System;
using System.Collections.Generic;

namespace NeiR.DATA.EF.Models
{
    public partial class Ability
    {
        public Ability()
        {
            Weapons = new HashSet<Weapon>();
        }

        public int AbilityId { get; set; }
        public string AbilityGiven { get; set; } = null!;

        public virtual ICollection<Weapon> Weapons { get; set; }
    }
}
