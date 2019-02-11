using Microsoft.EntityFrameworkCore.Migrations;

namespace iSHARE.IdentityServer.Data.Migrations.ConfigurationDb
{
    public partial class IncreaseClientSecretColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ClientSecrets",
                maxLength: 3000);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ApiSecrets",
                maxLength: 3000);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ClientSecrets",
                maxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ApiSecrets",
                maxLength: 2000);
        }
    }
}
