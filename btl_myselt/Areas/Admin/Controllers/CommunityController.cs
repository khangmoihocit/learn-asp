using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using btl_myselt.Data;
using btl_myselt.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace btl_myselt.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CommunityController : Controller
    {
        public ApplicationDbContext _context;
        public CommunityController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var communities = await _context.Communities.ToListAsync();
            return View(communities);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Community community)
        {
            if (ModelState.IsValid)
            {
                _context.Add(community);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(community);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var community = await _context.Communities.FindAsync(id);
            if (community == null) return NotFound();

            return View(community);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Community community)
        {
            if (id != community.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(community);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return View(community);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id){
            var community = await _context.Communities.FindAsync(id);
            if(community != null){
                _context.Communities.Remove(community);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id){
            var community = await _context.Communities.FindAsync(id);
            if(community == null) return NotFound();

            community.IsActive = !community.IsActive;
            _context.Communities.Update(community);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

    }
}