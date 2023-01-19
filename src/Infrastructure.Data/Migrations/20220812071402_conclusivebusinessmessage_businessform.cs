using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class conclusivebusinessmessage_businessform : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsValidationResponse",
                table: "FormRequestResponses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ConclusionBusinessMessageId",
                table: "BusinessForms",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessForms_ConclusionBusinessMessageId",
                table: "BusinessForms",
                column: "ConclusionBusinessMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessForms_BusinessMessages_ConclusionBusinessMessageId",
                table: "BusinessForms",
                column: "ConclusionBusinessMessageId",
                principalTable: "BusinessMessages",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessForms_BusinessMessages_ConclusionBusinessMessageId",
                table: "BusinessForms");

            migrationBuilder.DropIndex(
                name: "IX_BusinessForms_ConclusionBusinessMessageId",
                table: "BusinessForms");

            migrationBuilder.DropColumn(
                name: "IsValidationResponse",
                table: "FormRequestResponses");

            migrationBuilder.DropColumn(
                name: "ConclusionBusinessMessageId",
                table: "BusinessForms");
        }
    }
}
