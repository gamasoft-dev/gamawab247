using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class ixbusidiooo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WhatsappUsers_PhoneNumber",
                table: "WhatsappUsers");

            migrationBuilder.CreateIndex(
                name: "IX_WhatsappUsers_PhoneNumber",
                table: "WhatsappUsers",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboundMessages_BusinessId",
                table: "OutboundMessages",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_BusinessId",
                table: "InboundMessages",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessages_BusinessId",
                table: "BusinessMessages",
                column: "BusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessMessages_Businesses_BusinessId",
                table: "BusinessMessages",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessMessages_Businesses_BusinessId",
                table: "BusinessMessages");

            migrationBuilder.DropIndex(
                name: "IX_WhatsappUsers_PhoneNumber",
                table: "WhatsappUsers");

            migrationBuilder.DropIndex(
                name: "IX_OutboundMessages_BusinessId",
                table: "OutboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboundMessages_BusinessId",
                table: "InboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMessages_BusinessId",
                table: "BusinessMessages");

            migrationBuilder.CreateIndex(
                name: "IX_WhatsappUsers_PhoneNumber",
                table: "WhatsappUsers",
                column: "PhoneNumber");
        }
    }
}
