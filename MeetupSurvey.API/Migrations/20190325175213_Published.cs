using Microsoft.EntityFrameworkCore.Migrations;

namespace MeetupSurvey.API.Migrations
{
    public partial class Published : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Published",
                table: "Survey",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Published",
                table: "Survey");
        }
    }
}
