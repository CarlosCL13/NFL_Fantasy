using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NFLFantasy.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleToUserAndBaseTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultPositions",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "DefaultScoring",
                table: "Leagues");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultPositions",
                table: "Leagues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DefaultScoring",
                table: "Leagues",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
