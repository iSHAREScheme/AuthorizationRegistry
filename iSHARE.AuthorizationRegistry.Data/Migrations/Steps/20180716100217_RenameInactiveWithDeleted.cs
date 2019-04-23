using Microsoft.EntityFrameworkCore.Migrations;

namespace iSHARE.AuthorizationRegistry.Data.Migrations.Steps
{
    public partial class RenameInactiveWithDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Inactive",
                table: "Users",
                newName: "Deleted");

            migrationBuilder.RenameColumn(
                name: "Inactive",
                table: "Delegations",
                newName: "Deleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Deleted",
                table: "Users",
                newName: "Inactive");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                table: "Delegations",
                newName: "Inactive");
        }
    }
}
