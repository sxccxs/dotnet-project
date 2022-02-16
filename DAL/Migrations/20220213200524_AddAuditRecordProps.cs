using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class AddAuditRecordProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NewRoleId",
                table: "AuditRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldRoleId",
                table: "AuditRecords",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditRecords_NewRoleId",
                table: "AuditRecords",
                column: "NewRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditRecords_OldRoleId",
                table: "AuditRecords",
                column: "OldRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditRecords_RoleTypes_NewRoleId",
                table: "AuditRecords",
                column: "NewRoleId",
                principalTable: "RoleTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditRecords_RoleTypes_OldRoleId",
                table: "AuditRecords",
                column: "OldRoleId",
                principalTable: "RoleTypes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditRecords_RoleTypes_NewRoleId",
                table: "AuditRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditRecords_RoleTypes_OldRoleId",
                table: "AuditRecords");

            migrationBuilder.DropIndex(
                name: "IX_AuditRecords_NewRoleId",
                table: "AuditRecords");

            migrationBuilder.DropIndex(
                name: "IX_AuditRecords_OldRoleId",
                table: "AuditRecords");

            migrationBuilder.DropColumn(
                name: "NewRoleId",
                table: "AuditRecords");

            migrationBuilder.DropColumn(
                name: "OldRoleId",
                table: "AuditRecords");
        }
    }
}
