// In Package Manager Console, run:
// Add-Migration AddPartsTable -OutputDir Data/Migrations

using Microsoft.EntityFrameworkCore.Migrations;

namespace MLEFIN.Data.Migrations
{
    public partial class AddPartsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create Parts table
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
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.Id);
                });

            // Create unique index on PartNumber for active parts
            migrationBuilder.CreateIndex(
                name: "IX_Parts_PartNumber",
                schema: "joe",
                table: "Parts",
                column: "PartNumber",
                unique: true,
                filter: "[IsActive] = 1");

            // Add check constraints
            migrationBuilder.Sql(@"
                ALTER TABLE [joe].[Parts] ADD CONSTRAINT [CK_Parts_CurrentCost] 
                CHECK ([CurrentCost] IS NULL OR [CurrentCost] >= 0)
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE [joe].[Parts] ADD CONSTRAINT [CK_Parts_AverageCost] 
                CHECK ([AverageCost] IS NULL OR [AverageCost] >= 0)
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE [joe].[Parts] ADD CONSTRAINT [CK_Parts_ListPrice] 
                CHECK ([ListPrice] IS NULL OR [ListPrice] >= 0)
            ");

            // Add optional PartId column to VendorInvoiceItems for future part linking
            migrationBuilder.AddColumn<int>(
                name: "PartId",
                schema: "joe",
                table: "VendorInvoiceItems",
                type: "int",
                nullable: true);

            // Create foreign key relationship (optional)
            migrationBuilder.CreateIndex(
                name: "IX_VendorInvoiceItems_PartId",
                schema: "joe",
                table: "VendorInvoiceItems",
                column: "PartId");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorInvoiceItems_Parts_PartId",
                schema: "joe",
                table: "VendorInvoiceItems",
                column: "PartId",
                principalSchema: "joe",
                principalTable: "Parts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key and column from VendorInvoiceItems
            migrationBuilder.DropForeignKey(
                name: "FK_VendorInvoiceItems_Parts_PartId",
                schema: "joe",
                table: "VendorInvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_VendorInvoiceItems_PartId",
                schema: "joe",
                table: "VendorInvoiceItems");

            migrationBuilder.DropColumn(
                name: "PartId",
                schema: "joe",
                table: "VendorInvoiceItems");

            // Drop Parts table
            migrationBuilder.DropTable(
                name: "Parts",
                schema: "joe");
        }
    }
}