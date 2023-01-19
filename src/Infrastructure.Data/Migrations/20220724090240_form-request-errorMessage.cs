using System;
using Domain.Entities.FormProcessing.ValueObjects;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class formrequesterrorMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormRequestResponses_BusinessForms_BusinessFormId",
                table: "FormRequestResponses");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_FormRequestResponses_BusinessForms_BusinessFormId1",
            //    table: "FormRequestResponses");

            migrationBuilder.DropTable(
                name: "DialogSessions");

            //migrationBuilder.DropIndex(
            //    name: "IX_FormRequestResponses_BusinessFormId1",
            //    table: "FormRequestResponses");

            //migrationBuilder.DropColumn(
            //    name: "BusinessFormId1",
            //    table: "FormRequestResponses");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "FormRequestResponses",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FormRequestResponses_BusinessForms_BusinessFormId",
                table: "FormRequestResponses",
                column: "BusinessFormId",
                principalTable: "BusinessForms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormRequestResponses_BusinessForms_BusinessFormId",
                table: "FormRequestResponses");

            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "FormRequestResponses");

            migrationBuilder.AddColumn<Guid>(
                name: "BusinessFormId1",
                table: "FormRequestResponses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DialogSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessConversationId = table.Column<Guid>(type: "uuid", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    SessionFormDetails = table.Column<SessionFormDetail>(type: "jsonb", nullable: true),
                    SessionState = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    WaId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DialogSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormRequestResponses_BusinessFormId1",
                table: "FormRequestResponses",
                column: "BusinessFormId1");

            migrationBuilder.AddForeignKey(
                name: "FK_FormRequestResponses_BusinessForms_BusinessFormId",
                table: "FormRequestResponses",
                column: "BusinessFormId",
                principalTable: "BusinessForms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FormRequestResponses_BusinessForms_BusinessFormId1",
                table: "FormRequestResponses",
                column: "BusinessFormId1",
                principalTable: "BusinessForms",
                principalColumn: "Id");
        }
    }
}
