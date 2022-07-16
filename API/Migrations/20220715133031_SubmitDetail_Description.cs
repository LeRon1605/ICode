using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class SubmitDetail_Description : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "SubmissionDetails",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "True");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SubmissionDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "SubmissionDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "SubmissionDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "True",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);
        }
    }
}
