using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiTesting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixLinksInAttemptAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttemptAnswers_Questions_QuestionId",
                table: "AttemptAnswers");

            migrationBuilder.AddForeignKey(
                name: "FK_AttemptAnswers_Questions_QuestionId",
                table: "AttemptAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttemptAnswers_Questions_QuestionId",
                table: "AttemptAnswers");

            migrationBuilder.AddForeignKey(
                name: "FK_AttemptAnswers_Questions_QuestionId",
                table: "AttemptAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
