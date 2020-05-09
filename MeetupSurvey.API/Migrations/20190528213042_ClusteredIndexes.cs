using Microsoft.EntityFrameworkCore.Migrations;

namespace MeetupSurvey.API.Migrations
{
    public partial class ClusteredIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserAccount_CreatedAt",
                table: "UserAccount",
                column: "CreatedAt")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Survey_CreatedAt",
                table: "Survey",
                column: "CreatedAt")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Question_CreatedAt",
                table: "Question",
                column: "CreatedAt")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Prize_CreatedAt",
                table: "Prize",
                column: "CreatedAt")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupUser_CreatedAt",
                table: "GroupUser",
                column: "CreatedAt")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Group_CreatedAt",
                table: "Group",
                column: "CreatedAt")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Answer_CreatedAt",
                table: "Answer",
                column: "CreatedAt")
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAccount_CreatedAt",
                table: "UserAccount");

            migrationBuilder.DropIndex(
                name: "IX_Survey_CreatedAt",
                table: "Survey");

            migrationBuilder.DropIndex(
                name: "IX_Question_CreatedAt",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Prize_CreatedAt",
                table: "Prize");

            migrationBuilder.DropIndex(
                name: "IX_GroupUser_CreatedAt",
                table: "GroupUser");

            migrationBuilder.DropIndex(
                name: "IX_Group_CreatedAt",
                table: "Group");

            migrationBuilder.DropIndex(
                name: "IX_Answer_CreatedAt",
                table: "Answer");
        }
    }
}
