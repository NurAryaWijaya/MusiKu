using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusiKu_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentityCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Playlists",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "varchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RecentlyPlayeds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PlaylistId = table.Column<int>(type: "int", nullable: false),
                    PlayedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecentlyPlayeds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecentlyPlayeds_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecentlyPlayeds_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_AppUserId",
                table: "Playlists",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecentlyPlayeds_PlaylistId",
                table: "RecentlyPlayeds",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_RecentlyPlayeds_UserId",
                table: "RecentlyPlayeds",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_AspNetUsers_AppUserId",
                table: "Playlists",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_AspNetUsers_AppUserId",
                table: "Playlists");

            migrationBuilder.DropTable(
                name: "RecentlyPlayeds");

            migrationBuilder.DropIndex(
                name: "IX_Playlists_AppUserId",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");
        }
    }
}
