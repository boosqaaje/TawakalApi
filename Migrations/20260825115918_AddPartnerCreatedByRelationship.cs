using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TawakalApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPartnerCreatedByRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "T_a_partners",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_T_a_partners_CreatedBy",
                table: "T_a_partners",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_T_a_partners_T_a_portal_users_CreatedBy",
                table: "T_a_partners",
                column: "CreatedBy",
                principalTable: "T_a_portal_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_a_partners_T_a_portal_users_CreatedBy",
                table: "T_a_partners");

            migrationBuilder.DropIndex(
                name: "IX_T_a_partners_CreatedBy",
                table: "T_a_partners");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "T_a_partners");
        }
    }
}
