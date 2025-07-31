
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
    public class CoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CoresController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendorCore>>> GetCores()
        {
            return await _context.VendorCores.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VendorCore>> GetCore(int id)
        {
            var core = await _context.VendorCores.FindAsync(id);

            if (core == null)
            {
                return NotFound();
            }

            return core;
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<ActionResult<IEnumerable<VendorCore>>> GetCoresForInvoice(int invoiceId)
        {
            return await _context.VendorCores
                .Where(c => c.VendorInvoiceID == invoiceId)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<VendorCore>> PostCore(VendorCore core)
        {
            if (core.Id == 0)
            {
                _context.VendorCores.Add(core);
            }
            else
            {
                _context.Entry(core).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCore", new { id = core.Id }, core);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCore(int id)
        {
            var core = await _context.VendorCores.FindAsync(id);
            if (core == null)
            {
                return NotFound();
            }

            _context.VendorCores.Remove(core);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
