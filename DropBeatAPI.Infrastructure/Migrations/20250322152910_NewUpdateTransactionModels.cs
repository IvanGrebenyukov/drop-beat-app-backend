using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DropBeatAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewUpdateTransactionModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellerRequest_AspNetUsers_UserId",
                table: "SellerRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_AspNetUsers_UserId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Purchases_PurchaseId",
                table: "Transaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SellerRequest",
                table: "SellerRequest");

            migrationBuilder.RenameTable(
                name: "Transaction",
                newName: "Transactions");

            migrationBuilder.RenameTable(
                name: "SellerRequest",
                newName: "SellerRequests");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_UserId",
                table: "Transactions",
                newName: "IX_Transactions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_PurchaseId",
                table: "Transactions",
                newName: "IX_Transactions_PurchaseId");

            migrationBuilder.RenameIndex(
                name: "IX_SellerRequest_UserId",
                table: "SellerRequests",
                newName: "IX_SellerRequests_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SellerRequests",
                table: "SellerRequests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SellerRequests_AspNetUsers_UserId",
                table: "SellerRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_AspNetUsers_UserId",
                table: "Transactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Purchases_PurchaseId",
                table: "Transactions",
                column: "PurchaseId",
                principalTable: "Purchases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellerRequests_AspNetUsers_UserId",
                table: "SellerRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_AspNetUsers_UserId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Purchases_PurchaseId",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SellerRequests",
                table: "SellerRequests");

            migrationBuilder.RenameTable(
                name: "Transactions",
                newName: "Transaction");

            migrationBuilder.RenameTable(
                name: "SellerRequests",
                newName: "SellerRequest");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_UserId",
                table: "Transaction",
                newName: "IX_Transaction_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_PurchaseId",
                table: "Transaction",
                newName: "IX_Transaction_PurchaseId");

            migrationBuilder.RenameIndex(
                name: "IX_SellerRequests_UserId",
                table: "SellerRequest",
                newName: "IX_SellerRequest_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SellerRequest",
                table: "SellerRequest",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SellerRequest_AspNetUsers_UserId",
                table: "SellerRequest",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_AspNetUsers_UserId",
                table: "Transaction",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Purchases_PurchaseId",
                table: "Transaction",
                column: "PurchaseId",
                principalTable: "Purchases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
