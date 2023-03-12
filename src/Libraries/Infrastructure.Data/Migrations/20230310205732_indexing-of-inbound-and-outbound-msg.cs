using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class indexingofinboundandoutboundmsg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboundMessages_WhatsAppMessageId",
                table: "OutboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboundMessages_BusinessId",
                table: "InboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboundMessages_CreatedAt",
                table: "InboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboundMessages_From",
                table: "InboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboundMessages_Type",
                table: "InboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboundMessages_WhatsAppId",
                table: "InboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboundMessages_WhatsAppMessageId",
                table: "InboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMessages_BusinessId",
                table: "BusinessMessages");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundMessages_WhatsAppMessageId_RecipientWhatsappId_Crea~",
                table: "OutboundMessages",
                columns: new[] { "WhatsAppMessageId", "RecipientWhatsappId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_BusinessIdMessageId",
                table: "InboundMessages",
                column: "BusinessIdMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_ResponseProcessingStatus_CreatedAt",
                table: "InboundMessages",
                columns: new[] { "ResponseProcessingStatus", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessages_BusinessId_Position",
                table: "BusinessMessages",
                columns: new[] { "BusinessId", "Position" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboundMessages_WhatsAppMessageId_RecipientWhatsappId_Crea~",
                table: "OutboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboundMessages_BusinessIdMessageId",
                table: "InboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboundMessages_ResponseProcessingStatus_CreatedAt",
                table: "InboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMessages_BusinessId_Position",
                table: "BusinessMessages");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundMessages_WhatsAppMessageId",
                table: "OutboundMessages",
                column: "WhatsAppMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_BusinessId",
                table: "InboundMessages",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_CreatedAt",
                table: "InboundMessages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_From",
                table: "InboundMessages",
                column: "From");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_Type",
                table: "InboundMessages",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_WhatsAppId",
                table: "InboundMessages",
                column: "WhatsAppId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_WhatsAppMessageId",
                table: "InboundMessages",
                column: "WhatsAppMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessages_BusinessId",
                table: "BusinessMessages",
                column: "BusinessId");
        }
    }
}
