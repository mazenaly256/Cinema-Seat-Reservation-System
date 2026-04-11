using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace movie_service.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovieGenres",
                columns: table => new
                {
                    MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieGenres", x => new { x.MovieId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_MovieGenres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieGenres_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Showtimes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Showtimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Showtimes_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), "Sci-Fi" },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "Action" },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "Drama" },
                    { new Guid("20000000-0000-0000-0000-000000000004"), "Crime" },
                    { new Guid("20000000-0000-0000-0000-000000000005"), "Thriller" }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "DurationMinutes", "Name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), 180, "Oppenheimer" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), 166, "Dune: Part Two" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), 148, "Spider-Man: No Way Home" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), 176, "The Batman" },
                    { new Guid("10000000-0000-0000-0000-000000000005"), 169, "Interstellar" }
                });

            migrationBuilder.InsertData(
                table: "MovieGenres",
                columns: new[] { "GenreId", "MovieId" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("20000000-0000-0000-0000-000000000005"), new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("20000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("20000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000003") },
                    { new Guid("20000000-0000-0000-0000-000000000005"), new Guid("10000000-0000-0000-0000-000000000003") },
                    { new Guid("20000000-0000-0000-0000-000000000004"), new Guid("10000000-0000-0000-0000-000000000004") },
                    { new Guid("20000000-0000-0000-0000-000000000005"), new Guid("10000000-0000-0000-0000-000000000004") },
                    { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000005") },
                    { new Guid("20000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000005") }
                });

            migrationBuilder.InsertData(
                table: "Showtimes",
                columns: new[] { "Id", "EndTime", "MovieId", "StartTime" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 11, 21, 0, 0, 0, DateTimeKind.Unspecified), new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 11, 18, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("30000000-0000-0000-0000-000000000002"), new DateTime(2026, 4, 11, 23, 36, 0, 0, DateTimeKind.Unspecified), new Guid("10000000-0000-0000-0000-000000000002"), new DateTime(2026, 4, 11, 21, 30, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("30000000-0000-0000-0000-000000000003"), new DateTime(2026, 4, 12, 18, 28, 0, 0, DateTimeKind.Unspecified), new Guid("10000000-0000-0000-0000-000000000003"), new DateTime(2026, 4, 12, 16, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("30000000-0000-0000-0000-000000000004"), new DateTime(2026, 4, 12, 21, 56, 0, 0, DateTimeKind.Unspecified), new Guid("10000000-0000-0000-0000-000000000004"), new DateTime(2026, 4, 12, 19, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("30000000-0000-0000-0000-000000000005"), new DateTime(2026, 4, 13, 22, 49, 0, 0, DateTimeKind.Unspecified), new Guid("10000000-0000-0000-0000-000000000005"), new DateTime(2026, 4, 13, 20, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("30000000-0000-0000-0000-000000000006"), new DateTime(2026, 4, 11, 13, 0, 0, 0, DateTimeKind.Unspecified), new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 11, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("30000000-0000-0000-0000-000000000007"), new DateTime(2026, 4, 12, 14, 0, 0, 0, DateTimeKind.Unspecified), new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 12, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("30000000-0000-0000-0000-000000000008"), new DateTime(2026, 4, 13, 16, 36, 0, 0, DateTimeKind.Unspecified), new Guid("10000000-0000-0000-0000-000000000002"), new DateTime(2026, 4, 13, 14, 30, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("30000000-0000-0000-0000-000000000009"), new DateTime(2026, 4, 11, 17, 28, 0, 0, DateTimeKind.Unspecified), new Guid("10000000-0000-0000-0000-000000000003"), new DateTime(2026, 4, 11, 15, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("30000000-0000-0000-0000-000000000010"), new DateTime(2026, 4, 13, 13, 49, 0, 0, DateTimeKind.Unspecified), new Guid("10000000-0000-0000-0000-000000000005"), new DateTime(2026, 4, 13, 9, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenres_GenreId",
                table: "MovieGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Showtimes_MovieId",
                table: "Showtimes",
                column: "MovieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieGenres");

            migrationBuilder.DropTable(
                name: "Showtimes");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}
