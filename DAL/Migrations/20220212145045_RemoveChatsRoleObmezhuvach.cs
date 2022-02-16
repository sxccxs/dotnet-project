using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
#pragma warning disable CA1704
    public partial class RemoveChatsRoleObmezhuvach : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TextChats_RoleTypes_AccessRoleTypeId",
                table: "TextChats");

            migrationBuilder.DropForeignKey(
                name: "FK_VoiceChats_RoleTypes_AccessRoleTypeId",
                table: "VoiceChats");

            migrationBuilder.DropIndex(
                name: "IX_VoiceChats_AccessRoleTypeId",
                table: "VoiceChats");

            migrationBuilder.DropIndex(
                name: "IX_TextChats_AccessRoleTypeId",
                table: "TextChats");

            migrationBuilder.DropColumn(
                name: "AccessRoleTypeId",
                table: "VoiceChats");

            migrationBuilder.DropColumn(
                name: "AccessRoleTypeId",
                table: "TextChats");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccessRoleTypeId",
                table: "VoiceChats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AccessRoleTypeId",
                table: "TextChats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VoiceChats_AccessRoleTypeId",
                table: "VoiceChats",
                column: "AccessRoleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TextChats_AccessRoleTypeId",
                table: "TextChats",
                column: "AccessRoleTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TextChats_RoleTypes_AccessRoleTypeId",
                table: "TextChats",
                column: "AccessRoleTypeId",
                principalTable: "RoleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoiceChats_RoleTypes_AccessRoleTypeId",
                table: "VoiceChats",
                column: "AccessRoleTypeId",
                principalTable: "RoleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
