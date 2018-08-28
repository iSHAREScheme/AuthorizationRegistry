using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace NLIP.iShare.AuthorizationRegistry.Data.Migrations
{
    public partial class AddDelegationColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorizationRegistryId",
                table: "Delegations",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Delegations",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Delegations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Policy",
                table: "Delegations",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PolicyIssuer",
                table: "Delegations",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Delegations",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Delegations",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Delegations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorizationRegistryId",
                table: "Delegations");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Delegations");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Delegations");

            migrationBuilder.DropColumn(
                name: "Policy",
                table: "Delegations");

            migrationBuilder.DropColumn(
                name: "PolicyIssuer",
                table: "Delegations");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Delegations");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Delegations");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Delegations");
        }
    }
}
