using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NeiR.DATA.EF.Models;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using NeiR.UI.MVC.Utilities;

using X.PagedList; //Grants access to PagedList

namespace NeiR.UI.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class WeaponsController : Controller
    {
        private readonly NeiRContext _context;

        //Added prop bellow for access to the wwwroot folder
        private readonly IWebHostEnvironment _webHostEnvironment;

        public WeaponsController(NeiRContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            #region Make Admin Command
            // We want to show ALL products to the admins, but only products that are active and in stock for regular users
            if (User.IsInRole("Admin"))
            {
                var weapon = _context.Weapons

                .Include(p => p.Category).Include(p => p.OrderProducts);
                return View(await weapon.ToListAsync());
            }
            #endregion
            else
            #region Customer Command
            {
                var weapons = _context.Weapons.Where(p => p.StockAmount > 0)

                    .Include(p => p.Category).Include(p => p.OrderProducts);
                return View(await weapons.ToListAsync());
            }
            #endregion
        }

        #region Shows a tiled page for products if not admin
        [AllowAnonymous]
            public async Task<IActionResult> WeaponShop(string searchTerm, int categoryId = 0, int page = 1)
            {
                int pageSize = 6;

                var weapons = _context.Weapons
                .Include(p => p.Category)
                .Include(p => p.Ability)
                .Include(p => p.Stock)
                .Include(p => p.Damage)
                .Include(p => p.OrderProducts).ToList();

                #region Optional Category Filter

                //Create a ViewData object to send a list of categories to the view.
                //This is the same as what we see in Products/Create()

                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "WeaponType");

                //Create a ViewBag Variable to persist the selected category
                ViewBag.Category = 0;

                if (categoryId != 0)
                {
                    weapons = weapons.Where(p => p.CategoryId == categoryId).ToList();

                    //Repopulate the dropdown with current category selected
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "WeaponType", categoryId);

                    ViewBag.Category = categoryId;
                }

                #endregion

                #region Optional Search Filter

                if (!String.IsNullOrEmpty(searchTerm))
                {
                    weapons = weapons.Where(p =>
                        p.Name.ToLower().Contains(searchTerm.ToLower()) ||
                        p.Category.WeaponType.ToLower().Contains(searchTerm.ToLower()) ||
                        p.Ability.AbilityGiven.Contains(searchTerm.ToLower()) ||
                        p.Description.ToLower().Contains(searchTerm.ToLower())).ToList();

                    //Viewbag for the total number of results
                    ViewBag.NbrResults = weapons.Count();

                    //Viewbag for the search term
                    ViewBag.SearchTerm = searchTerm;

                }
                else
                {
                    ViewBag.NbrResults = null;
                    ViewBag.SearchTerm = null;
                }

                #endregion


                //return View(await products.ToListAsync());
                //return View(products);
                return View(weapons.ToPagedList(page, 6));
            }
      
        #endregion

        // GET: Products/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Weapons == null)
            {
                return NotFound();
            }

            var weapon = await _context.Weapons
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.WeaponId == id);
            if (weapon == null)
            {
                return NotFound();
            }

            return View(weapon);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["AbilityId"] = new SelectList(_context.Abilities, "AbilityId", "AbilityGiven");
            ViewData["DamageId"] = new SelectList(_context.Damages, "DamageId", "DamageRange");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "WeaponType");
            ViewData["StockId"] = new SelectList(_context.Stocks, "StockId", "StockStatus");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WeaponId, Name, Description, Price, AbilityId, DamageId, StockAmount, CategoryId, WeaponImage, StockId, UploadedImage")] Weapon weapon)
        {
            if (ModelState.IsValid)
            {

                #region File Upload - Create w/Image Utility

                if (weapon.UploadedImage != null)
                {
                    //process file

                    //check the file type
                    //- retrive the extention of the uploaded file
                    string ext = Path.GetExtension(weapon.UploadedImage.FileName);

                    //- create a list of valid extentions
                    string[] validExts = { ".jpg", ".jpeg", ".gif", ".png" };

                    //- check the file extention against the list of vaild extentions
                    if (validExts.Contains(ext.ToLower()) && weapon.UploadedImage.Length < 4_194_303)
                    {
                        //Generate a unique filename
                        weapon.WeaponImage = Guid.NewGuid() + ext;

                        //save file to the web server (here its saved to wwwroot/images
                        // -retrive the path from wwwroot
                        string webRootPath = _webHostEnvironment.WebRootPath;

                        //-create a variable for thr full image path
                        //string fullImage = webRootPath + "/images/";
                        string fullImage = webRootPath + "/img/";

                        //Create a MemoryStream to read rthe image into our web server's memory
                        using (var memoryStream = new MemoryStream())
                        {
                            await weapon.UploadedImage.CopyToAsync(memoryStream);
                            using (var img = Image.FromStream(memoryStream))
                            {
                                //send the image to the ImageUtility for resizing and saving
                                //need 5 arguments for the utility to resize our image
                                //1) int max image size
                                //2} int max thumbnail size
                                //3} string full path where the file will be saved
                                //4) image an image 
                                //5) string filename 
                                int maxImageSize = 500;
                                int maxThumbSize = 100;
                                ImageUtility.ResizeImage(fullImage, weapon.WeaponImage, img, maxImageSize, maxThumbSize);
                            }
                        }
                    }

                }
                else
                {
                    //assig default image
                    //If no image was uploaded, assing a default filename
                    //will also need to download a default image and name it "noimage.png" -> place in wwwroot
                    weapon.WeaponImage = "noimage.png";
                }
                #endregion

                _context.Add(weapon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(WeaponShop));
                //return RedirectToAction(nameof(Shop));
            }
            ViewData["AbilityId"] = new SelectList(_context.Abilities, "AbilityId", "AbilityGiven");
            ViewData["DamageId"] = new SelectList(_context.Damages, "DamageId", "DamageRange");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "WeaponType");
            ViewData["StockId"] = new SelectList(_context.Stocks, "StockId", "StockStatus");

            return View(weapon);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Weapons == null)
            {
                return NotFound();
            }

            var weapon = await _context.Weapons.FindAsync(id);
            if (weapon == null)
            {
                return NotFound();
            }

            ViewData["AbilityId"] = new SelectList(_context.Abilities, "AbilityId", "AbilityGiven");
            ViewData["DamageId"] = new SelectList(_context.Damages, "DamageId", "DamageRange");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "WeaponType");
            ViewData["StockId"] = new SelectList(_context.Stocks, "StockId", "StockStatus");
            return View(weapon);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WeaponId,Name,Description,Price,AbilityId,DamageId,StockAmount,CategoryId,WeaponImage,StockId, UploadedImage")] Weapon weapon)
        {
            if (id != weapon.WeaponId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                #region Edit Image/ File upload - EDIT w/ Image Utility

                //retian old image file name so we can reuse if needed, or delete if a new file is uploaded
                string oldImageName = weapon.WeaponImage;

                //check if user uploaded a file
                if (weapon.UploadedImage != null)
                {
                    //check the file extention
                    string ext = Path.GetExtension(weapon.UploadedImage.FileName);

                    //list of valid extentions
                    string[] validExts = { ".jpg", ".jpeg", ".gif", ".png" };

                    //check file extentions against the list ofvaild extentions and check file size
                    if (validExts.Contains(ext.ToLower()) && weapon.UploadedImage.Length < 4_194_303)
                    {
                        //generate a unique filename
                        weapon.WeaponImage = Guid.NewGuid() + ext;

                        // -retrive the path from wwwroot
                        string webRootPath = _webHostEnvironment.WebRootPath;

                        //define filepath to save our image
                        //string fullPath = _webHostEnvironment.WebRootPath + "/images/";
                        string fullPath = _webHostEnvironment.WebRootPath + "/img/";

                        //delete old image\
                        if (oldImageName != "~/noimage.png" && oldImageName != null)
                        {
                            ImageUtility.Delete(fullPath, oldImageName);
                        }

                        using (var memoryStream = new MemoryStream())
                        {
                            await weapon.UploadedImage.CopyToAsync(memoryStream);
                            using (var img = Image.FromStream(memoryStream))
                            {
                                int maxImageSize = 500;
                                int maxThumbSize = 100;
                                ImageUtility.ResizeImage(fullPath, weapon.WeaponImage, img, maxImageSize, maxThumbSize);
                            }
                        }  
                    }
                    else
                    {
                        weapon.WeaponImage = "noimage.png";
                    }

                }
                #endregion


                try
                {
                    _context.Update(weapon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(weapon.WeaponId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Shop));
                return RedirectToAction(nameof(WeaponShop));
            }
            ViewData["AbilityId"] = new SelectList(_context.Abilities, "AbilityId", "AbilityGiven");
            ViewData["DamageId"] = new SelectList(_context.Damages, "DamageId", "DamageRange");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "WeaponType");
            ViewData["StockId"] = new SelectList(_context.Stocks, "StockId", "StockStatus"); 
            ViewData["StockId"] = new SelectList(_context.Stocks, "StockId", "StockStatus");
            return View(weapon);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Weapons == null)
            {
                return NotFound();
            }

            var weapon = await _context.Weapons
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.WeaponId == id);
            if (weapon == null)
            {
                return NotFound();
            }

            return View(weapon);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Weapons == null)
            {
                return Problem("Entity set 'GadgetStoreContext.Products'  is null.");
            }
            var weapon = await _context.Weapons.FindAsync(id);
            if (weapon != null)
            {
                _context.Weapons.Remove(weapon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return (_context.Weapons?.Any(e => e.WeaponId == id)).GetValueOrDefault();
        }
    }
}
