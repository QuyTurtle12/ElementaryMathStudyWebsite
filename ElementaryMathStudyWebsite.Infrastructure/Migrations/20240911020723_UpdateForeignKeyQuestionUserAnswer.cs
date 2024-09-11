using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElementaryMathStudyWebsite.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForeignKeyQuestionUserAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswer_Question_OptionId",
                table: "UserAnswer");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswer_Question_QuestionId",
                table: "UserAnswer",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswer_Question_QuestionId",
                table: "UserAnswer");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswer_Question_OptionId",
                table: "UserAnswer",
                column: "OptionId",
                principalTable: "Question",
                principalColumn: "Id");
        }
    }
}
