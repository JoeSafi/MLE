using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MLEFIN.Data;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class PartsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PartsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("search")]
    public async Task < IActionResult > Search([FromQuery] string term)
    {
        var query = _context.Parts.AsQueryable();

        if (!string.IsNullOrEmpty(term)) {
            query = query.Where(p => p.PartNumber.Contains(term) || p.Description.Contains(term));
        }

        var parts = await query
            .Select(p => new {
                id = p.Id,
                // Your improved text for the dropdown display
                text = p.PartNumber + " - " + p.Description,
                // Creating a specific 'part' object to match the JavaScript's expectation
                part = new {
                    description = p.Description,
                    currentCost = p.Cost
                }
            })
            // This is the crucial performance improvement
            .Take(20)
            .ToListAsync();

        return Ok(new { results = parts });
    }
}