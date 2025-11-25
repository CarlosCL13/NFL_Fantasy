using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NFLFantasy.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerNewsAuditColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Auditoria",
                table: "PlayerNews",
                newName: "HoraCreacion");

            migrationBuilder.AddColumn<string>(
                name: "Cambios",
                table: "PlayerNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cambios",
                table: "PlayerNews");

            migrationBuilder.RenameColumn(
                name: "HoraCreacion",
                table: "PlayerNews",
                newName: "Auditoria");
        }
    }
}
