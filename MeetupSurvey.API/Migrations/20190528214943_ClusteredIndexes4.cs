using Microsoft.EntityFrameworkCore.Migrations;

namespace MeetupSurvey.API.Migrations
{
    public partial class ClusteredIndexes4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAccount_Id",
                table: "UserAccount");

            migrationBuilder.DropIndex(
                name: "IX_Survey_Id",
                table: "Survey");

            migrationBuilder.DropIndex(
                name: "IX_Question_Id",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Prize_Id",
                table: "Prize");

            migrationBuilder.DropIndex(
                name: "IX_GroupUser_Id",
                table: "GroupUser");

            migrationBuilder.DropIndex(
                name: "IX_Group_Id",
                table: "Group");

            migrationBuilder.DropIndex(
                name: "IX_Answer_Id",
                table: "Answer");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccount_Id",
                table: "UserAccount",
                column: "Id",
                unique: true)
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Survey_Id",
                table: "Survey",
                column: "Id",
                unique: true)
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Question_Id",
                table: "Question",
                column: "Id",
                unique: true)
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Prize_Id",
                table: "Prize",
                column: "Id",
                unique: true)
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_GroupUser_Id",
                table: "GroupUser",
                column: "Id",
                unique: true)
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Group_Id",
                table: "Group",
                column: "Id",
                unique: true)
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Answer_Id",
                table: "Answer",
                column: "Id",
                unique: true)
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAccount_Id",
                table: "UserAccount");

            migrationBuilder.DropIndex(
                name: "IX_Survey_Id",
                table: "Survey");

            migrationBuilder.DropIndex(
                name: "IX_Question_Id",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Prize_Id",
                table: "Prize");

            migrationBuilder.DropIndex(
                name: "IX_GroupUser_Id",
                table: "GroupUser");

            migrationBuilder.DropIndex(
                name: "IX_Group_Id",
                table: "Group");

            migrationBuilder.DropIndex(
                name: "IX_Answer_Id",
                table: "Answer");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccount_Id",
                table: "UserAccount",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Survey_Id",
                table: "Survey",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Question_Id",
                table: "Question",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Prize_Id",
                table: "Prize",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_GroupUser_Id",
                table: "GroupUser",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Group_Id",
                table: "Group",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Answer_Id",
                table: "Answer",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);
        }
    }
}
