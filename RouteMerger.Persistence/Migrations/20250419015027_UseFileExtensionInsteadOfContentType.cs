using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RouteMerger.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UseFileExtensionInsteadOfContentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "FileReferences");

            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "FileReferences",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "FileReferences");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "FileReferences",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
