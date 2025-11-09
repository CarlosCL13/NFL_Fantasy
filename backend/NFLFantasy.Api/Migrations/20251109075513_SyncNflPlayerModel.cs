using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NFLFantasy.Api.Migrations
{
    /// <inheritdoc />
    public partial class SyncNflPlayerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "NflPlayers");

            migrationBuilder.AddColumn<int>(
                name: "PositionId",
                table: "NflPlayers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_NflPlayers_PositionId",
                table: "NflPlayers",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_NflPlayers_Positions_PositionId",
                table: "NflPlayers",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NflPlayers_Positions_PositionId",
                table: "NflPlayers");

            migrationBuilder.DropIndex(
                name: "IX_NflPlayers_PositionId",
                table: "NflPlayers");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "NflPlayers");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "NflPlayers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
