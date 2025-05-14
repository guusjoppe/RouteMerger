using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RouteMerger.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedMergedFileReferencetoRoute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MergedFileReferenceId",
                table: "Routes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Routes_MergedFileReferenceId",
                table: "Routes",
                column: "MergedFileReferenceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_FileReferences_MergedFileReferenceId",
                table: "Routes",
                column: "MergedFileReferenceId",
                principalTable: "FileReferences",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routes_FileReferences_MergedFileReferenceId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_MergedFileReferenceId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "MergedFileReferenceId",
                table: "Routes");
        }
    }
}
