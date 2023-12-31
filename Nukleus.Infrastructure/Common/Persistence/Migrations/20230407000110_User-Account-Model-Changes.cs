﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nukleus.Infrastructure.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserAccountModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_User_OwnerUserId",
                table: "Account");

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerUserId",
                table: "Account",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Account_User_OwnerUserId",
                table: "Account",
                column: "OwnerUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_User_OwnerUserId",
                table: "Account");

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerUserId",
                table: "Account",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_User_OwnerUserId",
                table: "Account",
                column: "OwnerUserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
