using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MLEFIN.Data;
using MLEFIN.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MLEFIN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InvoicesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendorInvoice>>> GetInvoices()
        {
            return await _context.VendorInvoices.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VendorInvoice>> GetInvoice(int id)
        {
            var invoice = await _context.VendorInvoices
                .Include(i => i.VendorCompany)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return invoice;
        }

        [HttpPost]
        public async Task<ActionResult<VendorInvoice>> PostInvoice(VendorInvoice invoice)
        {
            if (invoice.Id == 0)
            {
                _context.VendorInvoices.Add(invoice);
            }
            else
            {
                _context.Entry(invoice).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInvoice", new { id = invoice.Id }, invoice);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.VendorInvoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            _context.VendorInvoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}