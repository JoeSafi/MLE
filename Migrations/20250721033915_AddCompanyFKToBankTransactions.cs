using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MLEFIN.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyFKToBankTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                schema: "joe",
                table: "BankTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_CompanyID",
                schema: "joe",
                table: "BankTransactions",
                column: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_BankTransactions_Company_CompanyID",
                schema: "joe",
                table: "BankTransactions",
                column: "CompanyID",
                principalSchema: "joe",
                principalTable: "Company",
                principalColumn: "CompanyID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankTransactions_Company_CompanyID",
                schema: "joe",
                table: "BankTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BankTransactions_CompanyID",
                schema: "joe",
                table: "BankTransactions");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                schema: "joe",
                table: "BankTransactions");
        }
    }
}
