
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MLEFIN.Data;
using MLEFIN.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MLEFIN.Pages
{
    public class PartsModel : PageModel
    {
        private readonly AppDbContext _context;

        public PartsModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Part> Parts { get; set; }

        [BindProperty]
        public Part Part { get; set; }

        public async Task OnGetAsync()
        {
            Parts = await _context.Parts.ToListAsync();
            Console.WriteLine($"Found {Parts.Count} parts");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Parts = await _context.Parts.ToListAsync();
                return Page();
            }

            if (Part.Id == 0)
            {
                _context.Parts.Add(Part);
            }
            else
            {
                _context.Attach(Part).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var part = await _context.Parts.FindAsync(id);

            if (part != null)
            {
                _context.Parts.Remove(part);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
