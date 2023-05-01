using Microsoft.AspNetCore.Mvc;
using NeiR.DATA.EF.Models; //Grants access to GadgetStoreContext and Product classes
using Microsoft.AspNetCore.Identity; //Grants access to Identity classes & methods
using NeiR.UI.MVC.Models; //Grants access to CartItemViewModel class
using Newtonsoft.Json; //Grants access to JSON classes & methonds (Serialization & Deserialization)
using System.Security.Cryptography.X509Certificates;
using System.Drawing;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.AspNetCore.Authorization;
namespace NeiR.UI.MVC.Controllers

{
    public class ShoppingCartController : Controller
    {

        #region Steps to Implement Session Based Shopping Cart
        /*
         * 1) Register Session in program.cs (builder.Services.AddSession... && app.UseSession())
         * 2) Create the CartItemViewModel class in [ProjName].UI.MVC/Models folder
         * 3) Add the 'Add To Cart' button in the Index and/or Details view of your Products
         * 4) Create the ShoppingCartController (empty controller -> named ShoppingCartController)
         *      - add using statements
         *          - using GadgetStore.DATA.EF.Models;
         *          - using Microsoft.AspNetCore.Identity;
         *          - using GadgetStore.UI.MVC.Models;
         *          - using Newtonsoft.Json;
         *      - Add props for the GadgetStoreContext && UserManager
         *      - Create a constructor for the controller - assign values to context && usermanager
         *      - Code the AddToCart() action
         *      - Code the Index() action
         *      - Code the Index View
         *          - Start with the basic table structure
         *          - Show the items that are easily accessible (like the properties from the model)
         *          - Calculate/show the lineTotal
         *          - Add the RemoveFromCart <a>
         *      - Code the RemoveFromCart() action
         *          - verify the button for RemoveFromCart in the Index view is coded with the controller/action/id
         *      - Add UpdateCart <form> to the Index View
         *      - Code the UpdateCart() action
         *      - Add Submit Order button to Index View
         *      - Code SubmitOrder() action
         * */
        #endregion

        //Fields/Props
        private readonly NeiRContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ShoppingCartController(NeiRContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {

            //Retrieve the contents from the Session shopping cart (stored as JSON) and convert them to C# via Newtonsoft.Json. After converting to C#, we can pass the collection of cart contents back to the strongly-typed view to display.

            //Retrive the cart contents
            var sessionCart = HttpContext.Session.GetString("cart");

            //Create a shell for the local version (C# Cart)
            Dictionary<int, CartItemViewModel> shoppingCart = null;

            //Check to see if teh session cart was null
            if (sessionCart == null || sessionCart.Count() == 0)
            {
                //User either hasn't put anything in the cart or they have removed all items.
                //So, set the shoppingCart to an empty object
                shoppingCart = new Dictionary<int, CartItemViewModel>();

                ViewBag.Message = "There are no items in your cart.";
            }
            else
            {
                ViewBag.Message = null;

                //Deserialize the cart contents from JSON to C#
                shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
            }

            return View(shoppingCart);
        }

        public IActionResult AddToCart(int id)
        {
            //Create an empty shell for the LOCAL shopping cart variable (NOT the cart in the session)
            //int (key) => ProductId
            //CartItemViewModel (value) => Product & Qty
            Dictionary<int, CartItemViewModel> shoppingCart = null;

            #region Session Notes

            /*
             Session is a tool available on the server-side that can store information for a user while they are actively using your site.

            Typically, the session lasts for 20 minutes ( this can be adjusted in Program.cs)
            Once the 20 minutes is up, the session variable is disposed.

            Values that we can store in the session are limited to: string, int
             - Because of this limitation, we have to get creative when trying to store complex objects
            (like CarItemViewModel objects).
             - To keep info seperated into properties
             */

            #endregion

            var sessionCart = HttpContext.Session.GetString("cart");

            //Check to see if the Session object exists
            //If not, instantiate a new local collection
            if (String.IsNullOrEmpty(sessionCart))
            {
                //Static vs Instance Methods
                //Static > Called from the CLASS
                //Instance > Called from a specific object (an INSTANCE of that class)
                //string name = "Evan";

                //name.ToUpper();

                //string.IsNullOrEmpty(name);

                //If the session didn't exist yet, instantiate the collection as a new object
                shoppingCart = new Dictionary<int, CartItemViewModel>();

            }
            else
            {
                //Cart already exists -- transfer the existing cart items from the Session into our local shoppinCart

                //DeserializeObject() is a methos that converts JSON to C#. We MUST tell this method
                //which C# class to convert the JSON into (here -- Dictionary<int, CIVM>)
                shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            }

            //Add the newly selected product to the cart
            //Retrive the Product from the Database
            Weapon weapon = _context.Weapons.Find(id);

            //Instantiate the CIVM object so we can add it to the cart
            CartItemViewModel civm = new CartItemViewModel(1, weapon); //Adds 1 of the selected product to the cart

            //If the product was already in the cart, increase quantity by 1
            //Otherwise, just add the new item to the cart
            if (shoppingCart.ContainsKey(weapon.WeaponId))
            {
                //Update the Qty
                shoppingCart[weapon.WeaponId].Qty++;
            }
            else
            {
                //Add the item to the cart
                shoppingCart.Add(weapon.WeaponId, civm);
            }

            //Update the session the version of the cart
            //Take the local version and serialize it as JSON
            string jsonCart = JsonConvert.SerializeObject(shoppingCart);

            //Assign that value to our Session
            HttpContext.Session.SetString("cart", jsonCart);

            return RedirectToAction("Index");

        }

        public IActionResult RemoveFromCart(int id)
        {
            //Retrieve the cart from the session
            var sessionCart = HttpContext.Session.GetString("cart");

            //Convert JSON to C#
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            //Remove cart item
            shoppingCart.Remove(id);

            //If there are no remaining items in the cart, remove it from the session
            if (shoppingCart.Count == 0)
            {
                HttpContext.Session.Remove("cart");
            }
            //Otherwise, update the session variable with the new local cart contents
            else
            {
                string jsonCart = JsonConvert.SerializeObject(shoppingCart);
                HttpContext.Session.SetString("cart", jsonCart);
            }

            return RedirectToAction("Index");

        }

        public IActionResult UpdateCart(int weaponid, int qty)
        {
            //Retrieve the cart from the Session
            var sessionCart = HttpContext.Session.GetString("cart");

            //Deserialize from JSON to C#
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            //Update the qty from our specific dictionary key
            shoppingCart[weaponid].Qty = qty;

            //Update the session
            string jsonCart = JsonConvert.SerializeObject(shoppingCart);
            HttpContext.Session.SetString("cart", jsonCart);

            return RedirectToAction("Index");
        }

        //Submit button
        [Authorize]
        public async Task<IActionResult> SubmitOrder()
        {
            #region Plannning out our order submittion

            //The big picture:
            //Create an order object when the user submits and saves it to the database
            //-OrderDate (supplied programmatically)
            //-UserId (supplied by the active user)
            //-ShipToName, ShipCity, ShipState, ShipZip -> This info will be pulled from the UserDetails table by looking up the record for the current UserId
            //Ass the record to the _contect (queue it up to be added  in the DB)
            //Save the db changes

            //Create an OrderProduct object for each item in the cart
            //-ProductId > pulled from the cart
            //-OrderId > Pulled from order object
            //-Qty > pulled from the cart
            //-ProductPrice > available from the cart
            //Add the record to the _context
            //Save the db changes


            #endregion

            //Retrieve the current users Id
            //The code below is a mechanism provided by Identity to retrieve the UserId in the current HttpContext. If your need to retrieve the UserID in any controller, you  can use this
            string? userId = (await _userManager.GetUserAsync(HttpContext.User))?.Id;

            //Retrieve the UserDetails record
            UserDetail ud = _context.UserDetails.Find(userId);

            //Create the order object
            Order o = new()
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                ShipCity = ud.City,
                ShipToName = ud.FirstName + " " + ud.LastName,
                ShipState = ud.State,
                ShipZip = ud.Zip
            };

            //Add the Order to _context
            _context.Orders.Add(o);

            //retrieve the session shopping cart
            var sessionCart = HttpContext.Session.GetString("cart");

            //Convery to C#
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            //Create an OrderProduct object for each item in the cart
            foreach (var item in shoppingCart)
            {
                OrderProduct op = new OrderProduct()
                {
                    WeaponId = item.Key,
                    OrderId = o.OrderId,
                    Quantity = (short?)item.Value.Qty,
                    Price = item.Value.Weapons.Price,
                };

                //For linking table records, we can add items/records to an existing entity
                //if the records are related >
                o.OrderProducts.Add(op);

                //remove the item from the cart
                shoppingCart.Remove(item.Key);
        }

                //Save changes to db
                _context.SaveChanges();

                //Empty the cart/Remove the cart strip from the session
                HttpContext.Session.Remove("cart");


                //Redirect the user to the orders index
                return RedirectToAction("Index", "Orders");
            }

    }
}
