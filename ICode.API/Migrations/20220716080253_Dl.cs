using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class Dl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Users_UserID",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_TestCases_Problems_ProblemID",
                table: "TestCases");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Users_UserID",
                table: "Submissions",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCases_Problems_ProblemID",
                table: "TestCases",
                column: "ProblemID",
                principalTable: "Problems",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Users_UserID",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_TestCases_Problems_ProblemID",
                table: "TestCases");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Users_UserID",
                table: "Submissions",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestCases_Problems_ProblemID",
                table: "TestCases",
                column: "ProblemID",
                principalTable: "Problems",
                principalColumn: "ID");
        }
    }
}
