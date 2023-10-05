using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nukleus.Infrastructure.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedOwnerUserToAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerUserId",
                table: "Account",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_OwnerUserId",
                table: "Account",
                column: "OwnerUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Account_User_OwnerUserId",
                table: "Account",
                column: "OwnerUserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_User_OwnerUserId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_OwnerUserId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "Account");
        }
    }
}
