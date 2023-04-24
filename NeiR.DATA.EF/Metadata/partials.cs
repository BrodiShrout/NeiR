using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeiR.DATA.EF.Models/*.Metadata*/
{
    //internal class partials
    //{
    //}

    #region Category
    [ModelMetadataType(typeof(CategoryMetadata))]
    public partial class Category { }
    #endregion

    #region Damage
    [ModelMetadataType(typeof(DamageMetadata))]
    public partial class Damage 
    {
        public string DamageRange
        {
            get { return $"{MinDamg} - {MaxDamg}"; }
        }
    }
    #endregion

    #region Stock
    [ModelMetadataType(typeof(StockMetadata))]
    public partial class Stock { }
    #endregion

    #region Weapon
    [ModelMetadataType(typeof(WeaponMetadata))]
    public partial class Weapon 
    {
        [NotMapped]
        public IFormFile? UploadedImage { get; set; }
    }
    #endregion

    #region UserDetails
    [ModelMetadataType(typeof(UserDetailMetadata))]
    public partial class UserDetail
    {
        public string FullName { get { return $"{FirstName} {LastName}"; } }
    }
    #endregion

    #region Order
    [ModelMetadataType(typeof(OrderMetadata))]
    public partial class Order { }
    #endregion
}
