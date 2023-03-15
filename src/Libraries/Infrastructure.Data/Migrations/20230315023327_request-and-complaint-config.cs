using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class requestandcomplaintconfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BusinessId",
                table: "PartnerIntegrationDetails",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "PartnerIntegrationDetails",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "RequireWebHookNotification",
                table: "PartnerIntegrationDetails",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebHookUrl",
                table: "PartnerIntegrationDetails",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "PartnerIntegrationDetails");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "PartnerIntegrationDetails");

            migrationBuilder.DropColumn(
                name: "RequireWebHookNotification",
                table: "PartnerIntegrationDetails");

            migrationBuilder.DropColumn(
                name: "WebHookUrl",
                table: "PartnerIntegrationDetails");
        }
    }
}
