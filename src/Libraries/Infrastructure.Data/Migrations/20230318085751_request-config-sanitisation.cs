using System;
using System.Collections.Generic;
using Domain.Entities.FormProcessing.ValueObjects;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class requestconfigsanitisation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "PartnerIntegrationDetails");

            migrationBuilder.DropColumn(
                name: "DetailKey",
                table: "PartnerIntegrationDetails");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "PartnerIntegrationDetails");

            migrationBuilder.DropColumn(
                name: "RequireWebHookNotification",
                table: "PartnerIntegrationDetails");

            migrationBuilder.DropColumn(
                name: "SubjectKey",
                table: "PartnerIntegrationDetails");

            migrationBuilder.DropColumn(
                name: "TimeInHoursOfComplaintResolution",
                table: "PartnerIntegrationDetails");

            migrationBuilder.DropColumn(
                name: "TimeInHoursOfRequestResolution",
                table: "PartnerIntegrationDetails");

            migrationBuilder.DropColumn(
                name: "WebHookUrl",
                table: "PartnerIntegrationDetails");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RequestAndComplaints",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "RequestAndComplaints",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RequestAndComplaints",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RequestAndComplaintConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartnerContentProcessorKey = table.Column<string>(type: "text", nullable: true),
                    Headers = table.Column<List<KeyValueObj>>(type: "jsonb", nullable: true),
                    Parameters = table.Column<List<KeyValueObj>>(type: "jsonb", nullable: true),
                    FullUrl = table.Column<string>(type: "text", nullable: true),
                    MetaData = table.Column<string>(type: "text", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: true),
                    WebHookUrl = table.Column<string>(type: "text", nullable: true),
                    RequireWebHookNotification = table.Column<bool>(type: "boolean", nullable: false),
                    SubjectKey = table.Column<string>(type: "text", nullable: true),
                    DetailKey = table.Column<string>(type: "text", nullable: true),
                    TimeInHoursOfRequestResolution = table.Column<int>(type: "integer", nullable: false),
                    TimeInHoursOfComplaintResolution = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestAndComplaintConfigs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestAndComplaintConfigs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RequestAndComplaints");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "RequestAndComplaints");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RequestAndComplaints");

            migrationBuilder.AddColumn<Guid>(
                name: "BusinessId",
                table: "PartnerIntegrationDetails",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DetailKey",
                table: "PartnerIntegrationDetails",
                type: "text",
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
                name: "SubjectKey",
                table: "PartnerIntegrationDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeInHoursOfComplaintResolution",
                table: "PartnerIntegrationDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeInHoursOfRequestResolution",
                table: "PartnerIntegrationDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebHookUrl",
                table: "PartnerIntegrationDetails",
                type: "text",
                nullable: true);
        }
    }
}
