using Microsoft.EntityFrameworkCore.Migrations;

namespace StorageWpfApp.Migrations
{
    public partial class AddedIsFullyPayedPropertyToDebtColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFullyPayed",
                table: "Debts",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFullyPayed",
                table: "Debts");
        }
    }
}
