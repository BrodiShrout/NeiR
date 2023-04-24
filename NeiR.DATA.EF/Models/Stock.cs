using System;
using System.Collections.Generic;

namespace NeiR.DATA.EF.Models
{
    public partial class Stock
    {
        public Stock()
        {
            Weapons = new HashSet<Weapon>();
        }

        public int StockId { get; set; }
        public string StockStatus { get; set; } = null!;

        public virtual ICollection<Weapon> Weapons { get; set; }
    }
}
