using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backendAPI.Migrations
{
    public partial class changehourrange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HourRange",
                table: "LocationHourRangeModel");

            migrationBuilder.AddColumn<DateTime>(
                name: "Hour",
                table: "LocationHourRangeModel",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hour",
                table: "LocationHourRangeModel");

            migrationBuilder.AddColumn<string>(
                name: "HourRange",
                table: "LocationHourRangeModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
