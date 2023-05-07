using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestGenerator.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedCreatedAtpropertyontheTestmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Test",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Test");
        }
    }
}
