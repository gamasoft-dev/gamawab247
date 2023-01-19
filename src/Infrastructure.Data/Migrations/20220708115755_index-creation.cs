using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class indexcreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BusinessMessages_Position_BusinessId",
                table: "BusinessMessages");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TextMessages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "TextMessages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TextMessages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ReplyButtonMessages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "ReplyButtonMessages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ReplyButtonMessages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ListMessages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "ListMessages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ListMessages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "BusinessConversations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "BusinessConversations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "BusinessConversations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WhatsappUsers_PhoneNumber",
                table: "WhatsappUsers",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_WhatsappUsers_WaId",
                table: "WhatsappUsers",
                column: "WaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TextMessages_BusinessMessageId",
                table: "TextMessages",
                column: "BusinessMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ReplyButtonMessages_BusinessMessageId",
                table: "ReplyButtonMessages",
                column: "BusinessMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundMessages_CreatedAt",
                table: "OutboundMessages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundMessages_From",
                table: "OutboundMessages",
                column: "From");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundMessages_RecipientWhatsappId",
                table: "OutboundMessages",
                column: "RecipientWhatsappId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundMessages_Type",
                table: "OutboundMessages",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundMessages_WhatsAppMessageId",
                table: "OutboundMessages",
                column: "WhatsAppMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_CreatedAt",
                table: "MessageLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ListMessages_BusinessMessageId",
                table: "ListMessages",
                column: "BusinessMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Industries_Name",
                table: "Industries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_ContextMessageId",
                table: "InboundMessages",
                column: "ContextMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_CreatedAt",
                table: "InboundMessages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_From",
                table: "InboundMessages",
                column: "From");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_MsgOptionId",
                table: "InboundMessages",
                column: "MsgOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_Type",
                table: "InboundMessages",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_Wa_Id",
                table: "InboundMessages",
                column: "Wa_Id");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_WhatsAppId",
                table: "InboundMessages",
                column: "WhatsAppId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundMessages_WhatsAppMessageId",
                table: "InboundMessages",
                column: "WhatsAppMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessages_MessageType",
                table: "BusinessMessages",
                column: "MessageType");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessages_Name",
                table: "BusinessMessages",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessages_Position",
                table: "BusinessMessages",
                column: "Position");

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_Email",
                table: "Businesses",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_Name",
                table: "Businesses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_PhoneNumber",
                table: "Businesses",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ListMessages_BusinessMessages_BusinessMessageId",
                table: "ListMessages",
                column: "BusinessMessageId",
                principalTable: "BusinessMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReplyButtonMessages_BusinessMessages_BusinessMessageId",
                table: "ReplyButtonMessages",
                column: "BusinessMessageId",
                principalTable: "BusinessMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TextMessages_BusinessMessages_BusinessMessageId",
                table: "TextMessages",
                column: "BusinessMessageId",
                principalTable: "BusinessMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListMessages_BusinessMessages_BusinessMessageId",
                table: "ListMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ReplyButtonMessages_BusinessMessages_BusinessMessageId",
                table: "ReplyButtonMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_TextMessages_BusinessMessages_BusinessMessageId",
                table: "TextMessages");

            migrationBuilder.DropIndex(
                name: "IX_WhatsappUsers_PhoneNumber",
                table: "WhatsappUsers");

            migrationBuilder.DropIndex(
                name: "IX_WhatsappUsers_WaId",
                table: "WhatsappUsers");

            migrationBuilder.DropIndex(
                name: "IX_TextMessages_BusinessMessageId",
                table: "TextMessages");

            migrationBuilder.DropIndex(
                name: "IX_ReplyButtonMessages_BusinessMessageId",
                table: "ReplyButtonMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboundMessages_CreatedAt",
                table: "OutboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboundMessages_From",
                table: "OutboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboundMessages_RecipientWhatsappId",
                table: "OutboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboundMessages_Type",
                table: "OutboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboundMessages_WhatsAppMessageId",
                table: "OutboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_MessageLogs_CreatedAt",
                table: "MessageLogs");

            migrationBuilder.DropIndex(
                name: "IX_ListMessages_BusinessMessageId",
                table: "ListMessages");

            migrationBuilder.DropIndex(
                name: "IX_Industries_Name",
                table: "Industries");

            migrationBuilder.DropIndex(
                name: "IX_InboundMessages_ContextMessageId",
                table: "InboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboundMessages_CreatedAt",
                table: "InboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboundMessages_From",
                table: "InboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboundMessages_MsgOptionId",
                table: "InboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboundMessages_Type",
                table: "InboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboundMessages_Wa_Id",
                table: "InboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboundMessages_WhatsAppId",
                table: "InboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboundMessages_WhatsAppMessageId",
                table: "InboundMessages");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMessages_MessageType",
                table: "BusinessMessages");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMessages_Name",
                table: "BusinessMessages");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMessages_Position",
                table: "BusinessMessages");

            migrationBuilder.DropIndex(
                name: "IX_Businesses_Email",
                table: "Businesses");

            migrationBuilder.DropIndex(
                name: "IX_Businesses_Name",
                table: "Businesses");

            migrationBuilder.DropIndex(
                name: "IX_Businesses_PhoneNumber",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TextMessages");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "TextMessages");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TextMessages");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ReplyButtonMessages");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ReplyButtonMessages");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ReplyButtonMessages");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ListMessages");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ListMessages");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ListMessages");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BusinessConversations");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "BusinessConversations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "BusinessConversations");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessages_Position_BusinessId",
                table: "BusinessMessages",
                columns: new[] { "Position", "BusinessId" },
                unique: true);
        }
    }
}
