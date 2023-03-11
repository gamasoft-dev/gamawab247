using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class business_messageexternalContentRetrievalprocessorkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessMessages_PartnerIntegrationDetails_ExternalContentR~",
                table: "BusinessMessages");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMessages_ExternalContentRetrievalId",
                table: "BusinessMessages");

            migrationBuilder.DropColumn(
                name: "ExternalContentRetrievalId",
                table: "BusinessMessages");

            migrationBuilder.AddColumn<string>(
                name: "ContentRetrievalProcessorKey",
                table: "BusinessMessages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShouldRetrieveContentAtRuntime",
                table: "BusinessMessages",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentRetrievalProcessorKey",
                table: "BusinessMessages");

            migrationBuilder.DropColumn(
                name: "ShouldRetrieveContentAtRuntime",
                table: "BusinessMessages");

            migrationBuilder.AddColumn<Guid>(
                name: "ExternalContentRetrievalId",
                table: "BusinessMessages",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessages_ExternalContentRetrievalId",
                table: "BusinessMessages",
                column: "ExternalContentRetrievalId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessMessages_PartnerIntegrationDetails_ExternalContentR~",
                table: "BusinessMessages",
                column: "ExternalContentRetrievalId",
                principalTable: "PartnerIntegrationDetails",
                principalColumn: "Id");
        }
    }
}
