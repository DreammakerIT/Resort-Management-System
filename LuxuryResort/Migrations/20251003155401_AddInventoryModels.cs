using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LuxuryResort.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Rooms_RoomId",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "Bookings",
                newName: "RoomInstanceId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_RoomId",
                table: "Bookings",
                newName: "IX_Bookings_RoomInstanceId");

            migrationBuilder.AddColumn<int>(
                name: "TotalRooms",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RoomInstances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomInstances_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomInstances_RoomId",
                table: "RoomInstances",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_RoomInstances_RoomInstanceId",
                table: "Bookings",
                column: "RoomInstanceId",
                principalTable: "RoomInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_RoomInstances_RoomInstanceId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "RoomInstances");

            migrationBuilder.DropColumn(
                name: "TotalRooms",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "RoomInstanceId",
                table: "Bookings",
                newName: "RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_RoomInstanceId",
                table: "Bookings",
                newName: "IX_Bookings_RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Rooms_RoomId",
                table: "Bookings",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
