using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MeetupSurvey.API.Migrations
{
    public partial class removeGroupSurvey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupSurvey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupSurvey",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    GroupId = table.Column<string>(nullable: true),
                    SurveyId = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupSurvey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupSurvey_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupSurvey_Survey_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Survey",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupSurvey_GroupId",
                table: "GroupSurvey",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupSurvey_SurveyId",
                table: "GroupSurvey",
                column: "SurveyId");
        }
    }
}
