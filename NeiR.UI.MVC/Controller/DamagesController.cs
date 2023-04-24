using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NeiR.DATA.EF.Models;

namespace NeiR.UI.MVC.Controllers
{
        [Authorize(Roles = "Admin")]
    public class DamagesController : Controller
    {
        private readonly NeiRContext _context;

        public DamagesController(NeiRContext context)
        {
            _context = context;
        }

        // GET: Damages
        public async Task<IActionResult> Index()
        {
              return _context.Damages != null ? 
                          View(await _context.Damages.ToListAsync()) :
                          Problem("Entity set 'NeiRContext.Damages'  is null.");
        }

        // GET: Damages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Damages == null)
            {
                return NotFound();
            }

            var damage = await _context.Damages
                .FirstOrDefaultAsync(m => m.DamageId == id);
            if (damage == null)
            {
                return NotFound();
            }

            return View(damage);
        }

        // GET: Damages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Damages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DamageId,MinDamg,MaxDamg")] Damage damage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(damage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(damage);
        }

        // GET: Damages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Damages == null)
            {
                return NotFound();
            }

            var damage = await _context.Damages.FindAsync(id);
            if (damage == null)
            {
                return NotFound();
            }
            return View(damage);
        }

        // POST: Damages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DamageId,MinDamg,MaxDamg")] Damage damage)
        {
            if (id != damage.DamageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(damage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DamageExists(damage.DamageId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(damage);
        }

        // GET: Damages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Damages == null)
            {
                return NotFound();
            }

            var damage = await _context.Damages
                .FirstOrDefaultAsync(m => m.DamageId == id);
            if (damage == null)
            {
                return NotFound();
            }

            return View(damage);
        }

        // POST: Damages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Damages == null)
            {
                return Problem("Entity set 'NeiRContext.Damages'  is null.");
            }
            var damage = await _context.Damages.FindAsync(id);
            if (damage != null)
            {
                _context.Damages.Remove(damage);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DamageExists(int id)
        {
          return (_context.Damages?.Any(e => e.DamageId == id)).GetValueOrDefault();
        }
    }
}
