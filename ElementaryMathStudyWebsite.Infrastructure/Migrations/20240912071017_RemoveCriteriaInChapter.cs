using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElementaryMathStudyWebsite.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCriteriaInChapter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Criteria",
                table: "Chapter");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Criteria",
                table: "Chapter",
                type: "float",
                nullable: true);
        }
    }
}
