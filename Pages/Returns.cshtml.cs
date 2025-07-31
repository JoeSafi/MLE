
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MLEFIN.Data;
using MLEFIN.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLEFIN.Pages
{
    public class ReturnsModel : PageModel
    {
        private readonly AppDbContext _context;

        public ReturnsModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<VendorReturn> Returns { get; set; }
        public SelectList Vendors { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? VendorFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string DateFilter { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.VendorReturns.AsQueryable();

            if (VendorFilter.HasValue)
            {
                query = query.Where(r => r.VendorCompanyID == VendorFilter.Value);
            }

            if (!string.IsNullOrEmpty(DateFilter))
            {
                if (System.DateTime.TryParse(DateFilter, out var date))
                {
                    query = query.Where(r => r.ReturnDate.Date == date.Date);
                }
            }

            Returns = await query.Include(r => r.VendorCompany).ToListAsync();
            Vendors = new SelectList(await _context.Companies.ToListAsync(), "CompanyID", "CompanyName");
        }
    }
}
