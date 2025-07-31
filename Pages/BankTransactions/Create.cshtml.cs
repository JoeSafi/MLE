using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MLEFIN.Data;
using MLEFIN.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System;

namespace MLEFIN.Pages.BankTransactions
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BankTransaction BankTransaction { get; set; }

        public IActionResult OnGet()
        {
            BankTransaction = new BankTransaction
            {
                TransactionDate = DateTime.Today
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Debug.WriteLine($"Model error in {state.Key}: {error.ErrorMessage}");
                    }
                }
                return Page();
            }

            _context.BankTransactions.Add(BankTransaction);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
