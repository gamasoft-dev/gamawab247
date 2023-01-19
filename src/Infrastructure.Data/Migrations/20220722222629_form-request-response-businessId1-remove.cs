using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class formrequestresponsebusinessId1remove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //   name: "FK_FormRequestResponses_BusinessForms_BusinessFormId1",
            //   table: "FormRequestResponses");


            //migrationBuilder.DropColumn(
            //    name: "BusinessFormId1",
            //    table: "FormRequestResponses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
            // name: "IX_FormRequestResponses_BusinessFormId1",
            // table: "FormRequestResponses");
        }
    }
}
