using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SoundTrack.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReviewComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Author = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Likes = table.Column<int>(type: "integer", nullable: false),
                    Dislikes = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewComments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    salt = table.Column<string>(type: "text", nullable: false),
                    ProfilePictureUrl = table.Column<string>(type: "text", nullable: false),
                    BirthDay = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlbumProfiles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    score = table.Column<int>(type: "integer", nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Genres = table.Column<List<string>>(type: "text[]", nullable: false),
                    Tags = table.Column<List<string>>(type: "text[]", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlbumProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ArtistProfiles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    score = table.Column<int>(type: "integer", nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Genres = table.Column<List<string>>(type: "text[]", nullable: false),
                    Tags = table.Column<List<string>>(type: "text[]", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserUser",
                columns: table => new
                {
                    FollowersId = table.Column<int>(type: "integer", nullable: false),
                    FollowingId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserUser", x => new { x.FollowersId, x.FollowingId });
                    table.ForeignKey(
                        name: "FK_UserUser_Users_FollowersId",
                        column: x => x.FollowersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserUser_Users_FollowingId",
                        column: x => x.FollowingId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SongProfiles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    AlbumId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    score = table.Column<int>(type: "integer", nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Genres = table.Column<List<string>>(type: "text[]", nullable: false),
                    Tags = table.Column<List<string>>(type: "text[]", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SongProfiles_AlbumProfiles_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "AlbumProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SongProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AlbumProfileArtistProfile",
                columns: table => new
                {
                    AlbumsId = table.Column<string>(type: "text", nullable: false),
                    ArtistId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumProfileArtistProfile", x => new { x.AlbumsId, x.ArtistId });
                    table.ForeignKey(
                        name: "FK_AlbumProfileArtistProfile_AlbumProfiles_AlbumsId",
                        column: x => x.AlbumsId,
                        principalTable: "AlbumProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumProfileArtistProfile_ArtistProfiles_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "ArtistProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArtistProfileSongProfile",
                columns: table => new
                {
                    ArtistsId = table.Column<string>(type: "text", nullable: false),
                    SongsId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistProfileSongProfile", x => new { x.ArtistsId, x.SongsId });
                    table.ForeignKey(
                        name: "FK_ArtistProfileSongProfile_ArtistProfiles_ArtistsId",
                        column: x => x.ArtistsId,
                        principalTable: "ArtistProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistProfileSongProfile_SongProfiles_SongsId",
                        column: x => x.SongsId,
                        principalTable: "SongProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    score = table.Column<int>(type: "integer", nullable: false),
                    AlbumProfileId = table.Column<string>(type: "text", nullable: true),
                    ArtistProfileId = table.Column<string>(type: "text", nullable: true),
                    SongProfileId = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Author = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Likes = table.Column<int>(type: "integer", nullable: false),
                    Dislikes = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_AlbumProfiles_AlbumProfileId",
                        column: x => x.AlbumProfileId,
                        principalTable: "AlbumProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reviews_ArtistProfiles_ArtistProfileId",
                        column: x => x.ArtistProfileId,
                        principalTable: "ArtistProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reviews_SongProfiles_SongProfileId",
                        column: x => x.SongProfileId,
                        principalTable: "SongProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumProfileArtistProfile_ArtistId",
                table: "AlbumProfileArtistProfile",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumProfiles_UserId",
                table: "AlbumProfiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistProfiles_UserId",
                table: "ArtistProfiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistProfileSongProfile_SongsId",
                table: "ArtistProfileSongProfile",
                column: "SongsId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_AlbumProfileId",
                table: "Reviews",
                column: "AlbumProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ArtistProfileId",
                table: "Reviews",
                column: "ArtistProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_SongProfileId",
                table: "Reviews",
                column: "SongProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SongProfiles_AlbumId",
                table: "SongProfiles",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_SongProfiles_UserId",
                table: "SongProfiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserUser_FollowingId",
                table: "UserUser",
                column: "FollowingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumProfileArtistProfile");

            migrationBuilder.DropTable(
                name: "ArtistProfileSongProfile");

            migrationBuilder.DropTable(
                name: "ReviewComments");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "UserUser");

            migrationBuilder.DropTable(
                name: "ArtistProfiles");

            migrationBuilder.DropTable(
                name: "SongProfiles");

            migrationBuilder.DropTable(
                name: "AlbumProfiles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
