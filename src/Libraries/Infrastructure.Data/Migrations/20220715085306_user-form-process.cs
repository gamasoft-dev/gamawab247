using System;
using System.Collections.Generic;
using Domain.Entities.FormProcessing.ValueObjects;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class userformprocess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanUseNLPMapping",
                table: "InboundMessages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "InboundMessages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BusinessFormId",
                table: "BusinessMessages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShouldTriggerFormProcessing",
                table: "BusinessMessages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BusinessForms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessConversationId = table.Column<Guid>(type: "uuid", nullable: true),
                    FormElements = table.Column<List<FormElement>>(type: "jsonb", nullable: true),
                    Headers = table.Column<List<FormHeader>>(type: "jsonb", nullable: true),
                    ResponseKvps = table.Column<List<FormResponseKvp>>(type: "jsonb", nullable: true),
                    IsFormToBeSubmittedToUrl = table.Column<bool>(type: "boolean", nullable: false),
                    SubmissionUrl = table.Column<string>(type: "text", nullable: true),
                    UrlMethodType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessForms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DialogSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessConversationId = table.Column<Guid>(type: "uuid", nullable: true),
                    WaId = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    SessionState = table.Column<int>(type: "integer", nullable: false),
                    SessionFormDetails = table.Column<SessionFormDetail>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DialogSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserFormDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessFormId = table.Column<Guid>(type: "uuid", nullable: false),
                    DialogSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserInputDetails = table.Column<List<UserInputDetail>>(type: "jsonb", nullable: true),
                    IsFormCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFormDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFormDatas_BusinessForms_BusinessFormId",
                        column: x => x.BusinessFormId,
                        principalTable: "BusinessForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMessages_BusinessFormId",
                table: "BusinessMessages",
                column: "BusinessFormId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFormDatas_BusinessFormId",
                table: "UserFormDatas",
                column: "BusinessFormId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessMessages_BusinessForms_BusinessFormId",
                table: "BusinessMessages",
                column: "BusinessFormId",
                principalTable: "BusinessForms",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessMessages_BusinessForms_BusinessFormId",
                table: "BusinessMessages");

            migrationBuilder.DropTable(
                name: "DialogSessions");

            migrationBuilder.DropTable(
                name: "UserFormDatas");

            migrationBuilder.DropTable(
                name: "BusinessForms");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMessages_BusinessFormId",
                table: "BusinessMessages");

            migrationBuilder.DropColumn(
                name: "CanUseNLPMapping",
                table: "InboundMessages");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "InboundMessages");

            migrationBuilder.DropColumn(
                name: "BusinessFormId",
                table: "BusinessMessages");

            migrationBuilder.DropColumn(
                name: "ShouldTriggerFormProcessing",
                table: "BusinessMessages");
        }
    }
}
