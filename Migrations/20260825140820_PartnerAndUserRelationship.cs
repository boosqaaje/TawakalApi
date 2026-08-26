using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TawakalApi.Migrations
{
    /// <inheritdoc />
    public partial class PartnerAndUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PartnerId",
                table: "T_a_portal_users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_a_portal_users_PartnerId",
                table: "T_a_portal_users",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_T_a_portal_users_T_a_partners_PartnerId",
                table: "T_a_portal_users",
                column: "PartnerId",
                principalTable: "T_a_partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_a_portal_users_T_a_partners_PartnerId",
                table: "T_a_portal_users");

            migrationBuilder.DropIndex(
                name: "IX_T_a_portal_users_PartnerId",
                table: "T_a_portal_users");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "T_a_portal_users");
        }
    }
}
