using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class requestcomplaintextrainfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DetailKey",
                table: "PartnerIntegrationDetails",
                type: "text",
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetailKey",
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
        }
    }
}
