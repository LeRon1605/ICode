using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class contestDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContestID",
                table: "Submissions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Submissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserID1",
                table: "Submissions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Contests",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerLimit = table.Column<int>(type: "int", nullable: false),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contests", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ContestDetails",
                columns: table => new
                {
                    ContestID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestDetails", x => new { x.UserID, x.ContestID });
                    table.ForeignKey(
                        name: "FK_ContestDetails_Contests_ContestID",
                        column: x => x.ContestID,
                        principalTable: "Contests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContestDetails_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProblemContestDetails",
                columns: table => new
                {
                    ContestID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProblemID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemContestDetails", x => new { x.ContestID, x.ProblemID });
                    table.ForeignKey(
                        name: "FK_ProblemContestDetails_Contests_ContestID",
                        column: x => x.ContestID,
                        principalTable: "Contests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProblemContestDetails_Problems_ProblemID",
                        column: x => x.ProblemID,
                        principalTable: "Problems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ContestID",
                table: "Submissions",
                column: "ContestID");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_UserID1",
                table: "Submissions",
                column: "UserID1");

            migrationBuilder.CreateIndex(
                name: "IX_ContestDetails_ContestID",
                table: "ContestDetails",
                column: "ContestID");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemContestDetails_ProblemID",
                table: "ProblemContestDetails",
                column: "ProblemID");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Contests_ContestID",
                table: "Submissions",
                column: "ContestID",
                principalTable: "Contests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Users_UserID1",
                table: "Submissions",
                column: "UserID1",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Contests_ContestID",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Users_UserID1",
                table: "Submissions");

            migrationBuilder.DropTable(
                name: "ContestDetails");

            migrationBuilder.DropTable(
                name: "ProblemContestDetails");

            migrationBuilder.DropTable(
                name: "Contests");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_ContestID",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_UserID1",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "ContestID",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "UserID1",
                table: "Submissions");
        }
    }
}
