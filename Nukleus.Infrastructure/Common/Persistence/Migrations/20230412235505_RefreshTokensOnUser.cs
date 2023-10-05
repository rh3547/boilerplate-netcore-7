using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nukleus.Infrastructure.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefreshTokensOnUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "User",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "User");
        }
    }
}
