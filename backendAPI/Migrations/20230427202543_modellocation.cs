using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backendAPI.Migrations
{
    public partial class modellocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LocationHourRangeModel",
                table: "LocationHourRangeModel");

            migrationBuilder.RenameTable(
                name: "LocationHourRangeModel",
                newName: "Location");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Location",
                table: "Location",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Location",
                table: "Location");

            migrationBuilder.RenameTable(
                name: "Location",
                newName: "LocationHourRangeModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocationHourRangeModel",
                table: "LocationHourRangeModel",
                column: "Id");
        }
    }
}
