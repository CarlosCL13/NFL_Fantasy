using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NFLFantasy.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDesignacionTableAndPlayerNewsRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Designacion",
                table: "PlayerNews");

            migrationBuilder.AddColumn<int>(
                name: "DesignacionId",
                table: "PlayerNews",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Designaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Designaciones", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerNews_DesignacionId",
                table: "PlayerNews",
                column: "DesignacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerNews_Designaciones_DesignacionId",
                table: "PlayerNews",
                column: "DesignacionId",
                principalTable: "Designaciones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerNews_Designaciones_DesignacionId",
                table: "PlayerNews");

            migrationBuilder.DropTable(
                name: "Designaciones");

            migrationBuilder.DropIndex(
                name: "IX_PlayerNews_DesignacionId",
                table: "PlayerNews");

            migrationBuilder.DropColumn(
                name: "DesignacionId",
                table: "PlayerNews");

            migrationBuilder.AddColumn<string>(
                name: "Designacion",
                table: "PlayerNews",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
