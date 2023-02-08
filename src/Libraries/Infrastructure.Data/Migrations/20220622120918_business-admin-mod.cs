using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class businessadminmod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c7b013f0-5201-4317-abd8-c211f91b7330"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c7b013f0-5201-4317-abd8-c222f91b7330"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("fab4fac1-c546-41de-aebc-a14da6895711"));

            //migrationBuilder.AddColumn<Guid>(
            //    name: "BusinessId",
            //    table: "Users",
            //    type: "uuid",
            //    nullable: true);

            //migrationBuilder.AddColumn<bool>(
            //    name: "IsResponsePermitted",
            //    table: "TextMessages",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);

            //migrationBuilder.AddColumn<string>(
            //    name: "KeyResponses",
            //    table: "TextMessages",
            //    type: "text",
            //    nullable: true);

            //migrationBuilder.AddColumn<bool>(
            //    name: "IsWebhookConfigured",
            //    table: "BusinessMessageSettings",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminUserId",
                table: "Businesses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_BusinessId",
                table: "Users",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_AdminUserId",
                table: "Businesses",
                column: "AdminUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Businesses_Users_AdminUserId",
                table: "Businesses",
                column: "AdminUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Users_Businesses_BusinessId",
            //    table: "Users",
            //    column: "BusinessId",
            //    principalTable: "Businesses",
            //    principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Businesses_Users_AdminUserId",
                table: "Businesses");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Businesses_BusinessId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BusinessId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Businesses_AdminUserId",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "BusinessId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsResponsePermitted",
                table: "TextMessages");

            migrationBuilder.DropColumn(
                name: "KeyResponses",
                table: "TextMessages");

            migrationBuilder.DropColumn(
                name: "IsWebhookConfigured",
                table: "BusinessMessageSettings");

            migrationBuilder.DropColumn(
                name: "AdminUserId",
                table: "Businesses");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("c7b013f0-5201-4317-abd8-c211f91b7330"), "2", "ADMIN", "ADMIN" },
                    { new Guid("c7b013f0-5201-4317-abd8-c222f91b7330"), "3", "SUPERADMIN", "SUPERADMIN" },
                    { new Guid("fab4fac1-c546-41de-aebc-a14da6895711"), "1", "USER", "USER" }
                });
        }
    }
}
