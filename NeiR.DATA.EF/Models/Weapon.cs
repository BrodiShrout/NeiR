using System;
using System.Collections.Generic;

namespace NeiR.DATA.EF.Models
{
    public partial class Weapon
    {
        public Weapon()
        {
            OrderProducts = new HashSet<OrderProduct>();
        }

        public int WeaponId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int AbilityId { get; set; }
        public int DamageId { get; set; }
        public int StockAmount { get; set; }
        public int CategoryId { get; set; }
        public string WeaponImage { get; set; } = null!;
        public int StockId { get; set; }

        public virtual Ability? Ability { get; set; }
        public virtual Category? Category { get; set; }
        public virtual Damage? Damage { get; set; }
        public virtual Stock? Stock { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
