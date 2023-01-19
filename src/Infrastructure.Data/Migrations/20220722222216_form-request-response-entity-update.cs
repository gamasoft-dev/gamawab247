using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class formrequestresponseentityupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormRequestResponses_BusinessForms_BusinessFormId",
                table: "FormRequestResponses");

            migrationBuilder.AddColumn<Guid>(
                name: "BusinessFormId1",
                table: "FormRequestResponses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormRequestResponses_BusinessFormId1",
                table: "FormRequestResponses",
                column: "BusinessFormId1");

            migrationBuilder.CreateIndex(
                name: "IX_FormRequestResponses_CreatedAt",
                table: "FormRequestResponses",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FormRequestResponses_From",
                table: "FormRequestResponses",
                column: "From");

            migrationBuilder.CreateIndex(
                name: "IX_FormRequestResponses_Status",
                table: "FormRequestResponses",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FormRequestResponses_To",
                table: "FormRequestResponses",
                column: "To");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormRequestResponses_BusinessForms_BusinessFormId",
                table: "FormRequestResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_FormRequestResponses_BusinessForms_BusinessFormId1",
                table: "FormRequestResponses");

            migrationBuilder.DropIndex(
                name: "IX_FormRequestResponses_BusinessFormId1",
                table: "FormRequestResponses");

            migrationBuilder.DropIndex(
                name: "IX_FormRequestResponses_CreatedAt",
                table: "FormRequestResponses");

            migrationBuilder.DropIndex(
                name: "IX_FormRequestResponses_From",
                table: "FormRequestResponses");

            migrationBuilder.DropIndex(
                name: "IX_FormRequestResponses_Status",
                table: "FormRequestResponses");

            migrationBuilder.DropIndex(
                name: "IX_FormRequestResponses_To",
                table: "FormRequestResponses");

                migrationBuilder.DropColumn(
                    name: "BusinessFormId1",
                    table: "FormRequestResponses");

            migrationBuilder.AddForeignKey(
                name: "FK_FormRequestResponses_BusinessForms_BusinessFormId",
                table: "FormRequestResponses",
                column: "BusinessFormId",
                principalTable: "BusinessForms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
