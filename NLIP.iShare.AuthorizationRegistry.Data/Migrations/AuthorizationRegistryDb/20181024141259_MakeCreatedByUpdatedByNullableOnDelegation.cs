using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NLIP.iShare.AuthorizationRegistry.Data.Migrations.AuthorizationRegistryDb
{
    public partial class MakeCreatedByUpdatedByNullableOnDelegation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DelegationsHistories_Users_CreatedById",
                table: "DelegationsHistories");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "DelegationsHistories",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "Delegations",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "Delegations",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_DelegationsHistories_Users_CreatedById",
                table: "DelegationsHistories",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DelegationsHistories_Users_CreatedById",
                table: "DelegationsHistories");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "DelegationsHistories",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedById",
                table: "Delegations",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "Delegations",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DelegationsHistories_Users_CreatedById",
                table: "DelegationsHistories",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
