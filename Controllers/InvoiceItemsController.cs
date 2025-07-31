using Microsoft.AspNetCore.Mvc;
using MLEFIN.Data;
using MLEFIN.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLEFIN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceItemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InvoiceItemsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("{invoiceId}")]
        public async Task<IActionResult> PostInvoiceItems(int invoiceId, [FromBody] List<VendorInvoiceItem> items)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingItems = _context.VendorInvoiceItems.Where(i => i.VendorInvoiceID == invoiceId);
            _context.VendorInvoiceItems.RemoveRange(existingItems);

            foreach (var item in items)
            {
                item.VendorInvoiceID = invoiceId;
                _context.VendorInvoiceItems.Add(item);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}