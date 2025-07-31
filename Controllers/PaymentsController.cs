
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
    public class PaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendorPayment>>> GetPayments()
        {
            return await _context.VendorPayments.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VendorPayment>> GetPayment(int id)
        {
            var payment = await _context.VendorPayments.FindAsync(id);

            if (payment == null)
            {
                return NotFound();
            }

            return payment;
        }

        [HttpPost]
        public async Task<ActionResult<VendorPayment>> PostPayment(VendorPayment payment)
        {
            if (payment.Id == 0)
            {
                _context.VendorPayments.Add(payment);
            }
            else
            {
                _context.Entry(payment).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPayment", new { id = payment.Id }, payment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.VendorPayments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            _context.VendorPayments.Remove(payment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
