using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MLEFIN.Data;
using MLEFIN.Models;

namespace MLEFIN.Pages
{
    public class BankModel : PageModel
    {
        private readonly AppDbContext _context;

        public BankModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BankTransaction Transaction { get; set; } = new BankTransaction();

        [BindProperty]
        public bool IsSalesTransaction { get; set; }

        public List<BankTransaction> Transactions { get; set; } = new List<BankTransaction>();

        // Filter properties
        [BindProperty(SupportsGet = true)]
        public DateTime? DateFrom { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? DateTo { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? TypeFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? PayeeFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? DescriptionFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? AmountFrom { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? AmountTo { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string AccountFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? DepositTypeFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "DateDesc";

        // Dropdown lists
        public SelectList AccountList { get; set; } = new SelectList(new List<object>());
        public SelectList TypeList { get; set; } = new SelectList(new List<object>());
        public SelectList CategoryList { get; set; } = new SelectList(new List<object>());
        public SelectList SortByList { get; set; } = new SelectList(new List<object>());
        public SelectList CompanyList { get; set; } = new SelectList(new List<object>());
        public SelectList PayeeFilterList { get; set; } = new SelectList(new List<object>());
        public SelectList AccountFilterList { get; set; } = new SelectList(new List<object>());
        public SelectList DepositTypeList { get; set; } = new SelectList(new List<object>());
        public SelectList DepositTypeFilterList { get; set; } = new SelectList(new List<object>());

        // Property for category filter options
        public List<dynamic> CategoryFilterOptions { get; set; } = new List<dynamic>();

        private void PopulateDropdowns()
        {
            // Account options
            var accounts = new List<object>
            {
                new { Value = "", Text = "-- Select Account --" },
                new { Value = "Cash", Text = "Cash" },
                new { Value = "Bank", Text = "Bank" }
            };
            AccountList = new SelectList(accounts, "Value", "Text");

            // Transaction types
            var types = new List<object>
            {
                new { Value = "", Text = "-- Select Type --" },
                new { Value = "Credit", Text = "Credit" },
                new { Value = "Debit", Text = "Debit" },
                new { Value = "Deposit", Text = "Deposit" },
                new { Value = "Withdrawal", Text = "Withdrawal" }
            };
            TypeList = new SelectList(types, "Value", "Text");

            // Deposit Types
            var depositTypes = new List<object>
            {
                new { Value = "", Text = "-- Select Deposit Type --" },
                new { Value = "CC ACH", Text = "CC ACH" },
                new { Value = "Check", Text = "Check" },
                new { Value = "Cash", Text = "Cash" }
            };
            DepositTypeList = new SelectList(depositTypes, "Value", "Text");

            // Deposit Type Filter
            var depositTypeFilter = new List<object>
            {
                new { Value = "", Text = "All Deposit Types" },
                new { Value = "CC ACH", Text = "CC ACH" },
                new { Value = "Check", Text = "Check" },
                new { Value = "Cash", Text = "Cash" }
            };
            DepositTypeFilterList = new SelectList(depositTypeFilter, "Value", "Text");

            // Categories - Load from database
            try
            {
                var categories = _context.CompanyCategories
                    .Select(c => new { Value = c.CompanyCategoryID, Text = c.CategoryName })
                    .ToList();

                var categoryOptions = new List<object> { new { Value = 0, Text = "-- Select Category --" } };
                categoryOptions.AddRange(categories.Select(c => new { Value = c.Value, Text = c.Text }));
                CategoryList = new SelectList(categoryOptions, "Value", "Text");

                // Create a property for category filter options instead of ViewBag
                var categoryFilterOptions = new List<dynamic> { new { Value = "", Text = "All Categories" } };
                categoryFilterOptions.AddRange(categories.Select(c => new { Value = c.Value.ToString(), Text = c.Text }));
                CategoryFilterOptions = categoryFilterOptions;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading categories: {ex.Message}");
                CategoryList = new SelectList(new List<object> { new { Value = 0, Text = "-- Select Category --" } }, "Value", "Text");
                CategoryFilterOptions = new List<dynamic>();
            }

            // Sort options
            var sortOptions = new List<object>
            {
                new { Value = "DateDesc", Text = "Date (Newest First)" },
                new { Value = "DateAsc", Text = "Date (Oldest First)" },
                new { Value = "AmountDesc", Text = "Amount (Highest First)" },
                new { Value = "AmountAsc", Text = "Amount (Lowest First)" },
                new { Value = "AccountAsc", Text = "Account (A-Z)" },
                new { Value = "TypeAsc", Text = "Type (A-Z)" },
                new { Value = "PayeeAsc", Text = "Payee (A-Z)" },
                new { Value = "DepositTypeAsc", Text = "Deposit Type (A-Z)" }
            };
            SortByList = new SelectList(sortOptions, "Value", "Text");

            // Company list for Payee dropdown - Load from database
            try
            {
                var companies = _context.Companies
                    .Select(c => new { Value = c.CompanyID, Text = c.CompanyName })
                    .ToList();

                var companyOptions = new List<object> { new { Value = "", Text = "-- Select Company --" } };
                companyOptions.AddRange(companies.Select(c => new { Value = c.Value.ToString(), Text = c.Text }));
                CompanyList = new SelectList(companyOptions, "Value", "Text");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading companies: {ex.Message}");
                CompanyList = new SelectList(new List<object> { new { Value = "", Text = "-- Select Company --" } }, "Value", "Text");
            }

            // Account filter list  
            var accountFilterOptions = new List<object>
            {
                new { Value = "", Text = "All Accounts" },
                new { Value = "Cash", Text = "Cash" },
                new { Value = "Bank", Text = "Bank" }
            };
            AccountFilterList = new SelectList(accountFilterOptions, "Value", "Text");

            // Payee filter list (from existing transactions) - Load from database
            try
            {
                var existingPayees = _context.BankTransactions
                    .Where(t => !string.IsNullOrEmpty(t.Payee))
                    .Select(t => t.Payee)
                    .Distinct()
                    .OrderBy(p => p)
                    .Take(50) // Limit for performance
                    .ToList();

                var payeeFilterOptions = new List<object> { new { Value = "", Text = "All Payees" } };
                // Add special option for sales transactions
                payeeFilterOptions.Add(new { Value = "SALES_ONLY", Text = "Sales Transactions Only" });
                payeeFilterOptions.AddRange(existingPayees.Select(p => new { Value = p, Text = p }));
                PayeeFilterList = new SelectList(payeeFilterOptions, "Value", "Text");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading payees: {ex.Message}");
                PayeeFilterList = new SelectList(new List<object> {
                    new { Value = "", Text = "All Payees" },
                    new { Value = "SALES_ONLY", Text = "Sales Transactions Only" }
                }, "Value", "Text");
            }
        }

        public async Task OnGetAsync()
        {
            // Set default date range
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
            await LoadTransactions();
        }

        public async Task<IActionResult> OnGetEditAsync(int id)
        {
            try
            {
                var transaction = await _context.BankTransactions.FindAsync(id);
                if (transaction == null)
                    return NotFound();

                // Prevent editing of sales-generated transactions
                if (transaction.Description == "From Sales")
                {
                    ModelState.AddModelError("", "Sales-generated transactions cannot be edited directly. Please edit the corresponding sales record instead.");
                    PopulateDropdowns();
                    await LoadTransactions();
                    return Page();
                }

                Transaction = transaction;

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
                await LoadTransactions();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading transaction: {ex.Message}");
                return RedirectToPage();
            }
        }

        private async Task LoadTransactions()
        {
            try
            {
                var query = _context.BankTransactions.AsQueryable();

                // Apply filters
                if (DateFrom.HasValue)
                    query = query.Where(t => t.TransactionDate.Date >= DateFrom.Value.Date);
                if (DateTo.HasValue)
                    query = query.Where(t => t.TransactionDate.Date <= DateTo.Value.Date);
                if (!string.IsNullOrEmpty(TypeFilter))
                    query = query.Where(t => t.Type == TypeFilter);

                // Special handling for sales transactions filter
                if (!string.IsNullOrEmpty(PayeeFilter))
                {
                    if (PayeeFilter == "SALES_ONLY")
                    {
                        query = query.Where(t => t.Description == "From Sales");
                    }
                    else
                    {
                        query = query.Where(t => t.Payee.Contains(PayeeFilter));
                    }
                }

                if (!string.IsNullOrEmpty(DescriptionFilter))
                    query = query.Where(t => t.Description.Contains(DescriptionFilter));
                if (AmountFrom.HasValue)
                    query = query.Where(t => t.Amount >= AmountFrom.Value);
                if (AmountTo.HasValue)
                    query = query.Where(t => t.Amount <= AmountTo.Value);
                if (CategoryFilter.HasValue && CategoryFilter.Value > 0)
                    query = query.Where(t => t.CompanyCategoryID == CategoryFilter.Value);
                if (!string.IsNullOrEmpty(AccountFilter))
                    query = query.Where(t => t.Account == AccountFilter);
                if (!string.IsNullOrEmpty(DepositTypeFilter))
                    query = query.Where(t => t.DepositType == DepositTypeFilter);

                var results = await query.ToListAsync();

                // Apply sorting
                results = SortBy switch
                {
                    "DateAsc" => results.OrderBy(t => t.TransactionDate).ThenBy(t => t.Id).ToList(),
                    "DateDesc" => results.OrderByDescending(t => t.TransactionDate).ThenByDescending(t => t.Id).ToList(),
                    "AmountAsc" => results.OrderBy(t => t.Amount).ThenBy(t => t.TransactionDate).ToList(),
                    "AmountDesc" => results.OrderByDescending(t => t.Amount).ThenBy(t => t.TransactionDate).ToList(),
                    "AccountAsc" => results.OrderBy(t => t.Account).ThenBy(t => t.TransactionDate).ToList(),
                    "TypeAsc" => results.OrderBy(t => t.Type).ThenBy(t => t.TransactionDate).ToList(),
                    "PayeeAsc" => results.OrderBy(t => t.Payee).ThenBy(t => t.TransactionDate).ToList(),
                    "DepositTypeAsc" => results.OrderBy(t => t.DepositType).ThenBy(t => t.TransactionDate).ToList(),
                    _ => results.OrderByDescending(t => t.TransactionDate).ThenByDescending(t => t.Id).ToList()
                };

                // Load categories for display
                var categoryIds = results.Where(r => r.CompanyCategoryID > 0)
                    .Select(r => r.CompanyCategoryID).Distinct().ToList();

                if (categoryIds.Any())
                {
                    var categories = await _context.CompanyCategories
                        .Where(c => categoryIds.Contains(c.CompanyCategoryID))
                        .ToListAsync();

                    // Set category names
                    foreach (var result in results)
                    {
                        if (result.CompanyCategoryID > 0)
                        {
                            result.CompanyCategory = categories.FirstOrDefault(c => c.CompanyCategoryID == result.CompanyCategoryID);
                        }
                    }
                }

                // Calculate running balances
                var creditTypes = new[] { "Credit", "Deposit" };
                var debitTypes = new[] { "Debit", "Withdrawal" };

                decimal balance = 0;
                if (DateFrom.HasValue)
                {
                    balance = await _context.BankTransactions
                        .Where(t => t.TransactionDate.Date < DateFrom.Value.Date)
                        .SumAsync(t => creditTypes.Contains(t.Type) ? t.Amount :
                                     debitTypes.Contains(t.Type) ? -t.Amount : 0);
                }

                foreach (var record in results)
                {
                    if (creditTypes.Contains(record.Type))
                        balance += record.Amount;
                    else if (debitTypes.Contains(record.Type))
                        balance -= record.Amount;
                    record.Balance = balance;
                }

                Transactions = results;

                System.Diagnostics.Debug.WriteLine($"Loaded {results.Count} transactions, including {results.Count(r => r.Description == "From Sales")} sales transactions");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error loading transactions: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Error loading transactions: {ex.Message}");
                Transactions = new List<BankTransaction>();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove ALL filter-related validation that's not relevant to transaction submission
            var keysToRemove = new[] { "SortBy", "DateFrom", "DateTo", "TypeFilter", "PayeeFilter",
                                     "DescriptionFilter", "AmountFrom", "AmountTo", "CategoryFilter", "DepositTypeFilter", "AccountFilter" };

            foreach (var key in keysToRemove)
            {
                ModelState.Remove(key);
            }

            try
            {
                // Clean ModelState of filter-related validation errors for display
                var filterKeys = new[] { "SortBy", "DateFrom", "DateTo", "TypeFilter", "PayeeFilter",
                                       "DescriptionFilter", "AmountFrom", "AmountTo", "CategoryFilter", "AccountFilter", "DepositTypeFilter" };

                foreach (var key in filterKeys)
                {
                    if (ModelState.ContainsKey(key))
                    {
                        ModelState[key].Errors.Clear();
                        ModelState[key].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                    }
                }

                // Handle sales transaction BEFORE validation
                if (IsSalesTransaction && Transaction.Id == 0)
                {
                    Transaction.Type = "Deposit";
                    Transaction.Description = "From Sales";
                    Transaction.Payee = "Sales";

                    // Validate that sales category/company exist
                    var salesCategory = await _context.CompanyCategories.FindAsync(9);
                    var salesCompany = await _context.Companies.FindAsync(6);

                    if (salesCategory == null)
                    {
                        ModelState.AddModelError("", "Sales category (ID: 9) not found. Please ensure the sales category exists.");
                    }
                    else
                    {
                        Transaction.CompanyCategoryID = 9; // Sales category
                    }

                    if (salesCompany == null)
                    {
                        ModelState.AddModelError("", "Sales company (ID: 6) not found. Please ensure the sales company exists.");
                    }
                    else
                    {
                        Transaction.CompanyID = 6; // Sales company
                    }

                    // IMPORTANT: Check if this is a check transaction
                    if (!string.IsNullOrWhiteSpace(Transaction.CheckNumber) &&
                        !string.IsNullOrWhiteSpace(Transaction.Payer))
                    {
                        // This is a check sales transaction
                        Transaction.DepositType = "Check";
                        Transaction.Payee = $"Check #{Transaction.CheckNumber} - {Transaction.Payer}";
                        System.Diagnostics.Debug.WriteLine($"Sales transaction: Check #{Transaction.CheckNumber} from {Transaction.Payer}");
                    }
                    else
                    {
                        // Default to CC ACH for non-check sales transactions
                        Transaction.DepositType = "CC ACH";
                        Transaction.CheckNumber = null;
                        Transaction.Payer = null;
                        System.Diagnostics.Debug.WriteLine("Sales transaction: CC ACH (no check info)");
                    }

                    // Remove validation for fields that sales transaction auto-fills
                    ModelState.Remove("Transaction.Type");
                    ModelState.Remove("Transaction.Description");
                    ModelState.Remove("Transaction.CompanyID");
                    ModelState.Remove("Transaction.CompanyCategoryID");
                    ModelState.Remove("Transaction.DepositType");
                    // Don't remove check validation - let it validate normally
                }
                else if (!IsSalesTransaction)
                {
                    // For non-sales transactions, set Payee based on selected Company
                    if (Transaction.CompanyID.HasValue)
                    {
                        var company = await _context.Companies.FindAsync(Transaction.CompanyID.Value);
                        if (company != null)
                        {
                            Transaction.Payee = company.CompanyName;
                        }
                    }

                    // Handle check transactions specially
                    if (Transaction.Type == "Deposit" && Transaction.DepositType == "Check")
                    {
                        // For check deposits, validate check fields
                        if (string.IsNullOrWhiteSpace(Transaction.CheckNumber))
                        {
                            ModelState.AddModelError("Transaction.CheckNumber", "Check number is required for check deposits");
                        }
                        if (string.IsNullOrWhiteSpace(Transaction.Payer))
                        {
                            ModelState.AddModelError("Transaction.Payer", "Payer name is required for check deposits");
                        }

                        // Set Payee to include check information for display
                        if (!string.IsNullOrWhiteSpace(Transaction.CheckNumber) && !string.IsNullOrWhiteSpace(Transaction.Payer))
                        {
                            Transaction.Payee = $"Check #{Transaction.CheckNumber} - {Transaction.Payer}";
                        }
                    }
                    else
                    {
                        // Clear check fields for non-check transactions
                        Transaction.CheckNumber = null;
                        Transaction.Payer = null;
                    }
                }

                // Custom validation for transaction fields only
                bool hasValidationErrors = false;

                if (string.IsNullOrEmpty(Transaction.Account))
                {
                    ModelState.AddModelError("Transaction.Account", "Account is required");
                    hasValidationErrors = true;
                }

                if (string.IsNullOrEmpty(Transaction.Type))
                {
                    ModelState.AddModelError("Transaction.Type", "Type is required");
                    hasValidationErrors = true;
                }

                if (string.IsNullOrEmpty(Transaction.Description))
                {
                    ModelState.AddModelError("Transaction.Description", "Description is required");
                    hasValidationErrors = true;
                }

                if (Transaction.Amount <= 0)
                {
                    ModelState.AddModelError("Transaction.Amount", "Amount must be greater than 0");
                    hasValidationErrors = true;
                }

                // Validate DepositType is required when Type is Deposit
                if (Transaction.Type == "Deposit" && string.IsNullOrEmpty(Transaction.DepositType))
                {
                    ModelState.AddModelError("Transaction.DepositType", "Deposit Type is required when Type is Deposit");
                    hasValidationErrors = true;
                }

                if (!IsSalesTransaction && !Transaction.CompanyID.HasValue)
                {
                    ModelState.AddModelError("Transaction.CompanyID", "Please select a company for non-sales transactions");
                    hasValidationErrors = true;
                }

                // Validate foreign key constraints exist
                if (Transaction.CompanyCategoryID.HasValue)
                {
                    var categoryExists = await _context.CompanyCategories.AnyAsync(c => c.CompanyCategoryID == Transaction.CompanyCategoryID.Value);
                    if (!categoryExists)
                    {
                        ModelState.AddModelError("Transaction.CompanyCategoryID", "Selected category does not exist");
                        hasValidationErrors = true;
                    }
                }

                if (Transaction.CompanyID.HasValue)
                {
                    var companyExists = await _context.Companies.AnyAsync(c => c.CompanyID == Transaction.CompanyID.Value);
                    if (!companyExists)
                    {
                        ModelState.AddModelError("Transaction.CompanyID", "Selected company does not exist");
                        hasValidationErrors = true;
                    }
                }

                if (hasValidationErrors)
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
                    await LoadTransactions();
                    return Page();
                }

                if (Transaction.Id > 0)
                {
                    // Update existing transaction
                    var existing = await _context.BankTransactions.FindAsync(Transaction.Id);
                    if (existing != null)
                    {
                        // Prevent updating sales-generated transactions
                        if (existing.Description == "From Sales")
                        {
                            ModelState.AddModelError("", "Sales-generated transactions cannot be edited directly. Please edit the corresponding sales record instead.");
                            PopulateDropdowns();
                            await LoadTransactions();
                            return Page();
                        }

                        existing.Account = Transaction.Account;
                        existing.TransactionDate = Transaction.TransactionDate;
                        existing.Amount = Transaction.Amount;
                        existing.Description = Transaction.Description;
                        existing.Type = Transaction.Type;
                        existing.Payee = Transaction.Payee;
                        existing.CompanyCategoryID = Transaction.CompanyCategoryID;
                        existing.CompanyID = Transaction.CompanyID;
                        existing.DepositType = Transaction.DepositType;
                        existing.CheckNumber = Transaction.CheckNumber;
                        existing.Payer = Transaction.Payer;

                        _context.BankTransactions.Update(existing);
                        System.Diagnostics.Debug.WriteLine($"Updated transaction ID {existing.Id} - Check: {existing.CheckNumber}, Payer: {existing.Payer}");
                    }
                }
                else
                {
                    // Add new transaction
                    var newTransaction = new BankTransaction
                    {
                        Account = Transaction.Account,
                        TransactionDate = Transaction.TransactionDate,
                        Amount = Transaction.Amount,
                        Description = Transaction.Description,
                        Type = Transaction.Type,
                        Payee = Transaction.Payee,
                        CompanyCategoryID = Transaction.CompanyCategoryID,
                        CompanyID = Transaction.CompanyID,
                        DepositType = Transaction.DepositType,
                        CheckNumber = Transaction.CheckNumber,
                        Payer = Transaction.Payer
                    };
                    _context.BankTransactions.Add(newTransaction);
                    System.Diagnostics.Debug.WriteLine($"Added new transaction: {newTransaction.Amount} - {newTransaction.Description} - Check: {newTransaction.CheckNumber}, Payer: {newTransaction.Payer}");
                }

                await _context.SaveChangesAsync();

                // Preserve current filter settings in redirect
                var redirectUrl = $"/Bank?DateFrom={DateFrom:yyyy-MM-dd}&DateTo={DateTo:yyyy-MM-dd}&SortBy={SortBy}";
                if (!string.IsNullOrEmpty(TypeFilter)) redirectUrl += $"&TypeFilter={TypeFilter}";
                if (!string.IsNullOrEmpty(PayeeFilter)) redirectUrl += $"&PayeeFilter={PayeeFilter}";
                if (!string.IsNullOrEmpty(DescriptionFilter)) redirectUrl += $"&DescriptionFilter={DescriptionFilter}";
                if (AmountFrom.HasValue) redirectUrl += $"&AmountFrom={AmountFrom}";
                if (AmountTo.HasValue) redirectUrl += $"&AmountTo={AmountTo}";
                if (CategoryFilter.HasValue) redirectUrl += $"&CategoryFilter={CategoryFilter}";
                if (!string.IsNullOrEmpty(AccountFilter)) redirectUrl += $"&AccountFilter={AccountFilter}";
                if (!string.IsNullOrEmpty(DepositTypeFilter)) redirectUrl += $"&DepositTypeFilter={DepositTypeFilter}";

                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error saving transaction: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Error saving transaction: {ex.Message}");

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
                await LoadTransactions();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var transaction = await _context.BankTransactions.FindAsync(id);
                if (transaction != null)
                {
                    // Prevent deleting sales-generated transactions
                    if (transaction.Description == "From Sales")
                    {
                        ModelState.AddModelError("", "Sales-generated transactions cannot be deleted directly. Please delete the corresponding sales record instead.");
                        PopulateDropdowns();
                        await LoadTransactions();
                        return Page();
                    }

                    _context.BankTransactions.Remove(transaction);
                    await _context.SaveChangesAsync();
                    System.Diagnostics.Debug.WriteLine($"Deleted transaction ID {id}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error deleting transaction: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Error deleting transaction: {ex.Message}");
            }

            return RedirectToPage();
        }
    }
}