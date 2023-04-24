using NeiR.DATA.EF.Models; //Grants access to Product

namespace NeiR.UI.MVC.Models
{
    public class CartItemViewModel
    {
        public int Qty { get; set; }

        public Weapon Weapons { get; set; }
        //The above is an example of a concept called "Containment":
        //This is the use of a complex datatype as a field or property in another complex type

        //Complex datatypes: Any class with multiple properties (Product, ContactViewModel, DateTime, etc.)
        //Primitive datatypes: Any class that stores ONLY a single value (int, bool, char, decimal, etc.)
       

        //Constructor (ctor)    
        public CartItemViewModel(int qty, Weapon weapons)
        {
            //Assignment
            Qty = qty;
            Weapons = weapons;
        }
    }
}
