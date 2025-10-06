using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiTesting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJoinIdPropertyToTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JoinId",
                table: "Tests",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JoinId",
                table: "Tests");
        }
    }
}
