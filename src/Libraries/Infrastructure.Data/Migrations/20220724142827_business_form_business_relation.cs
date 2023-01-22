using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class business_form_business_relation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BusinessForms_BusinessId",
                table: "BusinessForms",
                column: "BusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessForms_Businesses_BusinessId",
                table: "BusinessForms",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessForms_Businesses_BusinessId",
                table: "BusinessForms");

            migrationBuilder.DropIndex(
                name: "IX_BusinessForms_BusinessId",
                table: "BusinessForms");
        }
    }
}
