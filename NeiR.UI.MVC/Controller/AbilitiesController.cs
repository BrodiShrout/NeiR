using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NeiR.DATA.EF.Models;
using Microsoft.AspNetCore.Authorization;//added to lock down controller


namespace NeiR.UI.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AbilitiesController : Controller
    {
        private readonly NeiRContext _context;

        public AbilitiesController(NeiRContext context)
        {
            _context = context;
        }

        // GET: Abilities
        public async Task<IActionResult> Index()
        {
              return _context.Abilities != null ? 
                          View(await _context.Abilities.ToListAsync()) :
                          Problem("Entity set 'NeiRContext.Abilities'  is null.");
        }

        //AShop
        #region AJAX CRUD Functionality

        [AcceptVerbs("POST")]
        public JsonResult AjaxDelete(int id)
        {
            Ability ablility = _context.Abilities.Find(id);

            _context.Abilities.Remove(ablility);

            _context.SaveChanges();

            string confirmMessage = $"Deleted the Ability, {ablility.AbilityGiven}, from the database!";

            return Json(new { id = id, message = confirmMessage });
        }

        public PartialViewResult AbilityDetails(int id)
        {

            var ablility = _context.Abilities.Find(id);

            return PartialView(ablility);

            //HOW WE CREATED THE PARTIAL VIEW
            //right click in the acion
            //add view
            //razor view
            //hit details
            //make the model Category
            //DATA layer context
            //check 'create as partial view'

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AjaxCreate(Ability ablility)
        {

            _context.Abilities.Add(ablility);

            return Json(ablility);

            //We need a partial view to contain the form to create a Category
            //We will scaffold a partial view for this, CategoryCreate, using the folowing settings
            //Name: CategoryCreate
            //Create
            //Category Model
            //GadgetStoreContext
            //Create partial view
        }
        #endregion

        #region Original Scaffold

        //// GET: Abilities/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.Abilities == null)
        //    {
        //        return NotFound();
        //    }

        //    var ability = await _context.Abilities
        //        .FirstOrDefaultAsync(m => m.AbilityId == id);
        //    if (ability == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(ability);
        //}

        //// GET: Abilities/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Abilities/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("AbilityId,AbilityGiven")] Ability ability)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(ability);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(ability);
        //}

        //// GET: Abilities/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Abilities == null)
        //    {
        //        return NotFound();
        //    }

        //    var ability = await _context.Abilities.FindAsync(id);
        //    if (ability == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(ability);
        //}

        //// POST: Abilities/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("AbilityId,AbilityGiven")] Ability ability)
        //{
        //    if (id != ability.AbilityId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(ability);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!AbilityExists(ability.AbilityId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(ability);
        //}

        //// GET: Abilities/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Abilities == null)
        //    {
        //        return NotFound();
        //    }

        //    var ability = await _context.Abilities
        //        .FirstOrDefaultAsync(m => m.AbilityId == id);
        //    if (ability == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(ability);
        //}

        //// POST: Abilities/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.Abilities == null)
        //    {
        //        return Problem("Entity set 'NeiRContext.Abilities'  is null.");
        //    }
        //    var ability = await _context.Abilities.FindAsync(id);
        //    if (ability != null)
        //    {
        //        _context.Abilities.Remove(ability);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        #endregion
        private bool AbilityExists(int id)
        {
          return (_context.Abilities?.Any(e => e.AbilityId == id)).GetValueOrDefault();
        }
    }
}
