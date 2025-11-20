using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoundTrack.Server.Migrations
{
    /// <inheritdoc />
    public partial class RemoveShadowProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AlbumProfiles_AlbumProfileId1",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_ArtistProfiles_ArtistProfileId1",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_SongProfiles_SongProfileId1",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_AlbumProfileId1",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ArtistProfileId1",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_SongProfileId1",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "AlbumProfileId1",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ArtistProfileId1",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "SongProfileId1",
                table: "Reviews");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlbumProfileId1",
                table: "Reviews",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArtistProfileId1",
                table: "Reviews",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SongProfileId1",
                table: "Reviews",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_AlbumProfileId1",
                table: "Reviews",
                column: "AlbumProfileId1");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ArtistProfileId1",
                table: "Reviews",
                column: "ArtistProfileId1");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_SongProfileId1",
                table: "Reviews",
                column: "SongProfileId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AlbumProfiles_AlbumProfileId1",
                table: "Reviews",
                column: "AlbumProfileId1",
                principalTable: "AlbumProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_ArtistProfiles_ArtistProfileId1",
                table: "Reviews",
                column: "ArtistProfileId1",
                principalTable: "ArtistProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_SongProfiles_SongProfileId1",
                table: "Reviews",
                column: "SongProfileId1",
                principalTable: "SongProfiles",
                principalColumn: "Id");
        }
    }
}
