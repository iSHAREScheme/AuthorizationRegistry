using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NLIP.iShare.AuthorizationRegistry.Data.Migrations
{
    public partial class AddUserAndDelegationHistoryAndUpdateModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Delegations");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Delegations");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "Delegations",
                newName: "AccessSubject");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Delegations",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "Inactive",
                table: "Delegations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedById",
                table: "Delegations",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    AspNetUserId = table.Column<string>(nullable: true),
                    PartyId = table.Column<string>(nullable: true),
                    PartyName = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Inactive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DelegationsHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DelegationId = table.Column<Guid>(nullable: false),
                    Policy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DelegationsHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DelegationsHistories_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DelegationsHistories_Delegations_DelegationId",
                        column: x => x.DelegationId,
                        principalTable: "Delegations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Delegations_CreatedById",
                table: "Delegations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Delegations_UpdatedById",
                table: "Delegations",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DelegationsHistories_CreatedById",
                table: "DelegationsHistories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DelegationsHistories_DelegationId",
                table: "DelegationsHistories",
                column: "DelegationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Delegations_Users_CreatedById",
                table: "Delegations",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Delegations_Users_UpdatedById",
                table: "Delegations",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Delegations_Users_CreatedById",
                table: "Delegations");

            migrationBuilder.DropForeignKey(
                name: "FK_Delegations_Users_UpdatedById",
                table: "Delegations");

            migrationBuilder.DropTable(
                name: "DelegationsHistories");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Delegations_CreatedById",
                table: "Delegations");

            migrationBuilder.DropIndex(
                name: "IX_Delegations_UpdatedById",
                table: "Delegations");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Delegations");

            migrationBuilder.DropColumn(
                name: "Inactive",
                table: "Delegations");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Delegations");

            migrationBuilder.RenameColumn(
                name: "AccessSubject",
                table: "Delegations",
                newName: "UpdatedByUserId");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Delegations",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Delegations",
                nullable: false,
                defaultValue: "");
        }
    }
}
