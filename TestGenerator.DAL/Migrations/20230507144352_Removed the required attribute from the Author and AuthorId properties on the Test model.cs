using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestGenerator.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemovedtherequiredattributefromtheAuthorandAuthorIdpropertiesontheTestmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Test_AspNetUsers_AuthorId",
                table: "Test");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Test",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Test_AspNetUsers_AuthorId",
                table: "Test",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Test_AspNetUsers_AuthorId",
                table: "Test");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Test",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Test_AspNetUsers_AuthorId",
                table: "Test",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
