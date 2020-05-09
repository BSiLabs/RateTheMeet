﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace MeetupSurvey.API.Migrations
{
    public partial class PrizeFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prizes_Survey_SurveyId",
                table: "Prizes");

            migrationBuilder.DropForeignKey(
                name: "FK_Prizes_UserAccount_UserAccountId",
                table: "Prizes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prizes",
                table: "Prizes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Prizes");

            migrationBuilder.RenameTable(
                name: "Prizes",
                newName: "Prize");

            migrationBuilder.RenameIndex(
                name: "IX_Prizes_UserAccountId",
                table: "Prize",
                newName: "IX_Prize_UserAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Prizes_SurveyId",
                table: "Prize",
                newName: "IX_Prize_SurveyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prize",
                table: "Prize",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prize_Survey_SurveyId",
                table: "Prize",
                column: "SurveyId",
                principalTable: "Survey",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Prize_UserAccount_UserAccountId",
                table: "Prize",
                column: "UserAccountId",
                principalTable: "UserAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prize_Survey_SurveyId",
                table: "Prize");

            migrationBuilder.DropForeignKey(
                name: "FK_Prize_UserAccount_UserAccountId",
                table: "Prize");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prize",
                table: "Prize");

            migrationBuilder.RenameTable(
                name: "Prize",
                newName: "Prizes");

            migrationBuilder.RenameIndex(
                name: "IX_Prize_UserAccountId",
                table: "Prizes",
                newName: "IX_Prizes_UserAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Prize_SurveyId",
                table: "Prizes",
                newName: "IX_Prizes_SurveyId");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Prizes",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prizes",
                table: "Prizes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prizes_Survey_SurveyId",
                table: "Prizes",
                column: "SurveyId",
                principalTable: "Survey",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Prizes_UserAccount_UserAccountId",
                table: "Prizes",
                column: "UserAccountId",
                principalTable: "UserAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
