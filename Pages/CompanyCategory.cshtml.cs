using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MLEFIN.Data;
using MLEFIN.Models;

namespace MLEFIN.Pages
{
    public class CompanyCategoryPageModel : PageModel
    {
        private readonly AppDbContext _context;

        public List<CompanyCategory> Categories { get; set; } = new List<CompanyCategory>();
        [BindProperty]
        public CompanyCategory CurrentCategory { get; set; } = new CompanyCategory();
        public bool ShowModal { get; set; }

        public CompanyCategoryPageModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
            ShowModal = false;
        }

        public async Task<IActionResult> OnGetEditAsync(int id)
        {
            await LoadDataAsync();
            CurrentCategory = await _context.CompanyCategories.FindAsync(id) ?? new CompanyCategory();
            ShowModal = true;
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                ShowModal = true;
                return Page();
            }

            try
            {
                if (CurrentCategory.CompanyCategoryID == 0)
                {
                    _context.CompanyCategories.Add(CurrentCategory);
                }
                else
                {
                    _context.CompanyCategories.Update(CurrentCategory);
                }

                await _context.SaveChangesAsync();
                return RedirectToPage();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Error saving category. Please try again.");
                await LoadDataAsync();
                ShowModal = true;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var category = await _context.CompanyCategories.FindAsync(id);
            if (category != null)
            {
                _context.CompanyCategories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        private async Task LoadDataAsync()
        {
            Categories = await _context.CompanyCategories
                .OrderBy(cc => cc.CategoryName)
                .ToListAsync();
        }
    }
}