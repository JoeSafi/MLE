using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MLEFIN.Data;
using MLEFIN.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MLEFIN.Pages
{
    public class VendorPurchaseModel : PageModel
    {
        private readonly AppDbContext _context;

        public IList<VendorInvoice> Invoices { get; set; }

        [BindProperty]
        public VendorInvoice VendorInvoiceRecord { get; set; } = new VendorInvoice();

        // Statistics properties
        public decimal TotalAmount { get; set; }
        public int TotalCount { get; set; }
        public decimal AverageAmount { get; set; }

        // Filter properties
        [BindProperty(SupportsGet = true)]
        public DateTime? DateFrom { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? DateTo { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? VendorFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        [Display(Name = "Sort Order")]
        public string? SortBy { get; set; } = "DateDesc";
                public VendorPurchaseModel(AppDbContext context)
        {
            _context = context;
            Invoices = new List<VendorInvoice>();
        }
        // Dropdown properties
        public SelectList Vendors { get; set; }
        public SelectList Categories { get; set; }
        public SelectList SortByOptions { get; private set; }
        public IList<Part> Parts { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                // Initialize SortBy if not set
                SortBy ??= "DateDesc";

                // Load vendors for dropdown
                var companies = await _context.Companies
                    .OrderBy(c => c.CompanyName)
                    .ToListAsync();

                Vendors = new SelectList(companies, "CompanyID", "CompanyName");

                var categories = await _context.CompanyCategories
                    .OrderBy(c => c.CategoryName)
                    .ToListAsync();

                Categories = new SelectList(categories, "CompanyCategoryID", "CategoryName");

                Parts = await _context.Parts.ToListAsync();

                // Build query
                var query = _context.VendorInvoices
                    .Include(i => i.VendorCompany)
                        .ThenInclude(vc => vc.Category)
                    .AsQueryable();

                // Apply filters
                if (DateFrom.HasValue)
                    query = query.Where(i => i.InvoiceDate.Date >= DateFrom.Value.Date);

                if (DateTo.HasValue)
                    query = query.Where(i => i.InvoiceDate.Date <= DateTo.Value.Date);

                if (VendorFilter.HasValue)
                    query = query.Where(i => i.VendorCompanyID == VendorFilter.Value);

                // Apply sorting
                query = SortBy switch
                {
                    "DateAsc" => query.OrderBy(i => i.InvoiceDate),
                    "AmountDesc" => query.OrderByDescending(i => i.Amount),
                    "AmountAsc" => query.OrderBy(i => i.Amount),
                    _ => query.OrderByDescending(i => i.InvoiceDate)
                };

                // Execute query and store results
                Invoices = await query.ToListAsync();

                // Calculate statistics
                TotalAmount = Invoices.Sum(i => i.Amount);
                TotalCount = Invoices.Count;
                AverageAmount = TotalCount > 0 ? TotalAmount / TotalCount : 0;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error loading invoices: " + ex.Message);
                Invoices = new List<VendorInvoice>();
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            // If the submitted data is not valid, we need to rebuild the page
            // with the validation errors.
            if (!ModelState.IsValid)
            {
                // Repopulate only the data needed for dropdowns, NOT the entire invoice list.
                var companies = await _context.Companies.OrderBy(c => c.CompanyName).ToListAsync();
                Vendors = new SelectList(companies, "CompanyID", "CompanyName");

                var categories = await _context.CompanyCategories.OrderBy(c => c.CategoryName).ToListAsync();
                Categories = new SelectList(categories, "CompanyCategoryID", "CategoryName");

                // Return the page so the user can see the validation errors.
                return Page();
            }

            // If Id is 0, it's a new record. Otherwise, it's an update.
            if (VendorInvoiceRecord.Id == 0)
            {
                _context.VendorInvoices.Add(VendorInvoiceRecord);
            }
            else
            {
                _context.VendorInvoices.Update(VendorInvoiceRecord);
            }

            await _context.SaveChangesAsync();

            // Redirect to the GET page to prevent form re-submission on refresh.
            return RedirectToPage();
        }
    }
}