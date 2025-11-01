using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiTesting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixLinksInQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerOptionQuestion");

            migrationBuilder.CreateTable(
                name: "QuestionCorrectAnswers",
                columns: table => new
                {
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnswerOptionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionCorrectAnswers", x => new { x.QuestionId, x.AnswerOptionId });
                    table.ForeignKey(
                        name: "FK_QuestionCorrectAnswers_AnswerOptions_AnswerOptionId",
                        column: x => x.AnswerOptionId,
                        principalTable: "AnswerOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionCorrectAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionCorrectAnswers_AnswerOptionId",
                table: "QuestionCorrectAnswers",
                column: "AnswerOptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionCorrectAnswers");

            migrationBuilder.CreateTable(
                name: "AnswerOptionQuestion",
                columns: table => new
                {
                    CorrectAnswersId = table.Column<Guid>(type: "uuid", nullable: false),
                    Question1Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerOptionQuestion", x => new { x.CorrectAnswersId, x.Question1Id });
                    table.ForeignKey(
                        name: "FK_AnswerOptionQuestion_AnswerOptions_CorrectAnswersId",
                        column: x => x.CorrectAnswersId,
                        principalTable: "AnswerOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnswerOptionQuestion_Questions_Question1Id",
                        column: x => x.Question1Id,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerOptionQuestion_Question1Id",
                table: "AnswerOptionQuestion",
                column: "Question1Id");
        }
    }
}
