using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RouteMerger.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsMergedToFileReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMerged",
                table: "FileReferences",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_FileReferences_IsMerged",
                table: "FileReferences",
                column: "IsMerged");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FileReferences_IsMerged",
                table: "FileReferences");

            migrationBuilder.DropColumn(
                name: "IsMerged",
                table: "FileReferences");
        }
    }
}
