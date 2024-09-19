using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElementaryMathStudyWebsite.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddedVerificationTokenInUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerificationToken",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationToken",
                table: "User");
        }
    }
}
