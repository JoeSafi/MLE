
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
    public class VendorPaymentModel : PageModel
    {
        private readonly AppDbContext _context;

        public VendorPaymentModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<VendorPayment> Payments { get; set; }
        public SelectList Vendors { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? VendorFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string DateFilter { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.VendorPayments.AsQueryable();

            if (VendorFilter.HasValue)
            {
                query = query.Where(p => p.VendorCompanyID == VendorFilter.Value);
            }

            if (!string.IsNullOrEmpty(DateFilter))
            {
                if (System.DateTime.TryParse(DateFilter, out var date))
                {
                    query = query.Where(p => p.PaymentDate.Date == date.Date);
                }
            }

            Payments = await query.Include(p => p.VendorCompany).ToListAsync();
            Vendors = new SelectList(await _context.Companies.ToListAsync(), "CompanyID", "CompanyName");
        }
    }
}
