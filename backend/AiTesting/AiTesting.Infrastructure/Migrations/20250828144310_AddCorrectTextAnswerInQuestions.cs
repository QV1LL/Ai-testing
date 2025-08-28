using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiTesting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCorrectTextAnswerInQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrectTextAnswer",
                table: "Questions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectTextAnswer",
                table: "Questions");
        }
    }
}
