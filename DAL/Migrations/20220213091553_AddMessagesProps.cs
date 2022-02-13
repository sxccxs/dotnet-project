using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class AddMessagesProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEdited",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReplyToId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoiceChats_Name_RoomId",
                table: "VoiceChats",
                columns: new[] { "Name", "RoomId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TextChats_Name_RoomId",
                table: "TextChats",
                columns: new[] { "Name", "RoomId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReplyToId",
                table: "Messages",
                column: "ReplyToId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Messages_ReplyToId",
                table: "Messages",
                column: "ReplyToId",
                principalTable: "Messages",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Messages_ReplyToId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_VoiceChats_Name_RoomId",
                table: "VoiceChats");

            migrationBuilder.DropIndex(
                name: "IX_TextChats_Name_RoomId",
                table: "TextChats");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ReplyToId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsEdited",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ReplyToId",
                table: "Messages");
        }
    }
}
