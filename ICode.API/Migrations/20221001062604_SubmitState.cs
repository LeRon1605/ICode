using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class SubmitState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "SubmissionDetails");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SubmissionDetails");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Submissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "SubmissionDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "State",
                table: "SubmissionDetails");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Submissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Submissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SubmissionDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "SubmissionDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
