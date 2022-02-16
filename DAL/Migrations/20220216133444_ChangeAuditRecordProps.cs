using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class ChangeAuditRecordProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditRecords_TextChats_ForwardToTextChatId",
                table: "AuditRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditRecords_Users_ActorId",
                table: "AuditRecords");

            migrationBuilder.DropIndex(
                name: "IX_AuditRecords_ForwardToTextChatId",
                table: "AuditRecords");

            migrationBuilder.DropColumn(
                name: "ForwardToTextChatId",
                table: "AuditRecords");

            migrationBuilder.AlterColumn<int>(
                name: "ActorId",
                table: "AuditRecords",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditRecords_Users_ActorId",
                table: "AuditRecords",
                column: "ActorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditRecords_Users_ActorId",
                table: "AuditRecords");

            migrationBuilder.AlterColumn<int>(
                name: "ActorId",
                table: "AuditRecords",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ForwardToTextChatId",
                table: "AuditRecords",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditRecords_ForwardToTextChatId",
                table: "AuditRecords",
                column: "ForwardToTextChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditRecords_TextChats_ForwardToTextChatId",
                table: "AuditRecords",
                column: "ForwardToTextChatId",
                principalTable: "TextChats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditRecords_Users_ActorId",
                table: "AuditRecords",
                column: "ActorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
