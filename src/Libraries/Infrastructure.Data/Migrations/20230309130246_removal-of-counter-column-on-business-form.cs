using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class removalofcountercolumnonbusinessform : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Counter",
                table: "BusinessForms");

            migrationBuilder.AddColumn<Guid>(
                name: "BusinessIdMessageId",
                table: "InboundMessages",
                type: "uuid",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessIdMessageId",
                table: "InboundMessages");

            migrationBuilder.AddColumn<int>(
                name: "Counter",
                table: "BusinessForms",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
