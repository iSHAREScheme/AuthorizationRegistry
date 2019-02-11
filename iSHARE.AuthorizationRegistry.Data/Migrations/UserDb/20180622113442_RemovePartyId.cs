using Microsoft.EntityFrameworkCore.Migrations;

namespace iSHARE.AuthorizationRegistry.Data.Migrations.UserDb
{
    public partial class RemovePartyId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartyId",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PartyId",
                table: "AspNetUsers",
                nullable: true);
        }
    }
}
