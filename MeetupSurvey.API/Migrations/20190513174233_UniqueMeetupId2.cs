using Microsoft.EntityFrameworkCore.Migrations;

namespace MeetupSurvey.API.Migrations
{
    public partial class UniqueMeetupId2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MeetupUserId",
                table: "UserAccount",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccount_MeetupUserId",
                table: "UserAccount",
                column: "MeetupUserId",
                unique: true,
                filter: "[MeetupUserId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAccount_MeetupUserId",
                table: "UserAccount");

            migrationBuilder.AlterColumn<string>(
                name: "MeetupUserId",
                table: "UserAccount",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
