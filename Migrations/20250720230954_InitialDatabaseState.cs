using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MLEFIN.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabaseState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "joe");

            migrationBuilder.CreateTable(
                name: "CompanyCategory",
                schema: "joe",
                columns: table => new
                {
                    CompanyCategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyCategory", x => x.CompanyCategoryID);
                });

            migrationBuilder.CreateTable(
                name: "BankTransactions",
                schema: "joe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "date", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Payee = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyCategoryID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankTransactions_CompanyCategory_CompanyCategoryID",
                        column: x => x.CompanyCategoryID,
                        principalSchema: "joe",
                        principalTable: "CompanyCategory",
                        principalColumn: "CompanyCategoryID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                schema: "joe",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CellPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentTerms = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    CardOnFile = table.Column<bool>(type: "bit", nullable: false),
                    CardNumber = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Check = table.Column<bool>(type: "bit", nullable: false),
                    Cash = table.Column<bool>(type: "bit", nullable: false),
                    CompanyCategoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.CompanyID);
                    table.ForeignKey(
                        name: "FK_Company_CompanyCategory_CompanyCategoryID",
                        column: x => x.CompanyCategoryID,
                        principalSchema: "joe",
                        principalTable: "CompanyCategory",
                        principalColumn: "CompanyCategoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_CompanyCategoryID",
                schema: "joe",
                table: "BankTransactions",
                column: "CompanyCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Company_CompanyCategoryID",
                schema: "joe",
                table: "Company",
                column: "CompanyCategoryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankTransactions",
                schema: "joe");

            migrationBuilder.DropTable(
                name: "Company",
                schema: "joe");

            migrationBuilder.DropTable(
                name: "CompanyCategory",
                schema: "joe");
        }
    }
}
