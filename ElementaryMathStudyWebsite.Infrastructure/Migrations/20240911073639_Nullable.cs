using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElementaryMathStudyWebsite.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class Nullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Topic_QuizId",
                table: "Topic");

            migrationBuilder.DropIndex(
                name: "IX_Chapter_QuizId",
                table: "Chapter");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "User",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "QuizId",
                table: "Topic",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "SubjectId",
                table: "Progress",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "QuizId",
                table: "Chapter",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_QuizId",
                table: "Topic",
                column: "QuizId",
                unique: true,
                filter: "[QuizId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Chapter_QuizId",
                table: "Chapter",
                column: "QuizId",
                unique: true,
                filter: "[QuizId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Topic_QuizId",
                table: "Topic");

            migrationBuilder.DropIndex(
                name: "IX_Chapter_QuizId",
                table: "Chapter");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "User",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QuizId",
                table: "Topic",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubjectId",
                table: "Progress",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QuizId",
                table: "Chapter",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Topic_QuizId",
                table: "Topic",
                column: "QuizId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chapter_QuizId",
                table: "Chapter",
                column: "QuizId",
                unique: true);
        }
    }
}
