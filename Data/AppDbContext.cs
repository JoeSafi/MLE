using Microsoft.EntityFrameworkCore;
using MLEFIN.Models;

namespace MLEFIN.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Existing entities
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyCategory> CompanyCategories { get; set; }
        public DbSet<BankTransaction> BankTransactions { get; set; }
        public DbSet<Sales> Sales { get; set; }

        // Vendor Purchase entities
        public DbSet<VendorInvoice> VendorInvoices { get; set; }
        public DbSet<VendorInvoiceItem> VendorInvoiceItems { get; set; }
        public DbSet<VendorWarranty> VendorWarranties { get; set; }
        public DbSet<VendorCore> VendorCores { get; set; }
        public DbSet<VendorPayment> VendorPayments { get; set; }
        public DbSet<VendorInvoicePayment> VendorInvoicePayments { get; set; }
        public DbSet<VendorReturn> VendorReturns { get; set; }
        public DbSet<VendorReturnItem> VendorReturnItems { get; set; }
        public DbSet<Part> Parts { get; set; }

        // Database Views for reporting (add these after migration)
        // public DbSet<VendorInvoiceSummaryView> VendorInvoicesSummary { get; set; }
        // public DbSet<VendorOutstandingBalancesView> VendorOutstandingBalances { get; set; }
        // public DbSet<WarrantyCoreSummaryView> WarrantyCoreSummary { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("joe");

            // Configure Company table
            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("Company");
                entity.HasKey(e => e.CompanyID);
                entity.Property(e => e.CompanyID).ValueGeneratedOnAdd();

                entity.HasOne(c => c.Category)
                    .WithMany()
                    .HasForeignKey(c => c.CompanyCategoryID)
                    .IsRequired();
            });

            // Configure CompanyCategory table
            modelBuilder.Entity<CompanyCategory>(entity =>
            {
                entity.ToTable("CompanyCategory");
                entity.HasKey(e => e.CompanyCategoryID);
            });

            // Configure BankTransaction table to match your existing database
            modelBuilder.Entity<BankTransaction>(entity =>
            {
                entity.ToTable("BankTransactions");
                entity.HasKey(e => e.Id);

                // Configure properties to match your database structure
                entity.Property(b => b.TransactionDate).HasColumnType("date");
                entity.Property(b => b.Type).HasColumnType("nvarchar(max)");
                entity.Property(b => b.Account).HasColumnType("nvarchar(max)");
                entity.Property(b => b.Description).HasColumnType("nvarchar(max)");
                entity.Property(b => b.Amount).HasColumnType("decimal(18,2)");
                entity.Property(b => b.Payee).HasColumnType("nvarchar(max)");
                entity.Property(b => b.CompanyCategoryID).IsRequired(false); // Nullable
                entity.Property(b => b.CompanyID).IsRequired(false); // Nullable
                entity.Property(b => b.DepositType).HasMaxLength(20).IsRequired(false); // Nullable

                // Configure new check-related fields
                entity.Property(b => b.CheckNumber).HasMaxLength(50).IsRequired(false); // Nullable
                entity.Property(b => b.Payer).HasMaxLength(255).IsRequired(false); // Nullable

                // Configure the foreign key relationships
                entity.HasOne(b => b.CompanyCategory)
                    .WithMany(c => c.BankTransactions)
                    .HasForeignKey(b => b.CompanyCategoryID)
                    .OnDelete(DeleteBehavior.SetNull); // Since it's nullable

                entity.HasOne(b => b.Company)
                    .WithMany()
                    .HasForeignKey(b => b.CompanyID)
                    .OnDelete(DeleteBehavior.SetNull); // Since it's nullable

                // Add indexes for better performance
                entity.HasIndex(b => b.DepositType);
                entity.HasIndex(b => b.CheckNumber);
                entity.HasIndex(b => b.Payer);
            });

            // Configure Sales table
            modelBuilder.Entity<Sales>(entity =>
            {
                entity.ToTable("Sales");
                entity.HasKey(e => e.Id);

                // Configure properties
                entity.Property(s => s.SalesDate).HasColumnType("date");
                entity.Property(s => s.Lube).HasColumnType("decimal(18,2)");
                entity.Property(s => s.AutoRepair).HasColumnType("decimal(18,2)");
                entity.Property(s => s.Tires).HasColumnType("decimal(18,2)");
                entity.Property(s => s.OtherSales).HasColumnType("decimal(18,2)");
                entity.Property(s => s.Tax).HasColumnType("decimal(18,2)");
                entity.Property(s => s.Bank).HasColumnType("decimal(18,2)");
                entity.Property(s => s.Cash).HasColumnType("decimal(18,2)");

                // Ensure only one sales record per date
                entity.HasIndex(s => s.SalesDate).IsUnique();
            });

            // === VENDOR ENTITIES CONFIGURATION ===

            // Configure VendorInvoice table
            modelBuilder.Entity<VendorInvoice>(entity =>
            {
                entity.ToTable("VendorInvoices");
                entity.HasKey(e => e.Id);

                // Configure properties
                entity.Property(vi => vi.InvoiceDate).HasColumnType("date");
                entity.Property(vi => vi.DueDate).HasColumnType("date");
                entity.Property(vi => vi.Amount).HasColumnType("decimal(18,2)");
                entity.Property(vi => vi.SubTotal).HasColumnType("decimal(18,2)");
                entity.Property(vi => vi.TaxAmount).HasColumnType("decimal(18,2)");
                entity.Property(vi => vi.ShippingAmount).HasColumnType("decimal(18,2)");
                entity.Property(vi => vi.DiscountAmount).HasColumnType("decimal(18,2)");
                entity.Property(vi => vi.InvoiceNumber).HasMaxLength(50).IsRequired();
                entity.Property(vi => vi.Status).HasMaxLength(20).IsRequired();
                entity.Property(vi => vi.PONumber).HasMaxLength(50);
                entity.Property(vi => vi.Notes).HasMaxLength(500);
                entity.Property(vi => vi.Reference).HasMaxLength(100);

                // Configure foreign key relationship
                entity.HasOne(vi => vi.VendorCompany)
                    .WithMany()
                    .HasForeignKey(vi => vi.VendorCompanyID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Create unique constraint on invoice number per vendor
                entity.HasIndex(vi => new { vi.VendorCompanyID, vi.InvoiceNumber }).IsUnique();
            });

            // Configure VendorInvoiceItem table
            modelBuilder.Entity<VendorInvoiceItem>(entity =>
            {
                entity.ToTable("VendorInvoiceItems");
                entity.HasKey(e => e.Id);

                // Configure properties
                entity.Property(vii => vii.PartNumber).HasMaxLength(50).IsRequired();
                entity.Property(vii => vii.Description).HasMaxLength(255).IsRequired();
                entity.Property(vii => vii.Cost).HasColumnType("decimal(18,2)");

                // Configure foreign key relationship
                entity.HasOne(vii => vii.VendorInvoice)
                    .WithMany()
                    .HasForeignKey(vii => vii.VendorInvoiceID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure VendorWarranty table
            modelBuilder.Entity<VendorWarranty>(entity =>
            {
                entity.ToTable("VendorWarranties");
                entity.HasKey(e => e.Id);

                // Configure properties
                entity.Property(vw => vw.WarrantyDate).HasColumnType("date");
                entity.Property(vw => vw.ResolutionDate).HasColumnType("date");
                entity.Property(vw => vw.Amount).HasColumnType("decimal(18,2)");
                entity.Property(vw => vw.CreditAmount).HasColumnType("decimal(18,2)");
                entity.Property(vw => vw.Status).HasMaxLength(20).IsRequired();
                entity.Property(vw => vw.Notes).HasMaxLength(500);
                entity.Property(vw => vw.WarrantyClaimNumber).HasMaxLength(100);

                // Configure foreign key relationships
                entity.HasOne(vw => vw.VendorInvoice)
                    .WithMany()
                    .HasForeignKey(vw => vw.VendorInvoiceID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(vw => vw.VendorCompany)
                    .WithMany()
                    .HasForeignKey(vw => vw.VendorCompanyID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure VendorCore table
            modelBuilder.Entity<VendorCore>(entity =>
            {
                entity.ToTable("VendorCores");
                entity.HasKey(e => e.Id);

                // Configure properties
                entity.Property(vc => vc.CoreDate).HasColumnType("date");
                entity.Property(vc => vc.ShippedDate).HasColumnType("date");
                entity.Property(vc => vc.CreditedDate).HasColumnType("date");
                entity.Property(vc => vc.CoreAmount).HasColumnType("decimal(18,2)");
                entity.Property(vc => vc.CreditAmount).HasColumnType("decimal(18,2)");
                entity.Property(vc => vc.Status).HasMaxLength(20).IsRequired();
                entity.Property(vc => vc.TrackingNumber).HasMaxLength(100);
                entity.Property(vc => vc.CoreNumber).HasMaxLength(100);
                entity.Property(vc => vc.Notes).HasMaxLength(500);

                // Configure foreign key relationships
                entity.HasOne(vc => vc.VendorInvoice)
                    .WithMany()
                    .HasForeignKey(vc => vc.VendorInvoiceID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(vc => vc.VendorCompany)
                    .WithMany()
                    .HasForeignKey(vc => vc.VendorCompanyID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure VendorPayment table
            modelBuilder.Entity<VendorPayment>(entity =>
            {
                entity.ToTable("VendorPayments");
                entity.HasKey(e => e.Id);

                // Configure properties
                entity.Property(vp => vp.PaymentDate).HasColumnType("date");
                entity.Property(vp => vp.Amount).HasColumnType("decimal(18,2)");
                entity.Property(vp => vp.PaymentMethod).HasMaxLength(20).IsRequired();
                entity.Property(vp => vp.CheckNumber).HasMaxLength(50);
                entity.Property(vp => vp.ReferenceNumber).HasMaxLength(50);
                entity.Property(vp => vp.Notes).HasMaxLength(500);

                // Configure foreign key relationships
                entity.HasOne(vp => vp.VendorCompany)
                    .WithMany()
                    .HasForeignKey(vp => vp.VendorCompanyID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(vp => vp.BankTransaction)
                    .WithMany()
                    .HasForeignKey(vp => vp.BankTransactionID)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure VendorInvoicePayment table (junction table)
            modelBuilder.Entity<VendorInvoicePayment>(entity =>
            {
                entity.ToTable("VendorInvoicePayments");
                entity.HasKey(e => e.Id);

                // Configure properties
                entity.Property(vip => vip.AllocationAmount).HasColumnType("decimal(18,2)");

                // Configure foreign key relationships
                entity.HasOne(vip => vip.VendorInvoice)
                    .WithMany()
                    .HasForeignKey(vip => vip.VendorInvoiceID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(vip => vip.VendorPayment)
                    .WithMany()
                    .HasForeignKey(vip => vip.VendorPaymentID)
                    .OnDelete(DeleteBehavior.Cascade);

                // Create unique constraint to prevent duplicate allocations
                entity.HasIndex(vip => new { vip.VendorInvoiceID, vip.VendorPaymentID }).IsUnique();
            });

            // Configure VendorReturn table
            modelBuilder.Entity<VendorReturn>(entity =>
            {
                entity.ToTable("VendorReturns");
                entity.HasKey(e => e.Id);

                // Configure properties
                entity.Property(vr => vr.ReturnDate).HasColumnType("date");
                entity.Property(vr => vr.ProcessedDate).HasColumnType("date");
                entity.Property(vr => vr.Amount).HasColumnType("decimal(18,2)");
                entity.Property(vr => vr.SubTotal).HasColumnType("decimal(18,2)");
                entity.Property(vr => vr.TaxAmount).HasColumnType("decimal(18,2)");
                entity.Property(vr => vr.ShippingAmount).HasColumnType("decimal(18,2)");
                entity.Property(vr => vr.CreditAmount).HasColumnType("decimal(18,2)");
                entity.Property(vr => vr.ReturnNumber).HasMaxLength(50).IsRequired();
                entity.Property(vr => vr.Status).HasMaxLength(20).IsRequired();
                entity.Property(vr => vr.RMANumber).HasMaxLength(100);
                entity.Property(vr => vr.Notes).HasMaxLength(500);
                entity.Property(vr => vr.Reason).HasMaxLength(500);

                // Configure foreign key relationship
                entity.HasOne(vr => vr.VendorCompany)
                    .WithMany()
                    .HasForeignKey(vr => vr.VendorCompanyID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Create unique constraint on return number per vendor
                entity.HasIndex(vr => new { vr.VendorCompanyID, vr.ReturnNumber }).IsUnique();
            });

            // Configure VendorReturnItem table
            modelBuilder.Entity<VendorReturnItem>(entity =>
            {
                entity.ToTable("VendorReturnItems");
                entity.HasKey(e => e.Id);

                // Configure properties
                entity.Property(vri => vri.PartNumber).HasMaxLength(50).IsRequired();
                entity.Property(vri => vri.Description).HasMaxLength(255).IsRequired();
                entity.Property(vri => vri.Cost).HasColumnType("decimal(18,2)");
                entity.Property(vri => vri.Reason).HasMaxLength(500);

                // Configure foreign key relationship
                entity.HasOne(vri => vri.VendorReturn)
                    .WithMany()
                    .HasForeignKey(vri => vri.VendorReturnID)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Part>(entity =>
            {
                entity.ToTable("Parts");
                entity.HasKey(e => e.Id);

                // Configure properties
                entity.Property(p => p.PartNumber).HasMaxLength(50).IsRequired();
                entity.Property(p => p.Description).HasMaxLength(255).IsRequired();
                entity.Property(p => p.CurrentCost).HasColumnType("decimal(18,2)");
                entity.Property(p => p.AverageCost).HasColumnType("decimal(18,2)");
                entity.Property(p => p.ListPrice).HasColumnType("decimal(18,2)");
                entity.Property(p => p.Category).HasMaxLength(100);
                entity.Property(p => p.IsActive).IsRequired();
                entity.Property(p => p.Notes).HasMaxLength(500);
                entity.Property(p => p.CreatedDate).IsRequired();

                // Create unique constraint on active part numbers
                entity.HasIndex(p => p.PartNumber)
                    .IsUnique()
                    .HasFilter("[IsActive] = 1");
            });
        }
    }
}