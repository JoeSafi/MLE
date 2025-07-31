
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MLEFIN.Data;
using MLEFIN.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLEFIN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarrantiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WarrantiesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendorWarranty>>> GetWarranties()
        {
            return await _context.VendorWarranties.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VendorWarranty>> GetWarranty(int id)
        {
            var warranty = await _context.VendorWarranties.FindAsync(id);

            if (warranty == null)
            {
                return NotFound();
            }

            return warranty;
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<ActionResult<IEnumerable<VendorWarranty>>> GetWarrantiesForInvoice(int invoiceId)
        {
            return await _context.VendorWarranties
                .Where(w => w.VendorInvoiceID == invoiceId)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<VendorWarranty>> PostWarranty(VendorWarranty warranty)
        {
            if (warranty.Id == 0)
            {
                _context.VendorWarranties.Add(warranty);
            }
            else
            {
                _context.Entry(warranty).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWarranty", new { id = warranty.Id }, warranty);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarranty(int id)
        {
            var warranty = await _context.VendorWarranties.FindAsync(id);
            if (warranty == null)
            {
                return NotFound();
            }

            _context.VendorWarranties.Remove(warranty);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
