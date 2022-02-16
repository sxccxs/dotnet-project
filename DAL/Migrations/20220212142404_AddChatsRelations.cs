using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class AddChatsRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccessRoleTypeId",
                table: "VoiceChats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "VoiceChats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
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

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "TextChats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "TextChats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TextChatModelUserModel",
                columns: table => new
                {
                    TextChatsId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextChatModelUserModel", x => new { x.TextChatsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_TextChatModelUserModel_TextChats_TextChatsId",
                        column: x => x.TextChatsId,
                        principalTable: "TextChats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TextChatModelUserModel_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserModelVoiceChatModel",
                columns: table => new
                {
                    UsersId = table.Column<int>(type: "int", nullable: false),
                    VoiceChatsId = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserModelVoiceChatModel", x => new { x.UsersId, x.VoiceChatsId });
                    table.ForeignKey(
                        name: "FK_UserModelVoiceChatModel_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserModelVoiceChatModel_VoiceChats_VoiceChatsId",
                        column: x => x.VoiceChatsId,
                        principalTable: "VoiceChats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VoiceChats_AccessRoleTypeId",
                table: "VoiceChats",
                column: "AccessRoleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceChats_RoomId",
                table: "VoiceChats",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_TextChats_AccessRoleTypeId",
                table: "TextChats",
                column: "AccessRoleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TextChats_RoomId",
                table: "TextChats",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_TextChatModelUserModel_UsersId",
                table: "TextChatModelUserModel",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_UserModelVoiceChatModel_VoiceChatsId",
                table: "UserModelVoiceChatModel",
                column: "VoiceChatsId");

            migrationBuilder.AddForeignKey(
                name: "FK_TextChats_RoleTypes_AccessRoleTypeId",
                table: "TextChats",
                column: "AccessRoleTypeId",
                principalTable: "RoleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TextChats_Rooms_RoomId",
                table: "TextChats",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoiceChats_RoleTypes_AccessRoleTypeId",
                table: "VoiceChats",
                column: "AccessRoleTypeId",
                principalTable: "RoleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoiceChats_Rooms_RoomId",
                table: "VoiceChats",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TextChats_RoleTypes_AccessRoleTypeId",
                table: "TextChats");

            migrationBuilder.DropForeignKey(
                name: "FK_TextChats_Rooms_RoomId",
                table: "TextChats");

            migrationBuilder.DropForeignKey(
                name: "FK_VoiceChats_RoleTypes_AccessRoleTypeId",
                table: "VoiceChats");

            migrationBuilder.DropForeignKey(
                name: "FK_VoiceChats_Rooms_RoomId",
                table: "VoiceChats");

            migrationBuilder.DropTable(
                name: "TextChatModelUserModel");

            migrationBuilder.DropTable(
                name: "UserModelVoiceChatModel");

            migrationBuilder.DropIndex(
                name: "IX_VoiceChats_AccessRoleTypeId",
                table: "VoiceChats");

            migrationBuilder.DropIndex(
                name: "IX_VoiceChats_RoomId",
                table: "VoiceChats");

            migrationBuilder.DropIndex(
                name: "IX_TextChats_AccessRoleTypeId",
                table: "TextChats");

            migrationBuilder.DropIndex(
                name: "IX_TextChats_RoomId",
                table: "TextChats");

            migrationBuilder.DropColumn(
                name: "AccessRoleTypeId",
                table: "VoiceChats");

            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "VoiceChats");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "VoiceChats");

            migrationBuilder.DropColumn(
                name: "AccessRoleTypeId",
                table: "TextChats");

            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "TextChats");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "TextChats");
        }
    }
}
