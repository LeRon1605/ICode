using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class problem_submission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProblemID",
                table: "Submissions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Problems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Problems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ProblemID",
                table: "Submissions",
                column: "ProblemID");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Problems_ProblemID",
                table: "Submissions",
                column: "ProblemID",
                principalTable: "Problems",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Problems_ProblemID",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_ProblemID",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "ProblemID",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Problems");
        }
    }
}
