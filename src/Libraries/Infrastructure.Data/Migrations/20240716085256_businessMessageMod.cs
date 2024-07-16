using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class businessMessageMod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IsRespondedBy",
                table: "InboundMessages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminResponseStatus",
                table: "BusinessMessages",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRespondedBy",
                table: "InboundMessages");

            migrationBuilder.DropColumn(
                name: "AdminResponseStatus",
                table: "BusinessMessages");
        }
    }
}
