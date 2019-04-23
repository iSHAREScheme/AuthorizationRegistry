using Microsoft.EntityFrameworkCore.Migrations;

namespace iSHARE.Identity.Data.Migrations.Steps
{
    public partial class ReplaceUsernameWithEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
    UPDATE AspNetUsers
    SET 
    UserName = Email,
    NormalizedUserName = UPPER(Email)
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
