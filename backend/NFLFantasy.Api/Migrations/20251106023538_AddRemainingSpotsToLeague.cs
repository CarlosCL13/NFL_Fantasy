using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NFLFantasy.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainingSpotsToLeague : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RemainingSpots",
                table: "Leagues",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingSpots",
                table: "Leagues");
        }
    }
}
