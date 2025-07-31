using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MLEFIN.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sales",
                schema: "joe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesDate = table.Column<DateTime>(type: "date", nullable: false),
                    Lube = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AutoRepair = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tires = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtherSales = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CarCount = table.Column<int>(type: "int", nullable: false),
                    Bank = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cash = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_SalesDate",
                schema: "joe",
                table: "Sales",
                column: "SalesDate",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sales",
                schema: "joe");
        }
    }
}
