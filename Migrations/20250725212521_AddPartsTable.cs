using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MLEFIN.Migrations
{
    /// <inheritdoc />
    public partial class AddPartsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CheckNumber",
                schema: "joe",
                table: "BankTransactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Payer",
                schema: "joe",
                table: "BankTransactions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Parts",
                schema: "joe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CurrentCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AverageCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ListPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VendorInvoices",
                schema: "joe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VendorCompanyID = table.Column<int>(type: "int", nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "date", nullable: false),
                    DueDate = table.Column<DateTime>(type: "date", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PONumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HasWarranty = table.Column<bool>(type: "bit", nullable: false),
                    HasCore = table.Column<bool>(type: "bit", nullable: false),
                    HasParts = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorInvoices_Company_VendorCompanyID",
                        column: x => x.VendorCompanyID,
                        principalSchema: "joe",
                        principalTable: "Company",
                        principalColumn: "CompanyID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorPayments",
                schema: "joe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorCompanyID = table.Column<int>(type: "int", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "date", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CheckNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BankTransactionID = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorPayments_BankTransactions_BankTransactionID",
                        column: x => x.BankTransactionID,
                        principalSchema: "joe",
                        principalTable: "BankTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VendorPayments_Company_VendorCompanyID",
                        column: x => x.VendorCompanyID,
                        principalSchema: "joe",
                        principalTable: "Company",
                        principalColumn: "CompanyID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorReturns",
                schema: "joe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VendorCompanyID = table.Column<int>(type: "int", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "date", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HasParts = table.Column<bool>(type: "bit", nullable: false),
                    RMANumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProcessedDate = table.Column<DateTime>(type: "date", nullable: true),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorReturns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorReturns_Company_VendorCompanyID",
                        column: x => x.VendorCompanyID,
                        principalSchema: "joe",
                        principalTable: "Company",
                        principalColumn: "CompanyID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorCores",
                schema: "joe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorInvoiceID = table.Column<int>(type: "int", nullable: false),
                    VendorCompanyID = table.Column<int>(type: "int", nullable: false),
                    CoreDate = table.Column<DateTime>(type: "date", nullable: false),
                    CoreAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ShippedDate = table.Column<DateTime>(type: "date", nullable: true),
                    CreditedDate = table.Column<DateTime>(type: "date", nullable: true),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TrackingNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CoreNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorCores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorCores_Company_VendorCompanyID",
                        column: x => x.VendorCompanyID,
                        principalSchema: "joe",
                        principalTable: "Company",
                        principalColumn: "CompanyID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorCores_VendorInvoices_VendorInvoiceID",
                        column: x => x.VendorInvoiceID,
                        principalSchema: "joe",
                        principalTable: "VendorInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorInvoiceItems",
                schema: "joe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorInvoiceID = table.Column<int>(type: "int", nullable: false),
                    PartNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorInvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorInvoiceItems_VendorInvoices_VendorInvoiceID",
                        column: x => x.VendorInvoiceID,
                        principalSchema: "joe",
                        principalTable: "VendorInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorWarranties",
                schema: "joe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorInvoiceID = table.Column<int>(type: "int", nullable: false),
                    VendorCompanyID = table.Column<int>(type: "int", nullable: false),
                    WarrantyDate = table.Column<DateTime>(type: "date", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ResolutionDate = table.Column<DateTime>(type: "date", nullable: true),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WarrantyClaimNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorWarranties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorWarranties_Company_VendorCompanyID",
                        column: x => x.VendorCompanyID,
                        principalSchema: "joe",
                        principalTable: "Company",
                        principalColumn: "CompanyID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorWarranties_VendorInvoices_VendorInvoiceID",
                        column: x => x.VendorInvoiceID,
                        principalSchema: "joe",
                        principalTable: "VendorInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorInvoicePayments",
                schema: "joe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorInvoiceID = table.Column<int>(type: "int", nullable: false),
                    VendorPaymentID = table.Column<int>(type: "int", nullable: false),
                    AllocationAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorInvoicePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorInvoicePayments_VendorInvoices_VendorInvoiceID",
                        column: x => x.VendorInvoiceID,
                        principalSchema: "joe",
                        principalTable: "VendorInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VendorInvoicePayments_VendorPayments_VendorPaymentID",
                        column: x => x.VendorPaymentID,
                        principalSchema: "joe",
                        principalTable: "VendorPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorReturnItems",
                schema: "joe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorReturnID = table.Column<int>(type: "int", nullable: false),
                    PartNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorReturnItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorReturnItems_VendorReturns_VendorReturnID",
                        column: x => x.VendorReturnID,
                        principalSchema: "joe",
                        principalTable: "VendorReturns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_CheckNumber",
                schema: "joe",
                table: "BankTransactions",
                column: "CheckNumber");

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_Payer",
                schema: "joe",
                table: "BankTransactions",
                column: "Payer");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_PartNumber",
                schema: "joe",
                table: "Parts",
                column: "PartNumber",
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCores_VendorCompanyID",
                schema: "joe",
                table: "VendorCores",
                column: "VendorCompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCores_VendorInvoiceID",
                schema: "joe",
                table: "VendorCores",
                column: "VendorInvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_VendorInvoiceItems_VendorInvoiceID",
                schema: "joe",
                table: "VendorInvoiceItems",
                column: "VendorInvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_VendorInvoicePayments_VendorInvoiceID_VendorPaymentID",
                schema: "joe",
                table: "VendorInvoicePayments",
                columns: new[] { "VendorInvoiceID", "VendorPaymentID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorInvoicePayments_VendorPaymentID",
                schema: "joe",
                table: "VendorInvoicePayments",
                column: "VendorPaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_VendorInvoices_VendorCompanyID_InvoiceNumber",
                schema: "joe",
                table: "VendorInvoices",
                columns: new[] { "VendorCompanyID", "InvoiceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayments_BankTransactionID",
                schema: "joe",
                table: "VendorPayments",
                column: "BankTransactionID");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayments_VendorCompanyID",
                schema: "joe",
                table: "VendorPayments",
                column: "VendorCompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_VendorReturnItems_VendorReturnID",
                schema: "joe",
                table: "VendorReturnItems",
                column: "VendorReturnID");

            migrationBuilder.CreateIndex(
                name: "IX_VendorReturns_VendorCompanyID_ReturnNumber",
                schema: "joe",
                table: "VendorReturns",
                columns: new[] { "VendorCompanyID", "ReturnNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorWarranties_VendorCompanyID",
                schema: "joe",
                table: "VendorWarranties",
                column: "VendorCompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_VendorWarranties_VendorInvoiceID",
                schema: "joe",
                table: "VendorWarranties",
                column: "VendorInvoiceID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Parts",
                schema: "joe");

            migrationBuilder.DropTable(
                name: "VendorCores",
                schema: "joe");

            migrationBuilder.DropTable(
                name: "VendorInvoiceItems",
                schema: "joe");

            migrationBuilder.DropTable(
                name: "VendorInvoicePayments",
                schema: "joe");

            migrationBuilder.DropTable(
                name: "VendorReturnItems",
                schema: "joe");

            migrationBuilder.DropTable(
                name: "VendorWarranties",
                schema: "joe");

            migrationBuilder.DropTable(
                name: "VendorPayments",
                schema: "joe");

            migrationBuilder.DropTable(
                name: "VendorReturns",
                schema: "joe");

            migrationBuilder.DropTable(
                name: "VendorInvoices",
                schema: "joe");

            migrationBuilder.DropIndex(
                name: "IX_BankTransactions_CheckNumber",
                schema: "joe",
                table: "BankTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BankTransactions_Payer",
                schema: "joe",
                table: "BankTransactions");

            migrationBuilder.DropColumn(
                name: "CheckNumber",
                schema: "joe",
                table: "BankTransactions");

            migrationBuilder.DropColumn(
                name: "Payer",
                schema: "joe",
                table: "BankTransactions");
        }
    }
}
