using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestGenerator.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedAuthorandAuthorIdpropertiesontheTestmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Test",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Test_AuthorId",
                table: "Test",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Test_AspNetUsers_AuthorId",
                table: "Test",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Test_AspNetUsers_AuthorId",
                table: "Test");

            migrationBuilder.DropIndex(
                name: "IX_Test_AuthorId",
                table: "Test");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Test");
        }
    }
}
