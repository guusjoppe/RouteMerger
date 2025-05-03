using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RouteMerger.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserProvidedNameToFileReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserProvidedName",
                table: "FileReferences",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserProvidedName",
                table: "FileReferences");
        }
    }
}
