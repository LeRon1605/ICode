using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class ChangeRelationshipSubmission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Contests_ContestID",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Users_UserID1",
                table: "Submissions");

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

            migrationBuilder.CreateTable(
                name: "ContestSubmissions",
                columns: table => new
                {
                    SubmitID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContestID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestSubmissions", x => x.SubmitID);
                    table.ForeignKey(
                        name: "FK_ContestSubmissions_Contests_ContestID",
                        column: x => x.ContestID,
                        principalTable: "Contests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContestSubmissions_Submissions_SubmitID",
                        column: x => x.SubmitID,
                        principalTable: "Submissions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContestSubmissions_ContestID",
                table: "ContestSubmissions",
                column: "ContestID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContestSubmissions");

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

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ContestID",
                table: "Submissions",
                column: "ContestID");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_UserID1",
                table: "Submissions",
                column: "UserID1");

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
    }
}
