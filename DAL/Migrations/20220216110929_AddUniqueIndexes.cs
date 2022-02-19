using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class AddUniqueIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditRecords_Messages_MessageId",
                table: "AuditRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditRecords_Users_UserId",
                table: "AuditRecords");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AuditRecords",
                newName: "ActorId");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "AuditRecords",
                newName: "UserUnderActionId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditRecords_UserId",
                table: "AuditRecords",
                newName: "IX_AuditRecords_ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditRecords_MessageId",
                table: "AuditRecords",
                newName: "IX_AuditRecords_UserUnderActionId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RoleTypes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ForwardToTextChatId",
                table: "AuditRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ActionTypes",
                type: "nvarchar(450)",
                nullable: false,
#pragma warning disable SA1122
                defaultValue: "",
#pragma warning restore SA1122
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleTypes_Name",
                table: "RoleTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditRecords_ForwardToTextChatId",
                table: "AuditRecords",
                column: "ForwardToTextChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionTypes_Name",
                table: "ActionTypes",
                column: "Name",
                unique: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_AuditRecords_Users_UserUnderActionId",
                table: "AuditRecords",
                column: "UserUnderActionId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditRecords_TextChats_ForwardToTextChatId",
                table: "AuditRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditRecords_Users_ActorId",
                table: "AuditRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditRecords_Users_UserUnderActionId",
                table: "AuditRecords");

            migrationBuilder.DropIndex(
                name: "IX_RoleTypes_Name",
                table: "RoleTypes");

            migrationBuilder.DropIndex(
                name: "IX_AuditRecords_ForwardToTextChatId",
                table: "AuditRecords");

            migrationBuilder.DropIndex(
                name: "IX_ActionTypes_Name",
                table: "ActionTypes");

            migrationBuilder.DropColumn(
                name: "ForwardToTextChatId",
                table: "AuditRecords");

            migrationBuilder.RenameColumn(
                name: "UserUnderActionId",
                table: "AuditRecords",
                newName: "MessageId");

            migrationBuilder.RenameColumn(
                name: "ActorId",
                table: "AuditRecords",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditRecords_UserUnderActionId",
                table: "AuditRecords",
                newName: "IX_AuditRecords_MessageId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditRecords_ActorId",
                table: "AuditRecords",
                newName: "IX_AuditRecords_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RoleTypes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ActionTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditRecords_Messages_MessageId",
                table: "AuditRecords",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditRecords_Users_UserId",
                table: "AuditRecords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
