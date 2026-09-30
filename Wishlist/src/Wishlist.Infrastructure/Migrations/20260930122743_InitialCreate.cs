using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wishlist.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductListItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductListId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    Quantity = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductListItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductListItems_ProductLists_ProductListId",
                        column: x => x.ProductListId,
                        principalTable: "ProductLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductListItems_ProductListId",
                table: "ProductListItems",
                column: "ProductListId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLists_CustomerId",
                table: "ProductLists",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductListItems");

            migrationBuilder.DropTable(
                name: "ProductLists");
        }
    }
}
