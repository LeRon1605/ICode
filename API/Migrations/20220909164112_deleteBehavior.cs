using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class deleteBehavior : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Users_UserID",
                table: "Submissions");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Users_UserID",
                table: "Submissions",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Users_UserID",
                table: "Submissions");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Users_UserID",
                table: "Submissions",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID");
        }
    }
}
