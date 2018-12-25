using Microsoft.EntityFrameworkCore.Migrations;

namespace StorageWpfApp.Migrations
{
    public partial class AddedAmountColumnToDebtPayments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "DebtPayments",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "DebtPayments");
        }
    }
}
