using Microsoft.EntityFrameworkCore.Migrations;

namespace MeetupSurvey.API.Migrations
{
    public partial class groupid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupId",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeetupGroupId",
                table: "Group",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Survey_GroupId",
                table: "Survey",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Survey_Group_GroupId",
                table: "Survey",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Survey_Group_GroupId",
                table: "Survey");

            migrationBuilder.DropIndex(
                name: "IX_Survey_GroupId",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "MeetupGroupId",
                table: "Group");
        }
    }
}
