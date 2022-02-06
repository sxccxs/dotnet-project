using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class AddRoomUserRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_RoleTypes_RoleId",
                table: "Roles");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "Roles",
                newName: "RoleTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Roles_RoleId",
                table: "Roles",
                newName: "IX_Roles_RoleTypeId");

            migrationBuilder.CreateTable(
                name: "RoomModelUserModel",
                columns: table => new
                {
                    RoomsId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomModelUserModel", x => new { x.RoomsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_RoomModelUserModel_Rooms_RoomsId",
                        column: x => x.RoomsId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomModelUserModel_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomModelUserModel_UsersId",
                table: "RoomModelUserModel",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_RoleTypes_RoleTypeId",
                table: "Roles",
                column: "RoleTypeId",
                principalTable: "RoleTypes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_RoleTypes_RoleTypeId",
                table: "Roles");

            migrationBuilder.DropTable(
                name: "RoomModelUserModel");

            migrationBuilder.RenameColumn(
                name: "RoleTypeId",
                table: "Roles",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Roles_RoleTypeId",
                table: "Roles",
                newName: "IX_Roles_RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_RoleTypes_RoleId",
                table: "Roles",
                column: "RoleId",
                principalTable: "RoleTypes",
                principalColumn: "Id");
        }
    }
}
