using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestGenerator.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedwrongnamefortheTesttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_TestItem_TestId",
                table: "Question");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestItem",
                table: "TestItem");

            migrationBuilder.RenameTable(
                name: "TestItem",
                newName: "Test");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Test",
                table: "Test",
                column: "TestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_Test_TestId",
                table: "Question",
                column: "TestId",
                principalTable: "Test",
                principalColumn: "TestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_Test_TestId",
                table: "Question");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Test",
                table: "Test");

            migrationBuilder.RenameTable(
                name: "Test",
                newName: "TestItem");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestItem",
                table: "TestItem",
                column: "TestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_TestItem_TestId",
                table: "Question",
                column: "TestId",
                principalTable: "TestItem",
                principalColumn: "TestId");
        }
    }
}
