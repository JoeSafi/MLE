using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MLEFIN.Migrations
{
    /// <inheritdoc />
    public partial class AddDepositTypeToBankTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepositType",
                schema: "joe",
                table: "BankTransactions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_DepositType",
                schema: "joe",
                table: "BankTransactions",
                column: "DepositType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BankTransactions_DepositType",
                schema: "joe",
                table: "BankTransactions");

            migrationBuilder.DropColumn(
                name: "DepositType",
                schema: "joe",
                table: "BankTransactions");
        }
    }
}
