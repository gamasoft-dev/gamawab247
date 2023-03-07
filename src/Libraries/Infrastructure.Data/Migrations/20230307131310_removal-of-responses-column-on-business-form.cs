using System.Collections.Generic;
using Domain.Entities.FormProcessing.ValueObjects;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class removalofresponsescolumnonbusinessform : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponseKvps",
                table: "BusinessForms");

            migrationBuilder.RenameColumn(
                name: "IsRequestSuccessful",
                table: "BusinessForms",
                newName: "IsSummaryOfFormMessagesRequired");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSummaryOfFormMessagesRequired",
                table: "BusinessForms",
                newName: "IsRequestSuccessful");

            migrationBuilder.AddColumn<List<FormResponseKvp>>(
                name: "ResponseKvps",
                table: "BusinessForms",
                type: "jsonb",
                nullable: true);
        }
    }
}
