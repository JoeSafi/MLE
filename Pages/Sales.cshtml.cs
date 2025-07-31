using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MLEFIN.Data;
using MLEFIN.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MLEFIN.Pages
{
    public class SalesModel : PageModel
    {
        private readonly AppDbContext _context;

        public SalesModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Sales SalesRecord { get; set; } = new Sales();

        [BindProperty]
        public string ChecksJson { get; set; } = "[]";

        public List<Sales> SalesRecords { get; set; } = new List<Sales>();

        // Filter properties
        [BindProperty(SupportsGet = true)]
        public DateTime? DateFrom { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? DateTo { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "DateDesc";

        // Dropdown lists
        public SelectList SortByList { get; set; } = new SelectList(new List<object>());

        private void PopulateDropdowns()
        {
            // Sort options
            var sortOptions = new List<object>
            {
                new { Value = "DateDesc", Text = "Date (Newest First)" },
                new { Value = "DateAsc", Text = "Date (Oldest First)" },
                new { Value = "TotalSalesDesc", Text = "Total Sales (Highest First)" },
                new { Value = "TotalSalesAsc", Text = "Total Sales (Lowest First)" },
                new { Value = "CarCountDesc", Text = "Car Count (Highest First)" },
                new { Value = "CarCountAsc", Text = "Car Count (Lowest First)" }
            };
            SortByList = new SelectList(sortOptions, "Value", "Text");
        }

        public async Task OnGetAsync()
        {
            // Set default date range to current month
            if (!DateFrom.HasValue && !DateTo.HasValue)
            {
                DateFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                DateTo = DateFrom.Value.AddMonths(1).AddDays(-1);
            }

            // Set default sort if not provided
            if (string.IsNullOrEmpty(SortBy))
            {
                SortBy = "DateDesc";
            }

            PopulateDropdowns();
            await LoadSalesRecords();
        }

        public async Task<IActionResult> OnGetEditAsync(int id)
        {
            try
            {
                var salesRecord = await _context.Sales.FindAsync(id);
                if (salesRecord == null)
                    return NotFound();

                SalesRecord = salesRecord;

                // Load existing checks for this sales record
                await LoadChecksForSalesRecord(salesRecord);

                // Preserve current filters when editing
                if (!DateFrom.HasValue && !DateTo.HasValue)
                {
                    DateFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    DateTo = DateFrom.Value.AddMonths(1).AddDays(-1);
                }

                if (string.IsNullOrEmpty(SortBy))
                {
                    SortBy = "DateDesc";
                }

                PopulateDropdowns();
                await LoadSalesRecords();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading sales record: {ex.Message}");
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnGetViewAsync(int id)
        {
            try
            {
                var salesRecord = await _context.Sales.FindAsync(id);
                if (salesRecord == null)
                    return NotFound();

                SalesRecord = salesRecord;

                // Load existing checks for this sales record
                await LoadChecksForSalesRecord(salesRecord);

                // Preserve current filters when viewing
                if (!DateFrom.HasValue && !DateTo.HasValue)
                {
                    DateFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    DateTo = DateFrom.Value.AddMonths(1).AddDays(-1);
                }

                if (string.IsNullOrEmpty(SortBy))
                {
                    SortBy = "DateDesc";
                }

                PopulateDropdowns();
                await LoadSalesRecords();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading sales record: {ex.Message}");
                return RedirectToPage();
            }
        }

        private async Task LoadChecksForSalesRecord(Sales salesRecord)
        {
            if (salesRecord != null)
            {
                try
                {
                    // Load check transactions from BankTransactions table using new CheckNumber and Payer columns
                    var checkTransactions = await _context.BankTransactions
                        .Where(bt => bt.TransactionDate.Date == salesRecord.SalesDate.Date &&
                                    bt.Description == "From Sales" &&
                                    bt.Type == "Deposit" &&
                                    bt.DepositType == "Check")
                        .ToListAsync();

                    var checks = checkTransactions.Select(ct => new CheckTransaction
                    {
                        CheckNumber = ct.CheckNumber ?? "",
                        Amount = ct.Amount,
                        PayerName = ct.Payer ?? ""
                    }).ToList();

                    salesRecord.Checks = checks;
                    ChecksJson = JsonSerializer.Serialize(checks);

                    System.Diagnostics.Debug.WriteLine($"Loaded {checks.Count} checks for sales record on {salesRecord.SalesDate.ToShortDateString()}");
                }
                catch (Exception ex)
                {
                    // Log error but don't fail - just set empty checks
                    System.Diagnostics.Debug.WriteLine($"Error loading checks: {ex.Message}");
                    salesRecord.Checks = new List<CheckTransaction>();
                    ChecksJson = "[]";
                }
            }
        }

        private async Task LoadSalesRecords()
        {
            try
            {
                var query = _context.Sales.AsQueryable();

                // Apply filters
                if (DateFrom.HasValue)
                    query = query.Where(s => s.SalesDate.Date >= DateFrom.Value.Date);
                if (DateTo.HasValue)
                    query = query.Where(s => s.SalesDate.Date <= DateTo.Value.Date);

                var results = await query.ToListAsync();

                // Load checks for each sales record
                foreach (var salesRecord in results)
                {
                    await LoadChecksForSalesRecord(salesRecord);
                }

                // Apply sorting
                results = SortBy switch
                {
                    "DateAsc" => results.OrderBy(s => s.SalesDate).ThenBy(s => s.Id).ToList(),
                    "DateDesc" => results.OrderByDescending(s => s.SalesDate).ThenByDescending(s => s.Id).ToList(),
                    "TotalSalesAsc" => results.OrderBy(s => s.TotalSales).ThenBy(s => s.SalesDate).ToList(),
                    "TotalSalesDesc" => results.OrderByDescending(s => s.TotalSales).ThenBy(s => s.SalesDate).ToList(),
                    "CarCountAsc" => results.OrderBy(s => s.CarCount).ThenBy(s => s.SalesDate).ToList(),
                    "CarCountDesc" => results.OrderByDescending(s => s.CarCount).ThenBy(s => s.SalesDate).ToList(),
                    _ => results.OrderByDescending(s => s.SalesDate).ThenByDescending(s => s.Id).ToList()
                };

                SalesRecords = results;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading sales records: {ex.Message}");
                SalesRecords = new List<Sales>();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove filter-related validation
            var keysToRemove = new[] { "SortBy", "DateFrom", "DateTo" };

            foreach (var key in keysToRemove)
            {
                ModelState.Remove(key);
                if (ModelState.ContainsKey(key))
                {
                    ModelState[key].Errors.Clear();
                    ModelState[key].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                }
            }

            try
            {
                // Parse checks from JSON with better error handling
                List<CheckTransaction> checks = new List<CheckTransaction>();

                System.Diagnostics.Debug.WriteLine($"=== PROCESSING CHECKS JSON ===");
                System.Diagnostics.Debug.WriteLine($"Raw ChecksJson received: '{ChecksJson}'");
                System.Diagnostics.Debug.WriteLine($"ChecksJson is null: {ChecksJson == null}");
                System.Diagnostics.Debug.WriteLine($"ChecksJson is empty: {string.IsNullOrEmpty(ChecksJson)}");
                System.Diagnostics.Debug.WriteLine($"ChecksJson length: {ChecksJson?.Length ?? 0}");

                if (!string.IsNullOrEmpty(ChecksJson))
                {
                    try
                    {
                        // Configure JsonSerializer options for case-insensitive deserialization
                        var jsonOptions = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                        };

                        var allChecks = JsonSerializer.Deserialize<List<CheckTransaction>>(ChecksJson, jsonOptions) ?? new List<CheckTransaction>();

                        System.Diagnostics.Debug.WriteLine($"Deserialized {allChecks.Count} checks from JSON");

                        // Debug each check before filtering
                        for (int i = 0; i < allChecks.Count; i++)
                        {
                            var check = allChecks[i];
                            System.Diagnostics.Debug.WriteLine($"Check {i}: Number='{check.CheckNumber}', Amount={check.Amount}, Payer='{check.PayerName}'");
                        }

                        // Filter out empty checks - only include checks with all required fields
                        checks = allChecks.Where(c =>
                            c.Amount > 0 &&
                            !string.IsNullOrWhiteSpace(c.CheckNumber) &&
                            !string.IsNullOrWhiteSpace(c.PayerName)
                        ).ToList();

                        System.Diagnostics.Debug.WriteLine($"Valid checks after filtering: {checks.Count}");

                        // Debug each valid check
                        foreach (var check in checks)
                        {
                            System.Diagnostics.Debug.WriteLine($"Valid Check: #{check.CheckNumber}, Amount: {check.Amount}, Payer: {check.PayerName}");
                        }
                    }
                    catch (JsonException ex)
                    {
                        ModelState.AddModelError("ChecksJson", $"Invalid check data format: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"JSON parsing error: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"JSON that failed to parse: '{ChecksJson}'");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ChecksJson is null or empty - no checks to process");
                }

                System.Diagnostics.Debug.WriteLine($"Final result: {checks.Count} valid checks will be processed");
                System.Diagnostics.Debug.WriteLine($"=== END CHECKS JSON PROCESSING ===");

                // Validate checks
                foreach (var check in checks)
                {
                    if (string.IsNullOrWhiteSpace(check.CheckNumber))
                    {
                        ModelState.AddModelError("ChecksJson", "Check number is required for all checks.");
                        break;
                    }
                    if (string.IsNullOrWhiteSpace(check.PayerName))
                    {
                        ModelState.AddModelError("ChecksJson", "Payer name is required for all checks.");
                        break;
                    }
                    if (check.Amount <= 0)
                    {
                        ModelState.AddModelError("ChecksJson", "Check amount must be greater than 0.");
                        break;
                    }
                }

                // Check if a sales record already exists for this date (for new records)
                if (SalesRecord.Id == 0)
                {
                    var existingRecord = await _context.Sales
                        .FirstOrDefaultAsync(s => s.SalesDate.Date == SalesRecord.SalesDate.Date);

                    if (existingRecord != null)
                    {
                        ModelState.AddModelError("SalesRecord.SalesDate", "A sales record already exists for this date.");
                    }
                }

                if (!ModelState.IsValid)
                {
                    // Preserve filter values
                    if (!DateFrom.HasValue && !DateTo.HasValue)
                    {
                        DateFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                        DateTo = DateFrom.Value.AddMonths(1).AddDays(-1);
                    }
                    if (string.IsNullOrEmpty(SortBy))
                    {
                        SortBy = "DateDesc";
                    }

                    PopulateDropdowns();
                    await LoadSalesRecords();
                    return Page();
                }

                if (SalesRecord.Id > 0)
                {
                    // Update existing sales record
                    var existing = await _context.Sales.FindAsync(SalesRecord.Id);
                    if (existing != null)
                    {
                        // Remove existing bank transactions for this sales record
                        var existingBankTransactions = await _context.BankTransactions
                            .Where(bt => bt.TransactionDate.Date == existing.SalesDate.Date &&
                                        bt.Description == "From Sales" &&
                                        bt.Type == "Deposit")
                            .ToListAsync();

                        _context.BankTransactions.RemoveRange(existingBankTransactions);

                        // Update sales record
                        existing.SalesDate = SalesRecord.SalesDate;
                        existing.Lube = SalesRecord.Lube;
                        existing.AutoRepair = SalesRecord.AutoRepair;
                        existing.Tires = SalesRecord.Tires;
                        existing.OtherSales = SalesRecord.OtherSales;
                        existing.Tax = SalesRecord.Tax;
                        existing.CarCount = SalesRecord.CarCount;
                        existing.Bank = SalesRecord.Bank;
                        existing.Cash = SalesRecord.Cash;

                        _context.Sales.Update(existing);

                        // Create new bank transactions for updated amounts and checks
                        await CreateBankTransactions(existing, checks);
                    }
                }
                else
                {
                    // Add new sales record
                    _context.Sales.Add(SalesRecord);
                    await _context.SaveChangesAsync(); // Save to get the ID

                    // Create bank transactions
                    await CreateBankTransactions(SalesRecord, checks);
                }

                await _context.SaveChangesAsync();

                // Preserve current filter settings in redirect
                var redirectUrl = $"/Sales?DateFrom={DateFrom:yyyy-MM-dd}&DateTo={DateTo:yyyy-MM-dd}&SortBy={SortBy}";
                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error saving sales record: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Sales save error: {ex.Message}");

                // Preserve filter values on error
                if (!DateFrom.HasValue && !DateTo.HasValue)
                {
                    DateFrom = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    DateTo = DateFrom.Value.AddMonths(1).AddDays(-1);
                }
                if (string.IsNullOrEmpty(SortBy))
                {
                    SortBy = "DateDesc";
                }

                PopulateDropdowns();
                await LoadSalesRecords();
                return Page();
            }
        }

        private async Task CreateBankTransactions(Sales salesRecord, List<CheckTransaction> checks)
        {
            try
            {
                // Validate that required categories/companies exist
                var salesCategory = await _context.CompanyCategories.FindAsync(9);
                var salesCompany = await _context.Companies.FindAsync(6);

                if (salesCategory == null)
                {
                    throw new InvalidOperationException("Sales category (ID: 9) not found. Please ensure the category exists in the database.");
                }

                if (salesCompany == null)
                {
                    throw new InvalidOperationException("Sales company (ID: 6) not found. Please ensure the company exists in the database.");
                }

                // Create Bank transaction if Bank amount > 0 (CC ACH)
                if (salesRecord.Bank > 0)
                {
                    var bankTransaction = new BankTransaction
                    {
                        TransactionDate = salesRecord.SalesDate,
                        Account = "Bank",
                        Amount = salesRecord.Bank,
                        Type = "Deposit",
                        Description = "From Sales",
                        Payee = "Sales",
                        CompanyCategoryID = 9, // Sales category
                        CompanyID = 6, // Sales company
                        DepositType = "CC ACH",
                        CheckNumber = null, // No check for CC ACH
                        Payer = null // No payer for CC ACH
                    };
                    _context.BankTransactions.Add(bankTransaction);
                    System.Diagnostics.Debug.WriteLine($"Created CC ACH transaction: ${salesRecord.Bank}");
                }

                // Create Cash transaction if Cash amount > 0
                if (salesRecord.Cash > 0)
                {
                    var cashTransaction = new BankTransaction
                    {
                        TransactionDate = salesRecord.SalesDate,
                        Account = "Cash",
                        Amount = salesRecord.Cash,
                        Type = "Deposit",
                        Description = "From Sales",
                        Payee = "Sales",
                        CompanyCategoryID = 9, // Sales category
                        CompanyID = 6, // Sales company
                        DepositType = "Cash",
                        CheckNumber = null, // No check for Cash
                        Payer = null // No payer for Cash
                    };
                    _context.BankTransactions.Add(cashTransaction);
                    System.Diagnostics.Debug.WriteLine($"Created Cash transaction: ${salesRecord.Cash}");
                }

                // Create Check transactions for each valid check using new columns
                foreach (var check in checks)
                {
                    if (check.Amount > 0 &&
                        !string.IsNullOrWhiteSpace(check.CheckNumber) &&
                        !string.IsNullOrWhiteSpace(check.PayerName))
                    {
                        var checkTransaction = new BankTransaction
                        {
                            TransactionDate = salesRecord.SalesDate,
                            Account = "Bank",
                            Amount = check.Amount,
                            Type = "Deposit",
                            Description = "From Sales",
                            Payee = $"Check #{check.CheckNumber} - {check.PayerName}", // Keep legacy format for display
                            CompanyCategoryID = 9, // Sales category
                            CompanyID = 6, // Sales company
                            DepositType = "Check",
                            CheckNumber = check.CheckNumber, // New dedicated column
                            Payer = check.PayerName // New dedicated column
                        };
                        _context.BankTransactions.Add(checkTransaction);
                        System.Diagnostics.Debug.WriteLine($"Created Check transaction: ${check.Amount} from {check.PayerName} (Check #{check.CheckNumber})");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Created {(salesRecord.Bank > 0 ? 1 : 0) + (salesRecord.Cash > 0 ? 1 : 0) + checks.Count} bank transactions for sales record");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating bank transactions: {ex.Message}");
                throw new Exception($"Error creating bank transactions: {ex.Message}", ex);
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var salesRecord = await _context.Sales.FindAsync(id);
                if (salesRecord != null)
                {
                    // Remove associated bank transactions
                    var bankTransactions = await _context.BankTransactions
                        .Where(bt => bt.TransactionDate.Date == salesRecord.SalesDate.Date &&
                                    bt.Description == "From Sales" &&
                                    bt.Type == "Deposit")
                        .ToListAsync();

                    _context.BankTransactions.RemoveRange(bankTransactions);
                    _context.Sales.Remove(salesRecord);
                    await _context.SaveChangesAsync();

                    System.Diagnostics.Debug.WriteLine($"Deleted sales record and {bankTransactions.Count} associated bank transactions");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error deleting sales record: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Sales delete error: {ex.Message}");
            }

            return RedirectToPage();
        }
    }
}