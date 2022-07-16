using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class FkDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionDetails_Submissions_SubmitID",
                table: "SubmissionDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionDetails_TestCases_TestCaseID",
                table: "SubmissionDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Users_UserID",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_TestCases_Problems_ProblemID",
                table: "TestCases");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "SubmissionDetails",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionDetails_Submissions_SubmitID",
                table: "SubmissionDetails",
                column: "SubmitID",
                principalTable: "Submissions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionDetails_TestCases_TestCaseID",
                table: "SubmissionDetails",
                column: "TestCaseID",
                principalTable: "TestCases",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

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
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionDetails_Submissions_SubmitID",
                table: "SubmissionDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionDetails_TestCases_TestCaseID",
                table: "SubmissionDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Users_UserID",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_TestCases_Problems_ProblemID",
                table: "TestCases");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "SubmissionDetails",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionDetails_Submissions_SubmitID",
                table: "SubmissionDetails",
                column: "SubmitID",
                principalTable: "Submissions",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionDetails_TestCases_TestCaseID",
                table: "SubmissionDetails",
                column: "TestCaseID",
                principalTable: "TestCases",
                principalColumn: "ID");

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
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
