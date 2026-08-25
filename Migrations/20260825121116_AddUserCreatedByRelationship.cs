using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TawakalApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCreatedByRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "T_a_portal_users",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_a_portal_users_CreatedByUserId",
                table: "T_a_portal_users",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_T_a_portal_users_T_a_portal_users_CreatedByUserId",
                table: "T_a_portal_users",
                column: "CreatedByUserId",
                principalTable: "T_a_portal_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_a_portal_users_T_a_portal_users_CreatedByUserId",
                table: "T_a_portal_users");

            migrationBuilder.DropIndex(
                name: "IX_T_a_portal_users_CreatedByUserId",
                table: "T_a_portal_users");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "T_a_portal_users");
        }
    }
}
