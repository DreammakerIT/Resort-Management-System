using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LuxuryResort.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToRoomInstance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "RoomInstances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "RoomInstances");
        }
    }
}
