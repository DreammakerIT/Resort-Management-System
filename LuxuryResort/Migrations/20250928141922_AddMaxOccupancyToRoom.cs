using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LuxuryResort.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxOccupancyToRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxOccupancy",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxOccupancy",
                table: "Rooms");
        }
    }
}
