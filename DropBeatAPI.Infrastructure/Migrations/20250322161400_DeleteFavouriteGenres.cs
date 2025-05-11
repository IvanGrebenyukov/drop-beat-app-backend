using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DropBeatAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteFavouriteGenres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGenres_AspNetUsers_UserId1",
                table: "UserGenres");

            migrationBuilder.DropIndex(
                name: "IX_UserGenres_UserId1",
                table: "UserGenres");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserGenres");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "UserGenres",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGenres_UserId1",
                table: "UserGenres",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGenres_AspNetUsers_UserId1",
                table: "UserGenres",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
