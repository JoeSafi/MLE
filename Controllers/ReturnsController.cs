
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
    public class ReturnsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReturnsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendorReturn>>> GetReturns()
        {
            return await _context.VendorReturns.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VendorReturn>> GetReturn(int id)
        {
            var vendorReturn = await _context.VendorReturns.FindAsync(id);

            if (vendorReturn == null)
            {
                return NotFound();
            }

            return vendorReturn;
        }

        [HttpPost]
        public async Task<ActionResult<VendorReturn>> PostReturn(VendorReturn vendorReturn)
        {
            if (vendorReturn.Id == 0)
            {
                _context.VendorReturns.Add(vendorReturn);
            }
            else
            {
                _context.Entry(vendorReturn).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReturn", new { id = vendorReturn.Id }, vendorReturn);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReturn(int id)
        {
            var vendorReturn = await _context.VendorReturns.FindAsync(id);
            if (vendorReturn == null)
            {
                return NotFound();
            }

            _context.VendorReturns.Remove(vendorReturn);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
