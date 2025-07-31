// First, let's create a migration to add performance indexes and any missing constraints

// In Package Manager Console, run:
// Add-Migration VendorPurchaseIndexes -OutputDir Data/Migrations

// Migration file content:
using Microsoft.EntityFrameworkCore.Migrations;

namespace MLEFIN.Data.Migrations
{
    public partial class VendorPurchaseIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add performance indexes for Vendor Invoice queries
            migrationBuilder.CreateIndex(
                name: "IX_VendorInvoices_InvoiceDate",
                schema: "joe",
                table: "VendorInvoices",
                column: "InvoiceDate");

            migrationBuilder.CreateIndex(
                name: "IX_VendorInvoices_DueDate",
                schema: "joe",
                table: "VendorInvoices",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_VendorInvoices_Status",
                schema: "joe",
                table: "VendorInvoices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VendorInvoices_VendorCompanyID_InvoiceDate",
                schema: "joe",
                table: "VendorInvoices",
                columns: new[] { "VendorCompanyID", "InvoiceDate" });

            // Add performance indexes for Vendor Payment queries
            migrationBuilder.CreateIndex(
                name: "IX_VendorPayments_PaymentDate",
                schema: "joe",
                table: "VendorPayments",
                column: "PaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayments_VendorCompanyID_PaymentDate",
                schema: "joe",
                table: "VendorPayments",
                columns: new[] { "VendorCompanyID", "PaymentDate" });

            // Add performance indexes for Warranty tracking
            migrationBuilder.CreateIndex(
                name: "IX_VendorWarranties_WarrantyDate",
                schema: "joe",
                table: "VendorWarranties",
                column: "WarrantyDate");

            migrationBuilder.CreateIndex(
                name: "IX_VendorWarranties_Status",
                schema: "joe",
                table: "VendorWarranties",
                column: "Status");

            // Add performance indexes for Core tracking
            migrationBuilder.CreateIndex(
                name: "IX_VendorCores_CoreDate",
                schema: "joe",
                table: "VendorCores",
                column: "CoreDate");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCores_Status",
                schema: "joe",
                table: "VendorCores",
                column: "Status");

            // Add performance indexes for Returns
            migrationBuilder.CreateIndex(
                name: "IX_VendorReturns_ReturnDate",
                schema: "joe",
                table: "VendorReturns",
                column: "ReturnDate");

            migrationBuilder.CreateIndex(
                name: "IX_VendorReturns_VendorCompanyID_ReturnDate",
                schema: "joe",
                table: "VendorReturns",
                columns: new[] { "VendorCompanyID", "ReturnDate" });

            // Create the enhanced vendor summary view (replaces the existing one)
            migrationBuilder.Sql(@"
                -- Drop existing view if it exists
                IF OBJECT_ID('[joe].[VW_VendorInvoicesSummary]', 'V') IS NOT NULL
                    DROP VIEW [joe].[VW_VendorInvoicesSummary];
            ");

            migrationBuilder.Sql(@"
                -- Recreate the vendor invoices summary view with enhanced functionality
                CREATE VIEW [joe].[VW_VendorInvoicesSummary]
                AS
                SELECT 
                    vi.[Id],
                    vi.[InvoiceNumber],
                    c.[CompanyName] AS [VendorName],
                    vi.[InvoiceDate],
                    vi.[DueDate],
                    vi.[Amount],
                    vi.[Status],
                    vi.[HasWarranty],
                    vi.[HasCore],
                    vi.[HasParts],
                    vi.[PONumber],
                    vi.[Notes],
                    COALESCE(payments.[AmountPaid], 0) AS [AmountPaid],
                    (vi.[Amount] - COALESCE(payments.[AmountPaid], 0)) AS [AmountDue],
                    CASE 
                        WHEN vi.[Status] != 'Paid' AND vi.[DueDate] < CAST(GETDATE() AS DATE) THEN 1
                        ELSE 0
                    END AS [IsOverdue],
                    CASE 
                        WHEN vi.[Status] != 'Paid' AND vi.[DueDate] < CAST(GETDATE() AS DATE) 
                        THEN DATEDIFF(day, vi.[DueDate], GETDATE())
                        ELSE 0
                    END AS [DaysOverdue]
                FROM [joe].[VendorInvoices] vi
                INNER JOIN [joe].[Company] c ON vi.[VendorCompanyID] = c.[CompanyID]
                LEFT JOIN (
                    SELECT 
                        vip.[VendorInvoiceID],
                        SUM(vip.[AllocationAmount]) AS [AmountPaid]
                    FROM [joe].[VendorInvoicePayments] vip
                    GROUP BY vip.[VendorInvoiceID]
                ) payments ON vi.[Id] = payments.[VendorInvoiceID];
            ");

            // Create vendor outstanding balances view
            migrationBuilder.Sql(@"
                CREATE VIEW [joe].[VW_VendorOutstandingBalances] AS
                SELECT 
                    c.CompanyName AS VendorName,
                    c.CompanyID,
                    COUNT(vi.Id) AS OutstandingInvoices,
                    SUM(vi.Amount) AS TotalOutstanding,
                    SUM(COALESCE(payments.AmountPaid, 0)) AS TotalPaid,
                    SUM(vi.Amount - COALESCE(payments.AmountPaid, 0)) AS NetOutstanding,
                    MIN(vi.DueDate) AS OldestDueDate,
                    SUM(CASE WHEN vi.DueDate < CAST(GETDATE() AS DATE) AND vi.Status != 'Paid' THEN 1 ELSE 0 END) AS OverdueCount
                FROM [joe].[VendorInvoices] vi
                INNER JOIN [joe].[Company] c ON vi.VendorCompanyID = c.CompanyID
                LEFT JOIN (
                    SELECT VendorInvoiceID, SUM(AllocationAmount) AS AmountPaid
                    FROM [joe].[VendorInvoicePayments]
                    GROUP BY VendorInvoiceID
                ) payments ON vi.Id = payments.VendorInvoiceID
                WHERE vi.Status != 'Paid'
                GROUP BY c.CompanyName, c.CompanyID;
            ");

            // Create warranty and core summary view (separate SQL call)
            migrationBuilder.Sql(@"
                CREATE VIEW [joe].[VW_WarrantyCoreSummary] AS
                SELECT 
                    vi.Id AS InvoiceId,
                    vi.InvoiceNumber,
                    c.CompanyName AS VendorName,
                    vi.Amount AS InvoiceAmount,
                    vi.InvoiceDate,
                    vi.DueDate,
                    COALESCE(w.WarrantyAmount, 0) AS WarrantyAmount,
                    COALESCE(cor.CoreAmount, 0) AS CoreAmount,
                    w.PendingWarranties,
                    cor.PendingCores,
                    w.CreditedWarranties,
                    cor.CreditedCores
                FROM [joe].[VendorInvoices] vi
                INNER JOIN [joe].[Company] c ON vi.VendorCompanyID = c.CompanyID
                LEFT JOIN (
                    SELECT 
                        VendorInvoiceID,
                        SUM(Amount) AS WarrantyAmount,
                        COUNT(CASE WHEN Status = 'Pending' THEN 1 END) AS PendingWarranties,
                        COUNT(CASE WHEN Status = 'Credited' THEN 1 END) AS CreditedWarranties
                    FROM [joe].[VendorWarranties]
                    GROUP BY VendorInvoiceID
                ) w ON vi.Id = w.VendorInvoiceID
                LEFT JOIN (
                    SELECT 
                        VendorInvoiceID,
                        SUM(CoreAmount) AS CoreAmount,
                        COUNT(CASE WHEN Status = 'Pending' THEN 1 END) AS PendingCores,
                        COUNT(CASE WHEN Status = 'Credited' THEN 1 END) AS CreditedCores
                    FROM [joe].[VendorCores]
                    GROUP BY VendorInvoiceID
                ) cor ON vi.Id = cor.VendorInvoiceID
                WHERE vi.HasWarranty = 1 OR vi.HasCore = 1;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop views first
            migrationBuilder.Sql("DROP VIEW IF EXISTS [joe].[VW_WarrantyCoreSummary];");
            migrationBuilder.Sql("DROP VIEW IF EXISTS [joe].[VW_VendorOutstandingBalances];");

            // Restore original VW_VendorInvoicesSummary view
            migrationBuilder.Sql(@"
                CREATE VIEW [joe].[VW_VendorInvoicesSummary]
                AS
                SELECT 
                    vi.[Id],
                    vi.[InvoiceNumber],
                    c.[CompanyName] AS [VendorName],
                    vi.[InvoiceDate],
                    vi.[DueDate],
                    vi.[Amount],
                    vi.[Status],
                    vi.[HasWarranty],
                    vi.[HasCore],
                    vi.[HasParts],
                    COALESCE(payments.[AmountPaid], 0) AS [AmountPaid],
                    (vi.[Amount] - COALESCE(payments.[AmountPaid], 0)) AS [AmountDue],
                    CASE 
                        WHEN vi.[Status] != 'Paid' AND vi.[DueDate] < CAST(GETDATE() AS DATE) THEN 1
                        ELSE 0
                    END AS [IsOverdue],
                    CASE 
                        WHEN vi.[Status] != 'Paid' AND vi.[DueDate] < CAST(GETDATE() AS DATE) 
                        THEN DATEDIFF(day, vi.[DueDate], GETDATE())
                        ELSE 0
                    END AS [DaysOverdue]
                FROM [joe].[VendorInvoices] vi
                INNER JOIN [joe].[Company] c ON vi.[VendorCompanyID] = c.[CompanyID]
                LEFT JOIN (
                    SELECT 
                        vip.[VendorInvoiceID],
                        SUM(vip.[AllocationAmount]) AS [AmountPaid]
                    FROM [joe].[VendorInvoicePayments] vip
                    GROUP BY vip.[VendorInvoiceID]
                ) payments ON vi.[Id] = payments.[VendorInvoiceID];
            ");

            // Drop indexes
            migrationBuilder.DropIndex(name: "IX_VendorReturns_VendorCompanyID_ReturnDate", schema: "joe", table: "VendorReturns");
            migrationBuilder.DropIndex(name: "IX_VendorReturns_ReturnDate", schema: "joe", table: "VendorReturns");
            migrationBuilder.DropIndex(name: "IX_VendorCores_Status", schema: "joe", table: "VendorCores");
            migrationBuilder.DropIndex(name: "IX_VendorCores_CoreDate", schema: "joe", table: "VendorCores");
            migrationBuilder.DropIndex(name: "IX_VendorWarranties_Status", schema: "joe", table: "VendorWarranties");
            migrationBuilder.DropIndex(name: "IX_VendorWarranties_WarrantyDate", schema: "joe", table: "VendorWarranties");
            migrationBuilder.DropIndex(name: "IX_VendorPayments_VendorCompanyID_PaymentDate", schema: "joe", table: "VendorPayments");
            migrationBuilder.DropIndex(name: "IX_VendorPayments_PaymentDate", schema: "joe", table: "VendorPayments");
            migrationBuilder.DropIndex(name: "IX_VendorInvoices_VendorCompanyID_InvoiceDate", schema: "joe", table: "VendorInvoices");
            migrationBuilder.DropIndex(name: "IX_VendorInvoices_Status", schema: "joe", table: "VendorInvoices");
            migrationBuilder.DropIndex(name: "IX_VendorInvoices_DueDate", schema: "joe", table: "VendorInvoices");
            migrationBuilder.DropIndex(name: "IX_VendorInvoices_InvoiceDate", schema: "joe", table: "VendorInvoices");
        }
    }
}