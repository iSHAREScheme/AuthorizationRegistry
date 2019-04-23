using Microsoft.EntityFrameworkCore.Migrations;

namespace iSHARE.AuthorizationRegistry.Data.Migrations.Steps
{
    public partial class AspNetUsersMigrationsDelta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
if not exists (select null from __EFMigrationsHistory 
		where MigrationId = '20180705133538_InitialAspIdentityConfigurationDbMigration')

	insert into __EFMigrationsHistory values('20180705133538_InitialAspIdentityConfigurationDbMigration', '2.1.8-servicing-32085')

if not exists (select null from __EFMigrationsHistory 
		where MigrationId = '20190315134340_AspIdentityDeleted')

	insert into __EFMigrationsHistory values('20190315134340_AspIdentityDeleted', '2.1.8-servicing-32085')
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "delete __EFMigrationsHistory where MigrationId = '20180705133538_InitialAspIdentityConfigurationDbMigration'");

            migrationBuilder.Sql(
                "delete __EFMigrationsHistory where MigrationId = '20190315134340_AspIdentityDeleted'");
        }
    }
}
