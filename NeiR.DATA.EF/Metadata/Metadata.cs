using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeiR.DATA.EF.Models/*Metadata*/
{
    //internal class Metadata
    //{
    //}
    #region Category
    public class CategoryMetadata
    {
        //public int CategoryId { get; set; }

        [Required(ErrorMessage = "Weapon Type is required")]
        [Display(Name = "Weapon Type")]
        [StringLength(50, ErrorMessage = "Weapon Type cannot exceed 50 characters")]
        public string WeaponType { get; set; } = null!;
    }
    #endregion

    #region Damage
    public class DamageMetadata
    {
        //public int DamageId { get; set; }

        [Required(ErrorMessage = "Min Damage is required")]
        [Display(Name = "Min Type")]
        public int MinDamg { get; set; }

        [Required(ErrorMessage = "Max Damage is required")]
        [Display(Name = "Max Damage")]
        public int MaxDamg { get; set; }
    }
    #endregion

    #region Stock
    public class StockMetadata
    {
        //public int StockId { get; set; }

        [Required(ErrorMessage = "Stock Status is required")]
        [Display(Name = "Stock Status")]
        [StringLength(50, ErrorMessage = "Stock Status cannot exceed 50 characters")]
        public string StockStatus { get; set; } = null!;
    }
    #endregion

    #region Ability
    public class AbilityMetadata
    {
        //public int AbilityId { get; set; }

        [Required(ErrorMessage = "Ability is required")]
        [Display(Name = "Ability")]
        [StringLength(50, ErrorMessage = "Ability cannot exceed 50 characters")]
        public string AbilityGiven { get; set; } = null!;
    }
    #endregion

    #region Weapon
    public class WeaponMetadata
    {
        //public int WeaponId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        [StringLength(500, ErrorMessage = "Description cannot exceed 50 characters")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Price is required")]
        [Display(Name = "Price")]
        [Range(0, (double)decimal.MaxValue)]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:c}")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        public int AbilityId { get; set; }
        public int DamageId { get; set; }
        public int StockId { get; set; }
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "In Stock is required")]
        [Display(Name = "In Stock")]
        public int StockAmount { get; set; }
        public string WeaponImage { get; set; } = null!;
    }
    #endregion

    #region Order
    public class OrderMetadata
    {
        //nothing needed - this is a PK
        public int OrderId { get; set; }

        //no metadata needed for FKs - as they are represented in a View by a dropdown list
        public string UserId { get; set; } = null!;
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]//0:d => MM/dd/yyyy
        [Display(Name = "Order Date")]
        [Required]
        public DateTime OrderDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Ship To")]
        [Required]
        public string ShipToName { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "City")]
        [Required]
        public string ShipCity { get; set; } = null!;

        [StringLength(2)]
        [Display(Name = "State")]
        public string? ShipState { get; set; }

        [StringLength(5)]
        [Display(Name = "Zip")]
        [Required]
        [DataType(DataType.PostalCode)]
        public string ShipZip { get; set; } = null!;
    }
    #endregion
    #region UserDetail
    public class UserDetailMetadata
    {
        public string UserId { get; set; } = null!;
        [StringLength(50)]
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; } = null!;
        [StringLength(50)]
        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; } = null!;
        [StringLength(150)]
        public string? Address { get; set; }
        [StringLength(50)]
        public string? City { get; set; }
        [StringLength(2)]
        public string? State { get; set; }
        [StringLength(5)]
        [DataType(DataType.PostalCode)]
        public string? Zip { get; set; }
        [StringLength(24)]
        [DataType(DataType.PhoneNumber)]
        public string? Phone { get; set; }
    }
    #endregion
}
