using System;
using System.Collections.Generic;
using Domain.Entities.FormProcessing.ValueObjects;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class partnerdetailstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_BusinessMessageSettings_BusinessMessageSettingsId",
                table: "Businesses");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessMessageSettings_Businesses_BusinessId1",
                table: "BusinessMessageSettings");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMessageSettings_BusinessId1",
                table: "BusinessMessageSettings");

            migrationBuilder.DropIndex(
                name: "IX_Businesses_BusinessMessageSettingsId",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "BusinessId1",
                table: "BusinessMessageSettings");

            migrationBuilder.AddColumn<Guid>(
                name: "ExternalContentRetrievalId",
                table: "BusinessMessages",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PartnerIntegrationDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartnerContentProcessorKey = table.Column<string>(type: "text", nullable: true),
                    Headers = table.Column<List<KeyValueObj>>(type: "jsonb", nullable: true),
                    Parameters = table.Column<List<KeyValueObj>>(type: "jsonb", nullable: true),
                    FullUrl = table.Column<string>(type: "text", nullable: true),
                    MetaData = table.Column<string>(type: "text", nullable: true),
                    PartnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerIntegrationDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnerIntegrationDetails_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessageSettings_BusinessId",
                table: "BusinessMessageSettings",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessages_ExternalContentRetrievalId",
                table: "BusinessMessages",
                column: "ExternalContentRetrievalId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerIntegrationDetails_PartnerId",
                table: "PartnerIntegrationDetails",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessMessages_PartnerIntegrationDetails_ExternalContentR~",
                table: "BusinessMessages",
                column: "ExternalContentRetrievalId",
                principalTable: "PartnerIntegrationDetails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessMessageSettings_Businesses_BusinessId",
                table: "BusinessMessageSettings",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessMessages_PartnerIntegrationDetails_ExternalContentR~",
                table: "BusinessMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessMessageSettings_Businesses_BusinessId",
                table: "BusinessMessageSettings");

            migrationBuilder.DropTable(
                name: "PartnerIntegrationDetails");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMessageSettings_BusinessId",
                table: "BusinessMessageSettings");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMessages_ExternalContentRetrievalId",
                table: "BusinessMessages");

            migrationBuilder.DropColumn(
                name: "ExternalContentRetrievalId",
                table: "BusinessMessages");

            migrationBuilder.AddColumn<Guid>(
                name: "BusinessId1",
                table: "BusinessMessageSettings",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessageSettings_BusinessId1",
                table: "BusinessMessageSettings",
                column: "BusinessId1");

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_BusinessMessageSettingsId",
                table: "Businesses",
                column: "BusinessMessageSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_BusinessMessageSettings_BusinessMessageSettingsId",
                table: "Businesses",
                column: "BusinessMessageSettingsId",
                principalTable: "BusinessMessageSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessMessageSettings_Businesses_BusinessId1",
                table: "BusinessMessageSettings",
                column: "BusinessId1",
                principalTable: "Businesses",
                principalColumn: "Id");
        }
    }
}
