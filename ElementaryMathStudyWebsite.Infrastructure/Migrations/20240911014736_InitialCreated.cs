using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElementaryMathStudyWebsite.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chapter",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: true),
                    ChapterName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    SubjectId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuizId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Criteria = table.Column<double>(type: "float", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Option",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuestionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Option", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TotalPrice = table.Column<double>(type: "float", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubjectId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => new { x.OrderId, x.SubjectId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_OrderDetail_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Progress",
                columns: table => new
                {
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuizId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubjectId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Progress", x => new { x.StudentId, x.QuizId });
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuestionContext = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    QuizId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quiz",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuizName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quiz", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_User_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_User_LastUpdatedBy",
                        column: x => x.LastUpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subject",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubjectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subject_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subject_User_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subject_User_LastUpdatedBy",
                        column: x => x.LastUpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Topic",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: true),
                    TopicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    Criteria = table.Column<double>(type: "float", nullable: true),
                    QuizId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChapterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topic_Chapter_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapter",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Topic_Quiz_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quiz",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Topic_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Topic_User_DeletedBy",
                        column: x => x.DeletedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Topic_User_LastUpdatedBy",
                        column: x => x.LastUpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAnswer",
                columns: table => new
                {
                    QuestionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OptionId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAnswer", x => new { x.QuestionId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserAnswer_Option_OptionId",
                        column: x => x.OptionId,
                        principalTable: "Option",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserAnswer_Question_OptionId",
                        column: x => x.OptionId,
                        principalTable: "Question",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserAnswer_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Chapter_QuizId",
                table: "Chapter",
                column: "QuizId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chapter_SubjectId",
                table: "Chapter",
                column: "SubjectId");

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
                name: "IX_Option_QuestionId",
                table: "Option",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CreatedBy",
                table: "Order",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CustomerId",
                table: "Order",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_DeletedBy",
                table: "Order",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Order_LastUpdatedBy",
                table: "Order",
                column: "LastUpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_StudentId",
                table: "OrderDetail",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_SubjectId",
                table: "OrderDetail",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Progress_QuizId",
                table: "Progress",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Progress_SubjectId",
                table: "Progress",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Progress_UserId",
                table: "Progress",
                column: "UserId");

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
                name: "IX_Question_QuizId",
                table: "Question",
                column: "QuizId");

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
                name: "IX_Topic_ChapterId",
                table: "Topic",
                column: "ChapterId");

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
                name: "IX_Topic_QuizId",
                table: "Topic",
                column: "QuizId",
                unique: true);

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
                name: "IX_User_RoleId",
                table: "User",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswer_OptionId",
                table: "UserAnswer",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswer_UserId",
                table: "UserAnswer",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapter_Quiz_QuizId",
                table: "Chapter",
                column: "QuizId",
                principalTable: "Quiz",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapter_Subject_SubjectId",
                table: "Chapter",
                column: "SubjectId",
                principalTable: "Subject",
                principalColumn: "Id");

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
                name: "FK_Option_Question_QuestionId",
                table: "Option",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id");

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
                name: "FK_Order_User_CustomerId",
                table: "Order",
                column: "CustomerId",
                principalTable: "User",
                principalColumn: "Id");

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
                name: "FK_OrderDetail_Subject_SubjectId",
                table: "OrderDetail",
                column: "SubjectId",
                principalTable: "Subject",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_User_StudentId",
                table: "OrderDetail",
                column: "StudentId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Progress_Quiz_QuizId",
                table: "Progress",
                column: "QuizId",
                principalTable: "Quiz",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Progress_Subject_SubjectId",
                table: "Progress",
                column: "SubjectId",
                principalTable: "Subject",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Progress_User_UserId",
                table: "Progress",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_Quiz_QuizId",
                table: "Question",
                column: "QuizId",
                principalTable: "Quiz",
                principalColumn: "Id");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Role_User_CreatedBy",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_User_DeletedBy",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_User_LastUpdatedBy",
                table: "Role");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "Progress");

            migrationBuilder.DropTable(
                name: "Topic");

            migrationBuilder.DropTable(
                name: "UserAnswer");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Chapter");

            migrationBuilder.DropTable(
                name: "Option");

            migrationBuilder.DropTable(
                name: "Subject");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "Quiz");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
