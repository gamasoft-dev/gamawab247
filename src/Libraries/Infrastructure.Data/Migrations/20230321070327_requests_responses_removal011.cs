﻿using System;
using Domain.Entities.RequestAndComplaints;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    public partial class requests_responses_removal011 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestAndComplaints");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestAndComplaints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    CallBackUrl = table.Column<string>(type: "text", nullable: true),
                    Channel = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CustomerId = table.Column<string>(type: "text", nullable: true),
                    Detail = table.Column<string>(type: "text", nullable: true),
                    ResolutionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResolutionStatus = table.Column<string>(type: "text", nullable: true),
                    ResponsList = table.Column<RequestAndComplaintResponsList>(type: "jsonb", nullable: true),
                    Subject = table.Column<string>(type: "text", nullable: true),
                    TicketId = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestAndComplaints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestAndComplaints_Users_TreatedById",
                        column: x => x.TreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestAndComplaints_TreatedById",
                table: "RequestAndComplaints",
                column: "TreatedById");
        }
    }
}
