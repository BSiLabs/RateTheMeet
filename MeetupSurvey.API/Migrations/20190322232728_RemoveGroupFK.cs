using Microsoft.EntityFrameworkCore.Migrations;

namespace MeetupSurvey.API.Migrations
{
    public partial class RemoveGroupFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Survey_Group_GroupId",
                table: "Survey");

            migrationBuilder.DropIndex(
                name: "IX_Survey_GroupId",
                table: "Survey");

            migrationBuilder.AlterColumn<string>(
                name: "GroupId",
                table: "Survey",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GroupId",
                table: "Survey",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

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
    }
}
