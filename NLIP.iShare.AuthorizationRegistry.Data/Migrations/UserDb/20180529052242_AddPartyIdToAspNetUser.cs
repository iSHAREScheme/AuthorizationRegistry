using Microsoft.EntityFrameworkCore.Migrations;

namespace NLIP.iShare.AuthorizationRegistry.Data.Migrations.UserDb
{
    public partial class AddPartyIdToAspNetUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PartyId",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartyId",
                table: "AspNetUsers");
        }
    }
}
