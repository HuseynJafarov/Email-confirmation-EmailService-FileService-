using EmbryoFrontToBack.Data;
using EmbryoFrontToBack.Models;
using EmbryoFrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmbryoFrontToBack.Controllers
{
    public class AccessoriesController : Controller
    {
        private readonly AppDbContext _context;

        public AccessoriesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
           
            ViewBag.count = await _context.Accessories.Where(m=>!m.IsDeleted).CountAsync();

            IEnumerable<Accessories> accessories = await _context.Accessories.Where(m=>!m.IsDeleted).Take(3).OrderBy(m=>m.Id).ToListAsync();

            AccessoriesVM accessoriesVM = new AccessoriesVM
            {
               Accessories = accessories
            };
            return View(accessoriesVM);
        }

        public async Task<IActionResult> LoadMore(int skip)
        {
            IEnumerable<Accessories> accessories = await _context.Accessories.Where(m=>!m.IsDeleted).Skip(skip).Take(3).ToListAsync();
            return PartialView("_ProductPartial", accessories);
        }

    }
}
