using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DropBeatAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImproveRelationshipsAndIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Beats_BeatId",
                table: "CartItems");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_CreatedAt",
                table: "Reports",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_PurchaseDate",
                table: "Purchases",
                column: "PurchaseDate");

            migrationBuilder.CreateIndex(
                name: "IX_BeatLikes_UserId_BeatId",
                table: "BeatLikes",
                columns: new[] { "UserId", "BeatId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Beats_BeatId",
                table: "CartItems",
                column: "BeatId",
                principalTable: "Beats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Beats_BeatId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_Reports_CreatedAt",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_PurchaseDate",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_BeatLikes_UserId_BeatId",
                table: "BeatLikes");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Beats_BeatId",
                table: "CartItems",
                column: "BeatId",
                principalTable: "Beats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
