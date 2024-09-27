using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElementaryMathStudyWebsite.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapter_User_CreatedBy",
                table: "Chapter");

            migrationBuilder.DropForeignKey(
                name: "FK_Chapter_User_DeletedBy",
                table: "Chapter");

            migrationBuilder.DropForeignKey(
                name: "FK_Chapter_User_LastUpdatedBy",
                table: "Chapter");

            migrationBuilder.DropForeignKey(
                name: "FK_Option_User_CreatedBy",
                table: "Option");

            migrationBuilder.DropForeignKey(
                name: "FK_Option_User_DeletedBy",
                table: "Option");

            migrationBuilder.DropForeignKey(
                name: "FK_Option_User_LastUpdatedBy",
                table: "Option");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_User_CreatedBy",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_User_DeletedBy",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_User_LastUpdatedBy",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Question_User_CreatedBy",
                table: "Question");

            migrationBuilder.DropForeignKey(
                name: "FK_Question_User_DeletedBy",
                table: "Question");

            migrationBuilder.DropForeignKey(
                name: "FK_Question_User_LastUpdatedBy",
                table: "Question");

            migrationBuilder.DropForeignKey(
                name: "FK_Quiz_User_CreatedBy",
                table: "Quiz");

            migrationBuilder.DropForeignKey(
                name: "FK_Quiz_User_DeletedBy",
                table: "Quiz");

            migrationBuilder.DropForeignKey(
                name: "FK_Quiz_User_LastUpdatedBy",
                table: "Quiz");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_User_CreatedBy",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_User_DeletedBy",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_User_LastUpdatedBy",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_Subject_User_CreatedBy",
                table: "Subject");

            migrationBuilder.DropForeignKey(
                name: "FK_Subject_User_DeletedBy",
                table: "Subject");

            migrationBuilder.DropForeignKey(
                name: "FK_Subject_User_LastUpdatedBy",
                table: "Subject");

            migrationBuilder.DropForeignKey(
                name: "FK_Topic_User_CreatedBy",
                table: "Topic");

            migrationBuilder.DropForeignKey(
                name: "FK_Topic_User_DeletedBy",
                table: "Topic");

            migrationBuilder.DropForeignKey(
                name: "FK_Topic_User_LastUpdatedBy",
                table: "Topic");

            migrationBuilder.DropForeignKey(
                name: "FK_User_User_CreatedBy",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_User_DeletedBy",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_User_LastUpdatedBy",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_CreatedBy",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_DeletedBy",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_LastUpdatedBy",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Topic_CreatedBy",
                table: "Topic");

            migrationBuilder.DropIndex(
                name: "IX_Topic_DeletedBy",
                table: "Topic");

            migrationBuilder.DropIndex(
                name: "IX_Topic_LastUpdatedBy",
                table: "Topic");

            migrationBuilder.DropIndex(
                name: "IX_Subject_CreatedBy",
                table: "Subject");

            migrationBuilder.DropIndex(
                name: "IX_Subject_DeletedBy",
                table: "Subject");

            migrationBuilder.DropIndex(
                name: "IX_Subject_LastUpdatedBy",
                table: "Subject");

            migrationBuilder.DropIndex(
                name: "IX_Role_CreatedBy",
                table: "Role");

            migrationBuilder.DropIndex(
                name: "IX_Role_DeletedBy",
                table: "Role");

            migrationBuilder.DropIndex(
                name: "IX_Role_LastUpdatedBy",
                table: "Role");

            migrationBuilder.DropIndex(
                name: "IX_Quiz_CreatedBy",
                table: "Quiz");

            migrationBuilder.DropIndex(
                name: "IX_Quiz_DeletedBy",
                table: "Quiz");

            migrationBuilder.DropIndex(
                name: "IX_Quiz_LastUpdatedBy",
                table: "Quiz");

            migrationBuilder.DropIndex(
                name: "IX_Question_CreatedBy",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Question_DeletedBy",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Question_LastUpdatedBy",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Order_CreatedBy",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_DeletedBy",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_LastUpdatedBy",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Option_CreatedBy",
                table: "Option");

            migrationBuilder.DropIndex(
                name: "IX_Option_DeletedBy",
                table: "Option");

            migrationBuilder.DropIndex(
                name: "IX_Option_LastUpdatedBy",
                table: "Option");

            migrationBuilder.DropIndex(
                name: "IX_Chapter_CreatedBy",
                table: "Chapter");

            migrationBuilder.DropIndex(
                name: "IX_Chapter_DeletedBy",
                table: "Chapter");

            migrationBuilder.DropIndex(
                name: "IX_Chapter_LastUpdatedBy",
                table: "Chapter");

            migrationBuilder.AlterColumn<string>(
                name: "LastUpdatedBy",
                table: "User",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "User",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "User",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ResetTokenExpiry",
                table: "User",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Number",
                table: "Topic",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastUpdatedBy",
                table: "Topic",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Topic",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Topic",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastUpdatedBy",
                table: "Subject",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Subject",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Subject",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastUpdatedBy",
                table: "Role",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Role",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Role",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Quiz",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastUpdatedBy",
                table: "Quiz",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Quiz",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Criteria",
                table: "Quiz",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Quiz",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastUpdatedBy",
                table: "Question",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Question",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Question",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Question",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "Question",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedByUserId",
                table: "Question",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastUpdatedBy",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastUpdatedBy",
                table: "Option",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Option",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Option",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Number",
                table: "Chapter",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastUpdatedBy",
                table: "Chapter",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Chapter",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Chapter",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Question_CreatedByUserId",
                table: "Question",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_DeletedByUserId",
                table: "Question",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_LastUpdatedByUserId",
                table: "Question",
                column: "LastUpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_User_CreatedByUserId",
                table: "Question",
                column: "CreatedByUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_User_DeletedByUserId",
                table: "Question",
                column: "DeletedByUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_User_LastUpdatedByUserId",
                table: "Question",
                column: "LastUpdatedByUserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_User_CreatedByUserId",
                table: "Question");

            migrationBuilder.DropForeignKey(
                name: "FK_Question_User_DeletedByUserId",
                table: "Question");

            migrationBuilder.DropForeignKey(
                name: "FK_Question_User_LastUpdatedByUserId",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Question_CreatedByUserId",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Question_DeletedByUserId",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Question_LastUpdatedByUserId",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ResetTokenExpiry",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "LastUpdatedByUserId",
                table: "Question");

            migrationBuilder.AlterColumn<string>(
                name: "LastUpdatedBy",
                table: "User",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "User",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "User",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "User",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Number",
                table: "Topic",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "LastUpdatedBy",
                table: "Topic",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Topic",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Topic",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastUpdatedBy",
                table: "Subject",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Subject",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Subject",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastUpdatedBy",
                table: "Role",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Role",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Role",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Quiz",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "LastUpdatedBy",
                table: "Quiz",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Quiz",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Criteria",
                table: "Quiz",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Quiz",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastUpdatedBy",
                table: "Question",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Question",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Question",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastUpdatedBy",
                table: "Order",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Order",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Order",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastUpdatedBy",
                table: "Option",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Option",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Option",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Number",
                table: "Chapter",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "LastUpdatedBy",
                table: "Chapter",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Chapter",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Chapter",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_CreatedBy",
                table: "User",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_User_DeletedBy",
                table: "User",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_User_LastUpdatedBy",
                table: "User",
                column: "LastUpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_CreatedBy",
                table: "Topic",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_DeletedBy",
                table: "Topic",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_LastUpdatedBy",
                table: "Topic",
                column: "LastUpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_CreatedBy",
                table: "Subject",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_DeletedBy",
                table: "Subject",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_LastUpdatedBy",
                table: "Subject",
                column: "LastUpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Role_CreatedBy",
                table: "Role",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Role_DeletedBy",
                table: "Role",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Role_LastUpdatedBy",
                table: "Role",
                column: "LastUpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_CreatedBy",
                table: "Quiz",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_DeletedBy",
                table: "Quiz",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_LastUpdatedBy",
                table: "Quiz",
                column: "LastUpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Question_CreatedBy",
                table: "Question",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Question_DeletedBy",
                table: "Question",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Question_LastUpdatedBy",
                table: "Question",
                column: "LastUpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CreatedBy",
                table: "Order",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Order_DeletedBy",
                table: "Order",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Order_LastUpdatedBy",
                table: "Order",
                column: "LastUpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Option_CreatedBy",
                table: "Option",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Option_DeletedBy",
                table: "Option",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Option_LastUpdatedBy",
                table: "Option",
                column: "LastUpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Chapter_CreatedBy",
                table: "Chapter",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Chapter_DeletedBy",
                table: "Chapter",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Chapter_LastUpdatedBy",
                table: "Chapter",
                column: "LastUpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapter_User_CreatedBy",
                table: "Chapter",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chapter_User_DeletedBy",
                table: "Chapter",
                column: "DeletedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chapter_User_LastUpdatedBy",
                table: "Chapter",
                column: "LastUpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Option_User_CreatedBy",
                table: "Option",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Option_User_DeletedBy",
                table: "Option",
                column: "DeletedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Option_User_LastUpdatedBy",
                table: "Option",
                column: "LastUpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_User_CreatedBy",
                table: "Order",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_User_DeletedBy",
                table: "Order",
                column: "DeletedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_User_LastUpdatedBy",
                table: "Order",
                column: "LastUpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Question_User_CreatedBy",
                table: "Question",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Question_User_DeletedBy",
                table: "Question",
                column: "DeletedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Question_User_LastUpdatedBy",
                table: "Question",
                column: "LastUpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quiz_User_CreatedBy",
                table: "Quiz",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quiz_User_DeletedBy",
                table: "Quiz",
                column: "DeletedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Quiz_User_LastUpdatedBy",
                table: "Quiz",
                column: "LastUpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Role_User_CreatedBy",
                table: "Role",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Role_User_DeletedBy",
                table: "Role",
                column: "DeletedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Role_User_LastUpdatedBy",
                table: "Role",
                column: "LastUpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subject_User_CreatedBy",
                table: "Subject",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subject_User_DeletedBy",
                table: "Subject",
                column: "DeletedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subject_User_LastUpdatedBy",
                table: "Subject",
                column: "LastUpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_User_CreatedBy",
                table: "Topic",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_User_DeletedBy",
                table: "Topic",
                column: "DeletedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_User_LastUpdatedBy",
                table: "Topic",
                column: "LastUpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_User_CreatedBy",
                table: "User",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_User_DeletedBy",
                table: "User",
                column: "DeletedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_User_LastUpdatedBy",
                table: "User",
                column: "LastUpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
