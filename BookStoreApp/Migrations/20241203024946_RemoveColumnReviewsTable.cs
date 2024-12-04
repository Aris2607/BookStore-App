using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveColumnReviewsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "Reviews");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
