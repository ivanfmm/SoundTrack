using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SoundTrack.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddArtistFollowSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArtistFollows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ArtistProfileId = table.Column<string>(type: "text", nullable: false),
                    FollowDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AlbumProfileId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistFollows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistFollows_AlbumProfiles_AlbumProfileId",
                        column: x => x.AlbumProfileId,
                        principalTable: "AlbumProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArtistFollows_ArtistProfiles_ArtistProfileId",
                        column: x => x.ArtistProfileId,
                        principalTable: "ArtistProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistFollows_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtistFollows_AlbumProfileId",
                table: "ArtistFollows",
                column: "AlbumProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistFollows_ArtistProfileId",
                table: "ArtistFollows",
                column: "ArtistProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistFollows_UserId",
                table: "ArtistFollows",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtistFollows");
        }
    }
}
