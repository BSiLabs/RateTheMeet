using Microsoft.EntityFrameworkCore.Migrations;

namespace MeetupSurvey.API.Migrations
{
    public partial class UserAccountChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "UserAccount",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "UserAccount",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeetupUserId",
                table: "UserAccount",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "UserAccount");

            migrationBuilder.DropColumn(
                name: "MeetupUserId",
                table: "UserAccount");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "UserAccount",
                newName: "UserName");
        }
    }
}
