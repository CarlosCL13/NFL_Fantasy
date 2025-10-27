using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NFLFantasy.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLeagueAuditAndFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alias",
                table: "Teams",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Leagues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "LeagueAudits",
                columns: table => new
                {
                    LeagueAuditId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LeagueId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueAudits", x => x.LeagueAuditId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeagueAudits");

            migrationBuilder.DropColumn(
                name: "Alias",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Leagues");
        }
    }
}
