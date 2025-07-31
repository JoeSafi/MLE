using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MLEFIN.Data;
using MLEFIN.Models;

namespace MLEFIN.Pages
{
    public class CompanyModel : PageModel
    {
        private readonly AppDbContext _context;

        public CompanyModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Company> Companies { get; set; } = new List<Company>();

        [BindProperty]
        public Company Company { get; set; } = new Company();

        public List<SelectListItem> CategoryList { get; set; } = new List<SelectListItem>();

        public async Task OnGetAsync()
        {
            Company = new Company(); // Reset the company object
            await LoadDataAsync();
        }

        public async Task<IActionResult> OnGetEditAsync(int id)
        {
            Company = await _context.Companies
                .FirstOrDefaultAsync(c => c.CompanyID == id) ?? new Company();

            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            // Remove the Category from validation since we're only binding CompanyCategoryID
            ModelState.Remove("Company.Category");

            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                return Page();
            }

            try
            {
                // Verify the category exists
                var categoryExists = await _context.CompanyCategories
                    .AnyAsync(c => c.CompanyCategoryID == Company.CompanyCategoryID);

                if (!categoryExists)
                {
                    ModelState.AddModelError("Company.CompanyCategoryID", "Invalid category selected");
                    await LoadDataAsync();
                    return Page();
                }

                if (Company.CompanyID == 0)
                {
                    // Adding new company
                    _context.Companies.Add(Company);
                }
                else
                {
                    // Updating existing company
                    var existingCompany = await _context.Companies
                        .FirstOrDefaultAsync(c => c.CompanyID == Company.CompanyID);

                    if (existingCompany != null)
                    {
                        // Update properties
                        existingCompany.CompanyName = Company.CompanyName;
                        existingCompany.CompanyCategoryID = Company.CompanyCategoryID;
                        existingCompany.Address = Company.Address;
                        existingCompany.ContactName = Company.ContactName;
                        existingCompany.Phone = Company.Phone;
                        existingCompany.CellPhone = Company.CellPhone;
                        existingCompany.Email = Company.Email;
                        existingCompany.PaymentTerms = Company.PaymentTerms;
                        existingCompany.CardOnFile = Company.CardOnFile;
                        existingCompany.CardNumber = Company.CardNumber; // This will automatically convert to CardNumberDecimal
                        existingCompany.Check = Company.Check;
                        existingCompany.Cash = Company.Cash;
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToPage();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", $"Error saving company: {ex.Message}");
                await LoadDataAsync();
                return Page();
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                Companies = await _context.Companies
                    .Include(c => c.Category)
                    .OrderBy(c => c.CompanyName)
                    .ToListAsync();

                CategoryList = await _context.CompanyCategories
                    .OrderBy(cc => cc.CategoryName)
                    .Select(cc => new SelectListItem
                    {
                        Value = cc.CompanyCategoryID.ToString(),
                        Text = cc.CategoryName
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in LoadDataAsync: {ex.Message}");
                throw;
            }
        }
    }
}