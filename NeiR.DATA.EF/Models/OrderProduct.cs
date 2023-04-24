using System;
using System.Collections.Generic;

namespace NeiR.DATA.EF.Models
{
    public partial class OrderProduct
    {
        public int OrderProductsId { get; set; }
        public int WeaponId { get; set; }
        public int OrderId { get; set; }
        public short? Quantity { get; set; }
        public decimal? Price { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Weapon Weapon { get; set; } = null!;
    }
}
